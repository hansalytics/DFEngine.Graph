using System.Threading.Tasks;

namespace DFEngine.Core.Models
{
    interface IGraphBuilder
    {
        Task<GraphBuilderResult> Build();
    }
}
