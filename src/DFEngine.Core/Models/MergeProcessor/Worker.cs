using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DFEngine.Core.Models.MergeProcessor
{
    class Worker
    {
        public async Task Run(MergeProcessor processor, Graph graph_01, Graph graph_02)
        {
            await Task.Run(() => graph_01.Merge(graph_02));

            processor.NotifyReady(this, graph_01);
        }
    }
}
