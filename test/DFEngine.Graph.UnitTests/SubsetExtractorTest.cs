using DFEngine.Graph.Helpers;
using DFEngine.Graph.Models;
using System.Collections.Generic;
using Xunit;

namespace DFEngine.Graph.UnitTests
{
    public class SubsetExtractorTest
    {
        readonly Models.Graph graph;

        public SubsetExtractorTest()
        {
            var mockProvider = new MockProvider();

            graph = mockProvider.Graph_01;
            mockProvider.Graph_02.SetImmutable();
            graph.Merge(mockProvider.Graph_02);
            graph.SetImmutable();
        }

        [Fact]
        public void ShouldExtractSubset()
        {
            var filter = new SubsetExtractor();
            var filters = new List<string>()
            {
                "report_01",
                "report_02",
                "non_existing",
                "table_01"
            }.AsReadOnly();

            Models.Graph subset = filter.Extract(graph, filters, 1);

            bool haveNoDuplicateIds = TestHelper.HaveNoDuplicateIds(graph, subset);

            Assert.True(haveNoDuplicateIds);
            Assert.Equal(3, subset.Multitrees.Count);
        }
    }
}
