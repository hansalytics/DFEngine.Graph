using DFEngine.Core.Models;
using System;
using System.Collections.Generic;

namespace DFEngine.Core.Helpers
{
    public class SubsetExtractor
    {
        const int MAX_TRAVEL_LIMIT = 10;

        Graph originalGraph;
        Graph subset;

        int _travelLimit;

        /// <summary>
        /// Returns a subset of a graph by extracting all nodes which names match one of the provided filters.
        /// These nodes are called direct matches.
        /// From each direct match the method walks along the graph and returns each
        /// node visited to the subset until the max
        /// </summary>
        /// <param name="origin">The graph we want to extract the subset from</param>
        /// <param name="filters">A list of filters which are used to find nodes by names</param>
        /// <param name="travelLimit">The distance the method will travel along from each direct match</param>
        /// <returns>The subset</returns>
        public Graph Extract(Graph origin, IReadOnlyCollection<string> filters, int travelLimit)
        {
            if (travelLimit < 1 || travelLimit > MAX_TRAVEL_LIMIT)
                throw new ArgumentException($"The walking distance must be between 0 and {MAX_TRAVEL_LIMIT + 1}");

            if (!origin.Final)
                throw new InvalidOperationException("This method can only be performed on a graph that is set to immutable");

            originalGraph = origin;
            subset = new Graph();

            _travelLimit = travelLimit;

            foreach (var node in originalGraph.Multitrees)
                AddNodeIfMatch(node.Value, filters);

            subset.SetImmutable();
            return subset;
        }

        private void AddNodeIfMatch(Node node, IReadOnlyCollection<string> filters)
        {
            foreach (string filter in filters)
            {
                if (node.Name.ToLower().Contains(filter.ToLower()))
                {
                    Node subTree = Algorithms.GetClonedSubTree(originalGraph.Multitrees, node, out Node directReference);
                    subset.AddNode(subTree);
                    List<Node> enumeratedSubTree = new List<Node>();
                    Algorithms.EnumerateChildNodes(subTree, enumeratedSubTree);
                    foreach (var subTreePart in enumeratedSubTree)
                        AddAdjacentNodes(subTreePart, 0, 0);
                    return;
                }
            }

            foreach (var child in node.ChildNodes)
                AddNodeIfMatch(child.Value, filters);
        }

        /// <summary>
        /// Adds the all nodes that are adjacent to a given node + all related edges and does that recursivly for all new nodes
        /// added this way until the max walking distance is reached
        /// </summary>
        /// <param name="origin">The cloned node we want to know the adjacent nodes from. This node already exist in the subset at this point</param>
        /// <param name="travelDirection">The direction the method should search for adjacent neighbours: -1 = backwards; 0 = both ways; 1 = forward</param>
        /// <param name="walkedDistance">The current depth of recursion</param>
        private void AddAdjacentNodes(Node origin, short travelDirection, int traveledDistance)
        {
            if (traveledDistance >= _travelLimit)
                return;

            switch (travelDirection)
            {
                case -1:
                    {
                        if (originalGraph.ReversedAdjacencyList.Groups.TryGetValue(origin.ClonedFrom.Id, out AdjacentGroup incomingEdges))
                            TravelBackward(origin, incomingEdges.Edges.Values, traveledDistance);

                        break;
                    }
                case 0:
                    {
                        if (originalGraph.ReversedAdjacencyList.Groups.TryGetValue(origin.ClonedFrom.Id, out AdjacentGroup incomingEdges))
                            TravelBackward(origin, incomingEdges.Edges.Values, traveledDistance);

                        if (originalGraph.AdjacencyList.Groups.TryGetValue(origin.ClonedFrom.Id, out AdjacentGroup outgoingEdges))
                            TravelForward(origin, outgoingEdges.Edges.Values, traveledDistance);

                        break;
                    }
                case 1:
                    {
                        if (originalGraph.AdjacencyList.Groups.TryGetValue(origin.ClonedFrom.Id, out AdjacentGroup outgoingEdges))
                            TravelForward(origin, outgoingEdges.Edges.Values, traveledDistance);

                        break;
                    }
            }
        }

        private void TravelBackward(Node origin, IEnumerable<Edge> edges, int traveledDistance)
        {
            foreach (var edge in edges)
            {
                Node sourceSubTree = Algorithms.GetClonedSubTree(originalGraph.Multitrees, edge.Source, out Node directReference);
                subset.AddNode(sourceSubTree);
                subset.AddEdge(new Edge(directReference, origin, edge.Labels));
                List<Node> enumeratedSubTree = new List<Node>();
                Algorithms.EnumerateChildNodes(sourceSubTree, enumeratedSubTree);
                foreach (var subTreePart in enumeratedSubTree)
                    AddAdjacentNodes(subTreePart, -1, traveledDistance + 1);
            }
        }

        private void TravelForward(Node origin, IEnumerable<Edge> edges, int traveledDistance)
        {
            foreach (var edge in edges)
            {
                Node targetSubTree = Algorithms.GetClonedSubTree(originalGraph.Multitrees, edge.Target, out Node directReference);
                subset.AddNode(targetSubTree);
                subset.AddEdge(new Edge(origin, directReference, edge.Labels));
                List<Node> enumeratedSubTree = new List<Node>();
                Algorithms.EnumerateChildNodes(targetSubTree, enumeratedSubTree);
                foreach (var subTreePart in enumeratedSubTree)
                    AddAdjacentNodes(subTreePart, 1, traveledDistance + 1);
            }
        }
    }
}
