using DFEngine.Core.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace DFEngine.Core.UnitTests
{
    public class JsonTest
    {
        [Fact]
        public void ShouldConvertBackAndForth()
        {
            var _mockProvider = new MockProvider();

            var graph = _mockProvider.Graph_02;
            graph.SetImmutable();

            string serialized = JsonConvert.SerializeObject(graph);
            var deserialized = JsonConvert.DeserializeObject<Graph>(serialized);

            Assert.Equal(_mockProvider.Graph_02.AdjacencyList.Groups.Count, deserialized.AdjacencyList.Groups.Count);
        }
    }
}
