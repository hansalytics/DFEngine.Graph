using System;
using System.Collections.Generic;
using System.Text;

namespace DFEngine.Graph.Models
{
    public class AdjacencyList
    {
        internal bool Reversed { get; }

        public Dictionary<Guid, AdjacentGroup> Groups { get; }

        /// <summary>
        /// [DEPRECIATED] this constructor exists for Json conversion
        /// </summary>
        public AdjacencyList()
        {
            Groups = new Dictionary<Guid, AdjacentGroup>();
        }

        internal AdjacencyList(bool reverse)
        {
            Reversed = reverse;
            Groups = new Dictionary<Guid, AdjacentGroup>();
        }

        internal void AddEdge(Edge newEdge)
        {
            if (!Reversed)
            {
                bool entryExists = Groups.TryGetValue(newEdge.Source.Id, out AdjacentGroup existingGroup);

                if (!entryExists)
                {
                    var newGroup = new AdjacentGroup(Reversed);
                    newGroup.AddEdge(newEdge);
                    Groups.Add(newEdge.Source.Id, newGroup);
                }
                else
                    existingGroup.AddEdge(newEdge);
            }
            else
            {
                bool entryExists = Groups.TryGetValue(newEdge.Target.Id, out AdjacentGroup existingGroup);

                if (!entryExists)
                {
                    var newGroup = new AdjacentGroup(Reversed);
                    newGroup.AddEdge(newEdge);
                    Groups.Add(newEdge.Target.Id, newGroup);
                }
                else
                    existingGroup.AddEdge(newEdge);
            }
        }
    }
}
