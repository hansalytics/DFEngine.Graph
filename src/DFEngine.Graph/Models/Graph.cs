using DFEngine.Graph.Helpers;
using System;
using System.Collections.Generic;

namespace DFEngine.Graph.Models
{
    /// <summary>
    ///    This library defines a graph with the following specifications:
    /// 1. It is a directed graph but NOT a mixed graph. Means that EVERY edge must have a source and a target
    /// 2. Multiple edges are allowed but exist in one object. If an edge should be added that already exists between
    ///    two nodes, all the attributes are taken over into the existing edge
    /// 3. Loops are allowed
    /// </summary>
    public class Graph
    {
        /// <summary>
        /// Since nodes are stored in a hashtable, this value ensures that the chance of collision stays below 0.01 percent
        /// </summary>
        /// <seealso href="https://en.wikipedia.org/wiki/Birthday_problem"/>
        const int MAX_AMOUNT_NODES = 2932;

        /// <summary>
        /// Once a graph is declared invalid it can not longer be used
        /// </summary>
        internal bool Valid { get; private set; }
        internal bool Final { get; private set; }

        public Dictionary<int, Node> Multitrees { get; private set; }

        public AdjacencyList AdjacencyList { get; internal set; }

        /// <summary>
        /// This list has the target ids as index, so operations that need to know all edges pointing TO a
        /// particular node can easily be extracted by O(1). Reversing tho is time intense which is by this
        /// property is null until a graph becomes immutable
        /// </summary>
        public AdjacencyList ReversedAdjacencyList { get; set; }

        public Graph()
        {
            Final = false;
            Valid = true;
            Multitrees = new Dictionary<int, Node>();
            AdjacencyList = new AdjacencyList(false);
            ReversedAdjacencyList = new AdjacencyList(true);
        }

        /// <summary>
        /// Appends a node to the graph
        /// </summary>
        public void AddNode(Node newObject)
        {
            if (!Valid)
                throw new InvalidOperationException("Graph is declared invalid");

            if (Final)
                throw new InvalidOperationException("Graph can not be modified after set to immutable");

            if (newObject.Parent != null)
                throw new InvalidOperationException("Only root nodes can be added to the graph");

            if (Multitrees.Count == MAX_AMOUNT_NODES)
                throw new InvalidOperationException($"Graph can only store up to {MAX_AMOUNT_NODES} root nodes");

            if (!newObject.Unique)
                Multitrees.Add(newObject.CustomHashcode, newObject);
            else if (Multitrees.TryGetValue(newObject.GetHashCode(), out Node existingNode))
                existingNode.Merge(newObject);
            else
                Multitrees.Add(newObject.GetHashCode(), newObject);
        }

        /// <summary>
        /// Sets the graph to immutable and creates the reversed adjacency list
        /// </summary>
        public void SetImmutable()
        {
            if (!Valid)
                throw new InvalidOperationException("Graph is declared invalid");

            foreach (var tree in Multitrees)
                tree.Value.SetImmutable();

            Final = true;

            Algorithms.ComputeDimensionFactors(AdjacencyList);
            ReversedAdjacencyList = Algorithms.ReverseAdjacencyList(AdjacencyList);
        }

        internal void Invalidate()
        {
            if (!Valid)
                throw new InvalidOperationException("Graph is already declared invalid");

            Valid = false;

            foreach (var tree in Multitrees)
                tree.Value.Invalidate();
        }

        /// <summary>
        /// Appends an edge to the graph. If the nodes, the edge is referencing, are not yet
        /// part of the graph, they will also be appended
        /// </summary>
        public void AddEdge(Edge newEdge)
        {
            if (!Valid)
                throw new InvalidOperationException("Graph is declared invalid");

            if (Final)
                throw new InvalidOperationException("Graph can not be modified after set to immutable");

            Edge edgeToAdd = Algorithms.ResolvePossibleReplacements(newEdge);

            if (!Validators.IsValidEdge(edgeToAdd, this))
                throw new InvalidOperationException("Edge reference is not part of the graph");

            AdjacencyList.AddEdge(edgeToAdd);
        }

        /// <summary>
        /// Merges two graphs together.
        /// The function takes care of detecting duplicates and adjusting the related edges. Note that this method is kept internal
        /// because users of this library shall use the merge processor class for graph merging operations only. The second graph
        /// should not be used after merged into this graph which is why its declared invalid at the end of the method
        /// </summary>
        /// <param name="secondGraph">The graph from which all nodes and edges should be inherited. Note that this graph will become invalid
        /// after this operation</param>
        internal void Merge(Graph secondGraph)
        {
            if (!Valid || !secondGraph.Valid)
                throw new InvalidOperationException("Unable to merge graphs that are declared as invalid");

            if (Final)
                throw new InvalidOperationException("Graph can not be modified after set to immutable");

            foreach (var nodeToAdd in secondGraph.Multitrees)
            {
                if (Multitrees.TryGetValue(nodeToAdd.Key, out Node existingNode))
                    existingNode.Merge(nodeToAdd.Value);
                else
                    AddNode(nodeToAdd.Value.CreateReplacement());
            }

            foreach (var adjacentGroup in secondGraph.AdjacencyList.Groups)
            {
                foreach (var edge in adjacentGroup.Value.Edges)
                    AddEdge(edge.Value);
            }

            secondGraph.Invalidate();
        }
    }
}
