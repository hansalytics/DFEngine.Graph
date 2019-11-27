using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DFEngine.Core.Models.MergeProcessor
{
    /// <summary>
    /// Handles the merging of multiple graphs
    /// </summary>
    public class MergeProcessor
    {
        MergeProcessorState state;
        Queue<Graph> queue;
        readonly object queueSync;
        readonly int amountThreads;
        readonly Queue<Worker> threadPool;
        readonly object threadPoolSync;

        public MergeProcessor()
        {
            state = MergeProcessorState.AWAITING_TASKS;
            queue = new Queue<Graph>();
            queueSync = new object();
            amountThreads = Environment.ProcessorCount;
            threadPool = new Queue<Worker>();
            threadPoolSync = new object();
            for (int index = 0; index < amountThreads; index++)
            {
                threadPool.Enqueue(new Worker());
            }
        }

        /// <summary>
        /// Gets the final result after all queued elements have been merged successfully
        /// </summary>
        public async Task<Graph> GetFinalResult()
        {
            state = MergeProcessorState.FINISHED;

            return await Task.Run(() =>
            {
                while (true)
                {
                    Thread.Sleep(10);

                    int graphsInQueue;
                    int waitingThreads;

                    lock (queueSync)
                    {
                        graphsInQueue = queue.Count;
                    }
                    lock (threadPoolSync)
                    {
                        waitingThreads = threadPool.Count;
                    }

                    //All threads have finished and theres exactly one graph left in the queue. The merging process is considered to be finished
                    if (graphsInQueue == 1 && waitingThreads == amountThreads)
                    {
                        Graph finalGraph;

                        lock (queueSync)
                        {
                            finalGraph = queue.Dequeue();
                        }

                        return finalGraph;
                    }

                    if (graphsInQueue == 0 && waitingThreads == amountThreads)
                        return new Graph();
                }
            });
        }

        /// <summary>
        /// Adds a new graph to the queue which shall be merged
        /// </summary>
        public void Queue(Graph newGraph)
        {
            if (state.Equals(MergeProcessorState.FINISHED))
                throw new InvalidOperationException("Can't queue new elements. Make sure not to add elements after calling 'GetFinalResult()'!");

            Worker nextWorker = null;
            Graph nextGraph_01 = null;
            Graph nextGraph_02 = null;

            lock (queueSync)
            {
                queue.Enqueue(newGraph);

                lock (threadPoolSync)
                {
                    if (queue.Count >= 2 && threadPool.Count > 0)
                    {
                        nextGraph_01 = queue.Dequeue();
                        nextGraph_02 = queue.Dequeue();

                        nextWorker = threadPool.Dequeue();
                    }
                }
            }

            if (nextWorker != null)
                nextWorker.Run(this, nextGraph_01, nextGraph_02);
        }

        /// <summary>
        /// Sets the processor to his original state
        /// </summary>
        public void Reset()
        {
            state = MergeProcessorState.AWAITING_TASKS;
            queue = new Queue<Graph>();
        }

        /// <summary>
        /// Gets called by the workers. Their result gets enqueued and the worker is assigned new graphs again if available.
        /// If not they get staged into the threadpool again
        /// </summary>
        /// <param name="worker">The calle</param>
        /// <param name="graph">The merge result of the calling worker</param>
        internal void NotifyReady(Worker worker, Graph graph)
        {
            bool anotherRound = false;
            Graph nextGraph_01 = null;
            Graph nextGraph_02 = null;

            lock (queueSync)
            {
                queue.Enqueue(graph);

                if (queue.Count >= 2)
                {
                    nextGraph_01 = queue.Dequeue();
                    nextGraph_02 = queue.Dequeue();
                    anotherRound = true;
                }
            }

            if (anotherRound)
                worker.Run(this, nextGraph_01, nextGraph_02);
            else
            {
                lock (threadPoolSync)
                {
                    threadPool.Enqueue(worker);
                }
            }
        }
    }
}
