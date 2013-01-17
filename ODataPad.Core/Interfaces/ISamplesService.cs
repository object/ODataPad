using System.Threading.Tasks;

namespace ODataPad.Core.Interfaces
{
    public interface ISamplesService
    {
        Task<bool> CreateSamplesAsync();
        Task<bool> UpdateSamplesAsync();
    }
}