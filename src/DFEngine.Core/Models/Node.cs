using System;
using System.Collections.Generic;
using System.Linq;

namespace DFEngine.Core.Models
{
    /// <summary>
    /// In this class a node may also contain child nodes, so every node which has childs
    /// can also be seen as a DAG (Directed Acyclic Graph) or tree
    /// </summary>
    public class Node
    {
        /// <summary>
        /// Since nodes are stored in a hashtable, this value ensures that the chance of collision stays below 0.01 percent
        /// </summary>
        const int MAX_AMOUNT_CHILD_NODES = 2932;

        int referenced = 0;

        /// <summary>
        /// This is a reference to the node, this node got merged with or null if
        /// was not merged
        /// </summary>
        internal Node ReplacedBy { get; set; }
        internal Node ClonedFrom { get; set; }
        internal Node Parent { get; set; }
        internal int IndegreeBoxed { get; set; }
        internal int OutdegreeBoxed { get; set; }
        public Guid Id { get; set; }
        public string Name { get; private set; }
        public string NodeClass { get; private set; }

        /// <summary>
        /// If the node is declared as unique it may not have any neighbours with the same 
        /// name and class and will contain a self created hashvalue
        /// </summary>
        public bool Unique { get; private set; }

        public int CustomHashcode { get; private set; }

        public Dictionary<int, Node> ChildNodes { get; private set; }
        public List<string> Appearances { get; set; }
        public bool Immutable { get; private set; }
        public bool Valid { get; private set; }
        public float DimensionFactor { get; set; }

        public Node(string name, string nodeClass, bool unique = true)
        {
            ReplacedBy = null;
            Parent = null;
            Id = Guid.NewGuid();
            Name = name;
            NodeClass = nodeClass.ToLower();
            Unique = unique;
            ChildNodes = new Dictionary<int, Node>();
            Appearances = new List<string>();
            Immutable = false;
            Valid = true;
            DimensionFactor = 0;
            if (!Unique)
                CustomHashcode = new Random().Next(int.MinValue, int.MaxValue);
            else
                CustomHashcode = 0;
        }

        public void AddChild(Node child)
        {
            if (!Valid)
                throw new InvalidOperationException("Node is declared invalid. This happens when the graph, this node exists in, has already been merged");

            if (Immutable)
                throw new InvalidOperationException("Node can not be modified after set to immutable");

            if (ChildNodes.Count == MAX_AMOUNT_CHILD_NODES)
                throw new InvalidOperationException($"Each Node can only store up to {MAX_AMOUNT_CHILD_NODES} child nodes");

            ChildNodes.Add(child.GetHashCode(), child);
            child.Parent = this;
        }

        public void AddAppearance(string appearance)
        {
            if (!Valid)
                throw new InvalidOperationException("Node is declared invalid. This happens when the graph, this node exists in, has already been merged");

            if (Immutable)
                throw new InvalidOperationException("Node can not be modified after set to immutable");

            if (!Appearances.Contains(appearance.ToLower()))
                Appearances.Add(appearance.ToLower());
        }

        public override string ToString() => Name;

        /// <summary>
        /// Two nodes are considered equal if their names and types are equal
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj is Node nodeToCompare)
                return Name.Equals(nodeToCompare.Name, StringComparison.InvariantCultureIgnoreCase) && NodeClass.Equals(nodeToCompare.NodeClass, StringComparison.InvariantCultureIgnoreCase);
            else
                return false;
        }

        public override int GetHashCode()
        {
            if (Unique)
                return Tuple.Create(Name.ToLower(), NodeClass.ToLower()).GetHashCode();
            else
                return CustomHashcode;
        }

        /// <summary>
        /// Creates a clone, which also contains only cloned children. The result
        /// also contains a list of all original Ids from the cloned nodes
        /// </summary>
        internal Node DeepCopy()
        {
            if (!Valid)
                throw new InvalidOperationException("Node is declared invalid");

            Node clone = new Node(Name, NodeClass);
            clone.ClonedFrom = this;

            foreach (string appearance in Appearances)
                clone.AddAppearance(appearance);

            foreach (var child in ChildNodes)
            {
                var childClone = child.Value.DeepCopy();
                clone.AddChild(childClone);
            }

            return clone;
        }

        internal Node CreateReplacement()
        {
            if (!Valid)
                throw new InvalidOperationException("Node is declared invalid");

            Node replacement = new Node(Name, NodeClass);

            foreach (string appearance in Appearances)
                replacement.AddAppearance(appearance);

            foreach (var child in ChildNodes)
            {
                var childClone = child.Value.CreateReplacement();
                replacement.AddChild(childClone);
            }

            if (replacement == null)
                throw new InvalidOperationException();

            ReplacedBy = replacement;
            Valid = false;
            return replacement;
        }

        internal Node CreateChildlessClone()
        {
            if (!Valid)
                throw new InvalidOperationException("Node is declared invalid");

            Node clone = new Node(Name, NodeClass);
            clone.ClonedFrom = this;
            return clone;
        }

        /// <summary>
        /// Merges two nodes recursivly by also merging all child nodes
        /// </summary>
        /// <param name="sameNode">The node to be intaken</param>
        /// <param name="replacements">If the node already exist, the id of the sameNode gets added
        /// to the replacement list together with the already existing node. Can be null if not needed</param>
        internal void Merge(Node sameNode)
        {
            if (!Valid || !sameNode.Valid)
                throw new InvalidOperationException("Node is declared invalid");

            Appearances = Appearances.Union(sameNode.Appearances).ToList();

            foreach (var nodeToAdd in sameNode.ChildNodes)
            {
                if (ChildNodes.TryGetValue(nodeToAdd.Key, out Node existingNode))
                    existingNode.Merge(nodeToAdd.Value);
                else
                {
                    var replacement = nodeToAdd.Value.CreateReplacement();
                    AddChild(replacement);
                }
            }

            sameNode.ReplacedBy = this;
            sameNode.Invalidate();
        }

        internal void SetImmutable()
        {
            if (!Valid)
                throw new InvalidOperationException("Node is declared invalid");

            foreach (var child in ChildNodes)
                child.Value.SetImmutable();

            Immutable = true;
        }

        internal void Invalidate()
        {
            if (ReplacedBy == null)
                throw new InvalidOperationException();
            Valid = false;

            foreach (var child in ChildNodes)
                child.Value.Invalidate();
        }

        /// <summary>
        /// Is called on Dimension factor computation
        /// </summary>
        internal void IsSource()
        {
            referenced++;
            OutdegreeBoxed++;
            DimensionFactor = (float)IndegreeBoxed / referenced;
        }

        /// <summary>
        /// Is called on Dimension factor computation
        /// </summary>
        internal void IsTarget()
        {
            referenced++;
            IndegreeBoxed++;
            DimensionFactor = (float)IndegreeBoxed / referenced;
        }
    }
}
