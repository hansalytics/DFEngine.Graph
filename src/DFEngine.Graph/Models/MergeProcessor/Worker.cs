using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DFEngine.Graph.Models.MergeProcessor
{
    static class Worker
    {
        /// <summary>
        /// Runs a task of merging two graphs until its interrupted by its processor
        /// </summary>
        /// <param name="processor">The processor this task runs on</param>
        /// <param name="graph_01">The initial first graph this task is started with</param>
        /// <param name="graph_02">The initial second graph this task is started with</param>
        /// <returns>The task</returns>
        internal static Task Run(MergeProcessor processor, Graph graph_01, Graph graph_02)
        {
            bool anotherRound = true;
            Graph nextGraph_01 = graph_01;
            Graph nextGraph_02 = graph_02;

            return Task.Run(() =>
            {
                while (anotherRound || processor.state.Equals(MergeProcessorState.AWAITING_TASKS))
                {
                    if (anotherRound)
                    {
                        nextGraph_01.Merge(nextGraph_02);

                        lock (processor.enqueueSync)
                        {
                            processor.queue.Enqueue(nextGraph_01);
                        }
                    }

                    lock (processor.enqueueSync)
                    {
                        if (processor.queue.Count >= 2)
                        {
                            nextGraph_01 = processor.queue.Dequeue();
                            nextGraph_02 = processor.queue.Dequeue();
                            anotherRound = true;
                        }
                        else
                            anotherRound = false;
                    }
                }
            }); 
        }
    }
}
