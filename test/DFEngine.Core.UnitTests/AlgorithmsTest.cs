using DFEngine.Core.Models;
using DFEngine.Core.Helpers;
using System.Collections.Generic;
using Xunit;

namespace DFEngine.Core.UnitTests
{
    public class AlgorithmsTest
    {
        [Fact]
        public void ShouldGetClonedSubTree()
        {
            Node root = new Node("root", "file");
            Node root2 = new Node("root2", "file");
            Node layer_01_node_1 = new Node("layer_01_node_1", "file");
            Node layer_01_node_2 = new Node("layer_01_node_2", "file");
            Node layer_02_node_1 = new Node("layer_02_node_1", "file");
            Node layer_02_node_2 = new Node("layer_02_node_2", "file");
            Node layer_03_node_1 = new Node("layer_03_node_1", "file");
            Node layer_03_node_2 = new Node("layer_03_node_2", "file");
            Node layer_04_node_1 = new Node("layer_04_node_1", "file");
            layer_03_node_1.AddChild(layer_04_node_1);
            layer_02_node_1.AddChild(layer_03_node_1);
            layer_02_node_1.AddChild(layer_03_node_2);
            layer_01_node_2.AddChild(layer_02_node_1);
            layer_01_node_2.AddChild(layer_02_node_2);
            root.AddChild(layer_01_node_1);
            root.AddChild(layer_01_node_2);

            Dictionary<int, Node> nodes = new Dictionary<int, Node>
            {
                { root.GetHashCode(), root },
                { root2.GetHashCode(), root2 }
            };

            Node clone = Algorithms.GetClonedSubTree(nodes, layer_02_node_1, out Node directReference);

            Assert.Equal("layer_02_node_1", directReference.Name);
            Assert.Single(clone.ChildNodes);
            Assert.Equal("root", clone.Name);
            Assert.Single(clone.ChildNodes[layer_01_node_2.GetHashCode()].ChildNodes);
            Assert.Equal(2, clone.ChildNodes[layer_01_node_2.GetHashCode()].ChildNodes[layer_02_node_1.GetHashCode()].ChildNodes.Count);
            Assert.Equal("layer_03_node_1", clone.ChildNodes[layer_01_node_2.GetHashCode()].ChildNodes[layer_02_node_1.GetHashCode()].ChildNodes[layer_03_node_1.GetHashCode()].Name);
            Assert.Equal("layer_03_node_2", clone.ChildNodes[layer_01_node_2.GetHashCode()].ChildNodes[layer_02_node_1.GetHashCode()].ChildNodes[layer_03_node_2.GetHashCode()].Name);
            Assert.Empty(clone.ChildNodes[layer_01_node_2.GetHashCode()].ChildNodes[layer_02_node_1.GetHashCode()].ChildNodes[layer_03_node_1.GetHashCode()].ChildNodes[layer_04_node_1.GetHashCode()].ChildNodes);
            Assert.Equal("layer_04_node_1", clone.ChildNodes[layer_01_node_2.GetHashCode()].ChildNodes[layer_02_node_1.GetHashCode()].ChildNodes[layer_03_node_1.GetHashCode()].ChildNodes[layer_04_node_1.GetHashCode()].Name);
        }
    }
}
