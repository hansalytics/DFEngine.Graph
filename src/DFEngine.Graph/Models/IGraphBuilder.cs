using System.Threading.Tasks;

namespace DFEngine.Graph.Models
{
    interface IGraphBuilder
    {
        Task<GraphBuilderResult> Build();
    }
}
