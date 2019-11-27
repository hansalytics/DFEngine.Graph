using DFEngine.Core.Models;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DFEngine.Core.UnitTests")]

namespace DFEngine.Core.Helpers
{
    static class Algorithms
    {
        /// <summary>
        /// Reverses an adjacency list, so e.g. a list that is sorted by the nodes
        /// the edges are starting from can be transformed to a list that is sorted
        /// by the nodes that are beeing targeted
        /// </summary>
        internal static AdjacencyList ReverseAdjacencyList(AdjacencyList adjacencyList)
        {
            AdjacencyList reversed = new AdjacencyList(!adjacencyList.Reversed);

            foreach (var group in adjacencyList.Groups)
            {
                foreach (var edge in group.Value.Edges)
                {
                    bool exists;
                    AdjacentGroup existingGroup;

                    if (reversed.Reversed)
                        exists = reversed.Groups.TryGetValue(edge.Value.Target.Id, out existingGroup);
                    else
                        exists = reversed.Groups.TryGetValue(edge.Value.Source.Id, out existingGroup);

                    if (exists)
                        existingGroup.AddEdge(edge.Value);
                    else
                    {
                        var newGroup = new AdjacentGroup(reversed.Reversed);
                        newGroup.AddEdge(edge.Value);
                        if (reversed.Reversed)
                            reversed.Groups.Add(edge.Value.Target.Id, newGroup);
                        else
                            reversed.Groups.Add(edge.Value.Source.Id, newGroup);
                    }
                }
            }

            return reversed;
        }

        /// <summary>
        /// Gets all parental nodes of a given node + the node itself by traveling upwards a multitree 
        /// and also gets all children that are dependend of a given node
        /// </summary>
        /// <param name="multiTrees">All possible multitrees we expect the specified node in</param>
        /// <param name="node">The node we want to extract its subtree from</param>
        /// <param name="directReference">A direct reference to the cloned object of the specified node</param>
        /// <returns>A subset or null if the node does not exists</returns>
        /// <remarks>A visual example is given in the related testproject</remarks>
        internal static Node GetClonedSubTree(Dictionary<int, Node> multiTrees, Node node, out Node directReference)
        {
            if (multiTrees.TryGetValue(node.GetHashCode(), out Node match) && ReferenceEquals(node, match))
            {
                var deepCopy = match.DeepCopy();
                directReference = deepCopy;
                return deepCopy;
            }
            else
            {
                foreach (var dataNode in multiTrees)
                {
                    var childClone = GetClonedSubTree(dataNode.Value.ChildNodes, node, out directReference);
                    if (childClone != null)
                    {
                        Node parentClone = dataNode.Value.CreateChildlessClone();
                        parentClone.AddChild(childClone);

                        return parentClone;
                    }
                }
            }

            directReference = null;
            return null;
        }

        /// <summary>
        /// Enumerates all children of a given node + the node itself. The order of the collection is not
        /// specified.
        /// </summary>
        /// <param name="collection">The list we want all enumerated nodes to be written to</param>
        internal static void EnumerateChildNodes(Node node, List<Node> collection)
        {
            collection.Add(node);

            foreach (var child in node.ChildNodes)
                EnumerateChildNodes(child.Value, collection);


        }

        /// <summary>
        /// Checks if one or both of the references of an edge have been replaced and replaces
        /// them if needed
        /// </summary>
        internal static Edge ResolvePossibleReplacements(Edge edge)
        {
            Node finalSource = ResolveReferenceChain(edge.Source, 0);
            Node finalTarget = ResolveReferenceChain(edge.Target, 0);

            return new Edge(finalSource, finalTarget, edge.Labels);
        }

        /// <summary>
        /// Computes and assigns a dimensionfactor to all nodes which appear in an adjacencylist
        /// The dimensionfactor is a float value between 0 and 1 and states 
        /// the indegree/outdegree ratio a node has in its dimensionlayer. This value can be used
        /// by frontend applications to easily position the objects
        /// </summary>
        /// <remarks>A visual example is given in the related testproject</remarks>
        internal static void ComputeDimensionFactors(AdjacencyList adjacencyList)
        {
            foreach (var group in adjacencyList.Groups)
            {
                foreach (var edge in group.Value.Edges)
                {
                    Queue<Node> sourceStack = new Queue<Node>();
                    Queue<Node> targetStack = new Queue<Node>();
                    GetUpperTreePart(sourceStack, edge.Value.Source);
                    GetUpperTreePart(targetStack, edge.Value.Target);

                    while (sourceStack.Count > targetStack.Count)
                        sourceStack.Dequeue();
                    while (targetStack.Count > sourceStack.Count)
                        targetStack.Dequeue();

                    while (true)
                    {
                        Node currentSource = sourceStack.Dequeue();
                        Node currentTarget = targetStack.Dequeue();

                        if (sourceStack.Count == 0 || sourceStack.Peek().Id.Equals(targetStack.Peek().Id))
                        {
                            currentSource.IsSource();
                            currentTarget.IsTarget();
                            break;
                        }
                    }
                }
            }

        }

        private static void GetUpperTreePart(Queue<Node> queue, Node leaf)
        {
            queue.Enqueue(leaf);
            if (leaf.Parent != null)
                GetUpperTreePart(queue, leaf.Parent);
        }

        private static Node ResolveReferenceChain(Node origin, int chainLength)
        {
            if (origin.ReplacedBy == null)
                return origin;
            else
            {
                var finalNode = ResolveReferenceChain(origin.ReplacedBy, chainLength + 1);
                return finalNode;
            }
        }
    }
}
