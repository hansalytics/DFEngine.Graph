using DFEngine.Graph.Models;

namespace DFEngine.Graph.UnitTests
{
    static class TestHelper
    {
        public static int CountEdges(AdjacencyList adjacencyList)
        {
            int amount = 0;

            foreach(var entry in adjacencyList.Groups)
            {
                foreach (var edge in entry.Value.Edges)
                    amount++;
            }

            return amount;
        }

        /// <summary>
        /// Ensures that not a single id of one graph appears in another
        /// </summary>
        /// <param name="graph_01">The first graph</param>
        /// <param name="graph_02">The second graph</param>
        /// <returns>The state if the two graphs have completly seperate ids</returns>
        public static bool HaveNoDuplicateIds(Models.Graph graph_01, Models.Graph graph_02)
        {
            foreach (var node_01 in graph_01.Multitrees)
            {
                foreach (var node_02 in graph_02.Multitrees)
                {
                    bool haveNoDuplicateIds = HaveNoDuplicateIds(node_01.Value, node_02.Value);
                    if (!haveNoDuplicateIds)
                        return false;
                }
            }

            return true;
        }

        private static bool HaveNoDuplicateIds(Node node_01, Node node_02)
        {
            if (node_01.Id.Equals(node_02.Id))
                return false;

            bool childMatch;

            foreach (var child in node_02.ChildNodes)
            {
                childMatch = HaveNoDuplicateIds(node_01, child.Value);
                if (!childMatch)
                    return false;
            }

            foreach (var child_01 in node_01.ChildNodes)
            {
                foreach (var child_02 in node_02.ChildNodes)
                {
                    childMatch = HaveNoDuplicateIds(child_01.Value, child_02.Value);
                    if (!childMatch)
                        return false;
                }
            }

            return true;
        }
    }
}
