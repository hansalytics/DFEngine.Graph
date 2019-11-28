using DFEngine.Graph.Models;
using System;
using System.Collections.Generic;

namespace DFEngine.Graph.Helpers
{
    static class Validators
    {
        /// <summary>
        /// Checks if both references of an edge are contained in a given graph
        /// </summary>
        internal static bool IsValidEdge(Edge edge, Models.Graph graph)
        {
            if (!edge.Source.Valid || !edge.Target.Valid)
                return false;

            bool sourceExists = false;
            bool targetExists = false;

            IsValidEdge(graph.Multitrees, ref sourceExists, ref targetExists, edge);

            return sourceExists && targetExists;
        }

        internal static bool IsValidAdjacencyList(AdjacencyList adList, Models.Graph graph)
        {
            foreach (var group in adList.Groups)
            {
                if (!NodeExist(group.Key, graph.Multitrees))
                    return false;

                foreach (var entry in group.Value.Edges)
                {
                    if (!NodeExist(entry.Key, graph.Multitrees))
                        return false;
                }
            }

            return true;
        }

        private static bool NodeExist(Guid key, Dictionary<int, Node> nodes)
        {
            foreach (var node in nodes)
            {
                if (node.Value.Id.Equals(key))
                    return true;

                if (NodeExist(key, node.Value.ChildNodes))
                    return true;
            }

            return false;
        }

        private static void IsValidEdge(Dictionary<int, Node> nodes, ref bool sourceExists, ref bool targetExists, Edge checkMe)
        {
            foreach (var node in nodes)
            {
                if (ReferenceEquals(node.Value, checkMe.Source))
                    sourceExists = true;
                if (ReferenceEquals(node.Value, checkMe.Target))
                    targetExists = true;

                if (sourceExists && targetExists)
                    return;
                else
                {
                    IsValidEdge(node.Value.ChildNodes, ref sourceExists, ref targetExists, checkMe);
                    if (sourceExists && targetExists)
                        return;
                }
            }
        }

    }
}
