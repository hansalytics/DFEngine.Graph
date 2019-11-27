using System;
using System.Collections.Generic;

namespace DFEngine.Core.Models
{
    public class GraphBuilderResult
    {
        public Graph Graph { get; private set; }
        public List<string> Errors { get; set; }
        public DateTime Timestamp { get; set; }

        public GraphBuilderResult()
        {
            Errors = new List<string>();
            Timestamp = DateTime.UtcNow;
        }

        public void SetGraph(Graph graph)
        {
            if (!graph.Final)
                throw new InvalidOperationException("Graph must be final before adding it to the result object");

            Graph = graph;
        }
    }
}