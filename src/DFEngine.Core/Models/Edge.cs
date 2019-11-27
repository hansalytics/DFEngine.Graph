using System;
using System.Collections.Generic;
using System.Linq;

namespace DFEngine.Core.Models
{
    /// <summary>
    /// Represents the directed connection between two nodes.
    /// caution: very edgy!
    /// </summary>
    public class Edge
    {
        /// <summary>
        /// [DO NOT USE] This property exsists for Json serialization only!
        /// </summary>
        public Guid SourceId { get; set; }
        /// <summary>
        /// [DO NOT USE] This property exsists for Json serialization only!
        /// </summary>
        public Guid TargetId { get; set; }

        internal Node Source { get; }
        internal Node Target { get; }
        internal int Occurences { get; private set; }
        public List<string> Labels { get; set; }

        public Edge()
        {
            Labels = new List<string>();
        }

        public Edge(Node source, Node target, string label)
        {
            SourceId = source.Id;
            TargetId = target.Id;
            Source = source;
            Target = target;
            Occurences = 1;
            Labels = new List<string>()
            {
                label
            };
        }

        public Edge(Node source, Node target, List<string> labels)
        {
            SourceId = source.Id;
            TargetId = target.Id;
            Source = source;
            Target = target;
            Occurences = labels.Count;
            Labels = labels;
        }

        internal void Merge(Edge newEdge)
        {
            Labels = Labels.Union(newEdge.Labels).ToList();
            Occurences += newEdge.Occurences;
        }

        /// <summary>
        /// Two edges are considered equal if their source and their target have both same names and types
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj is Edge edgeToCompare)
                return Source.Equals(edgeToCompare.Source) && Target.Equals(edgeToCompare.Target);
            else
                return false;
        }

        public override int GetHashCode() => Tuple.Create(Source.GetHashCode(), Target.GetHashCode()).GetHashCode();

        public override string ToString() => "From " + Source.Name + " to " + Target.Name;
    }
}
