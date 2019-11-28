using System;
using System.Collections.Generic;
using System.Text;

namespace DFEngine.Graph.Models
{
    public class AdjacentGroup
    {
        /// <summary>
        /// States if the adjacency list, this group exist in, is grouped by outgoing (false) or incoming edges (true)
        /// </summary>
        private readonly bool reversed;
        public  Dictionary<Guid, Edge> Edges { get; }

        /// <summary>
        /// [DEPRECIATED] this constructor exists for Json conversion
        /// </summary>
        public AdjacentGroup()
        {
            Edges = new Dictionary<Guid, Edge>();
        }

        internal AdjacentGroup(bool groupage)
        {
            reversed = groupage;
            Edges = new Dictionary<Guid, Edge>();
        }

        internal void AddEdge(Edge newEdge)
        {
            if (reversed)
            {
                bool entryExists = Edges.TryGetValue(newEdge.Source.Id, out Edge existingEdge);

                if (!entryExists)
                    Edges.Add(newEdge.Source.Id, newEdge);
                else
                    existingEdge.Merge(existingEdge);
            }
            else
            {
                bool entryExists = Edges.TryGetValue(newEdge.Target.Id, out Edge existingEdge);

                if (!entryExists)
                    Edges.Add(newEdge.Target.Id, newEdge);
                else
                    existingEdge.Merge(existingEdge);
            }
        }
    }
}
