using DFEngine.Graph.Models;
using Xunit;

namespace DFEngine.Graph.UnitTests
{
    public class GraphTest
    {
        readonly MockProvider mockProvider;

        public GraphTest()
        {
            mockProvider = new MockProvider();
        }

        [Fact]
        public void ShouldMergeGraphs()
        {
            Models.Graph graph_01 = mockProvider.Graph_01;
            Models.Graph graph_02 = mockProvider.Graph_02;

            graph_01.Merge(graph_02);
            graph_01.SetImmutable();

            graph_01.Multitrees.TryGetValue(mockProvider.ReportService_01.GetHashCode(), out Node reportService);
            graph_01.Multitrees.TryGetValue(mockProvider.Server_01.GetHashCode(), out Node server);
            server.ChildNodes.TryGetValue(mockProvider.Database_04.GetHashCode(), out Node db);

            Assert.Equal(3, graph_01.Multitrees.Count);
            Assert.Equal(1, reportService.DimensionFactor);
            Assert.Equal(1, db.IndegreeBoxed);
            Assert.Equal(0, db.OutdegreeBoxed);
            Assert.Equal(6, TestHelper.CountEdges(graph_01.AdjacencyList));
        }

        [Fact]
        public void ShouldAddEdgeAfterNodeTransform()
        {
            Models.Graph graph = new Models.Graph();

            var targetServer = new Node("server", "Sql Server");
            var targetDatabase = new Node("database", "Database");
            var target = new Node("table", "Table");

            targetDatabase.AddChild(target);
            targetServer.AddChild(targetDatabase);
            graph.AddNode(targetServer);

            var sourceServer = new Node("server", "Sql Server");
            var sourceDatabase = new Node("database", "Database");
            var source = new Node("table", "Table");

            sourceDatabase.AddChild(source);
            sourceServer.AddChild(sourceDatabase);

            graph.AddNode(sourceServer);
            graph.AddEdge(new Edge(source, target, "someLabel"));

            Assert.Single(graph.Multitrees);
        }

        [Fact]
        public void ShouldAddUnuniqueNodes()
        {
            Models.Graph graph = new Models.Graph();

            var select_01 = new Node("SELECT", "CTE", false);

            graph.AddNode(select_01);

            var select_02 = new Node("SELECT", "CTE", false);

            graph.AddNode(select_02);

            graph.AddEdge(new Edge(select_01, select_02, "someLabel"));

             Assert.Equal(2, graph.Multitrees.Count);
        }
    }
}
