using DFEngine.Graph.Models;
using DFEngine.Graph.Models.MergeProcessor;
using Xunit;

namespace DFEngine.Graph.UnitTests
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
            Models.Graph graph_01 = mockProvider.Graph_01;
            Models.Graph graph_02 = mockProvider.Graph_02;

            MergeProcessor processor = new MergeProcessor();

            processor.Queue(graph_01);
            processor.Queue(graph_02);

            Models.Graph result = processor.GetFinalResult().Result;

            Assert.Equal(3, result.Multitrees.Count);
            Assert.Equal(6, TestHelper.CountEdges(result.AdjacencyList));
        }

        [Fact]
        public void ShouldMergeMultipleGraphs()
        {
            Models.Graph graph_01 = mockProvider.Graph_01;
            Models.Graph graph_02 = mockProvider.Graph_02;
            Models.Graph graph_03 = new Models.Graph();

            MergeProcessor processor = new MergeProcessor();

            processor.Queue(graph_01);
            processor.Queue(graph_02);
            processor.Queue(graph_03);

            Models.Graph result = processor.GetFinalResult().Result;
        }

        [Fact]
        public void ShouldGetResultOfSingleGraph()
        {
            Models.Graph graph_01 = mockProvider.Graph_01;

            MergeProcessor processor = new MergeProcessor();

            processor.Queue(graph_01);

            Models.Graph result = processor.GetFinalResult().Result;

            Assert.Equal(2, result.Multitrees.Count);
        }

        [Fact]
        public void ShouldReturnIfQueueIsEmpty()
        { 
            MergeProcessor processor = new MergeProcessor();

            Models.Graph result = processor.GetFinalResult().Result;;
        }
    }
}
