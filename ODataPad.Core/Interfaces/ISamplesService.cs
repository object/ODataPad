using System.Threading.Tasks;

namespace ODataPad.Core.Interfaces
{
    public interface ISamplesService
    {
        Task CreateSamplesAsync();
        Task UpdateSamplesAsync();
    }
}