using DFEngine.Core.Models;
using DFEngine.Core.Models.MergeProcessor;
using Xunit;

namespace DFEngine.Core.UnitTests
{
    public class MergeProcessorTest
    {
        readonly MockProvider mockProvider;

        public MergeProcessorTest()
        {
            mockProvider = new MockProvider();
        }

        [Fact]
        public void ShouldMergeLandscapesWithMergeProcessor()
        {
            Graph graph_01 = mockProvider.Graph_01;
            Graph graph_02 = mockProvider.Graph_02;
            graph_02.SetImmutable();

            MergeProcessor processor = new MergeProcessor();

            processor.Queue(graph_01);
            processor.Queue(graph_02);

            Graph result = processor.GetFinalResult().Result;

            Assert.Equal(3, result.Multitrees.Count);
            Assert.Equal(6, TestHelper.CountEdges(result.AdjacencyList));
        }

        [Fact]
        public void ShouldGetResultOfSingleGraph()
        {
            Graph graph_01 = mockProvider.Graph_01;

            MergeProcessor processor = new MergeProcessor();

            processor.Queue(graph_01);

            Graph result = processor.GetFinalResult().Result;

            Assert.Equal(2, result.Multitrees.Count);
        }
    }
}
