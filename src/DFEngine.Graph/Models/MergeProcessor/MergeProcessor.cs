using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DFEngine.Graph.Models.MergeProcessor
{
    /// <summary>
    /// Handles the merging of multiple graphs
    /// </summary>
    public class MergeProcessor
    {
        internal MergeProcessorState state;
        internal Queue<Graph> queue;
        internal readonly object enqueueSync;
        readonly int threadsAvailable;
        readonly Queue<Task> taskPool;
        readonly object taskPoolSync;

        public MergeProcessor()
        {
            state = MergeProcessorState.AWAITING_TASKS;
            queue = new Queue<Graph>();
            enqueueSync = new object();
            threadsAvailable = (int)Math.Round((double)Environment.ProcessorCount / 2);
            taskPool = new Queue<Task>();
            taskPoolSync = new object();
        }

        /// <summary>
        /// Gets the final result after all queued elements have been merged successfully
        /// </summary>
        public async Task<Graph> GetFinalResult()
        {
            state = MergeProcessorState.FINISHED;

            while(taskPool.Count > 0)
            {
                var task = taskPool.Dequeue();
                await task;
            }

            if (queue.Count == 0)
                return new Graph();
            if (queue.Count == 1)
                return queue.Dequeue();
            else
                throw new Exception("Final result could not be determined");
        }

        /// <summary>
        /// Adds a new graph to the queue which shall be merged
        /// </summary>
        public void Queue(Graph newGraph)
        {
            if (newGraph == null)
                throw new ArgumentNullException("Graph may not be null");

            if (state.Equals(MergeProcessorState.FINISHED))
                throw new InvalidOperationException("Can't queue new elements. Make sure not to add elements after calling 'GetFinalResult()'!");

            lock (enqueueSync)
            {
                queue.Enqueue(newGraph);
            }

            Task workerTask = null;

            lock (enqueueSync)
            {
                if (queue.Count >= 2 && taskPool.Count < threadsAvailable)
                {
                    Graph nextGraph_01 = queue.Dequeue();
                    Graph nextGraph_02 = queue.Dequeue();

                    workerTask = new Worker().Run(this, nextGraph_01, nextGraph_02);
                }
            }

            if (workerTask != null)
            {
                lock (taskPoolSync)
                {
                    taskPool.Enqueue(workerTask);
                }
            }
        }

        /// <summary>
        /// Sets the processor to his original state
        /// </summary>
        public void Reset()
        {
            _ = GetFinalResult().Result;
            state = MergeProcessorState.AWAITING_TASKS;
            queue = new Queue<Graph>();
        }
    }
}
