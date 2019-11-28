using DFEngine.Graph.Helpers;
using DFEngine.Graph.Models;
using Xunit;

namespace DFEngine.Graph.UnitTests
{
    public class NodeTest
    {
        readonly MockProvider mockProvider;

        public NodeTest()
        {
            mockProvider = new MockProvider();
        }

        [Fact]
        public void ShouldAccessChildren()
        {
            Node root = new Node("root", "file");
            Node child1 = new Node("child1", "file");
            Node child2 = new Node("child2", "file");

            root.AddChild(child1);
            root.AddChild(child2);

            root.ChildNodes.TryGetValue(child1.GetHashCode(), out Node result_01);
            root.ChildNodes.TryGetValue(child2.GetHashCode(), out Node result_02);

            Assert.Equal("child1", result_01.Name);
            Assert.Equal("child2", result_02.Name);
        }

        [Fact]
        public void ShouldStateTwoNodesAsEqual_01()
        {
            Node node_01 = mockProvider.File_01;
            Node node_02 = mockProvider.File_02;

            Assert.Equal(node_01, node_02);
        }

        [Fact]
        public void ShouldStateTwoNodesAsEqual_02()
        {
            Node clone_01 = Algorithms.GetClonedSubTree(mockProvider.Graph_01.Multitrees, mockProvider.Table_01, out Node directRef_01);
            Node clone_02 = Algorithms.GetClonedSubTree(mockProvider.Graph_02.Multitrees, mockProvider.Table_04, out Node directRef_02);

            Assert.Equal(clone_01, clone_02);
        }
    }
}
