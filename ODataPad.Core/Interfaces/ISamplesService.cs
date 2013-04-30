using System.Threading.Tasks;

namespace ODataPad.Core.Interfaces
{
    public interface ISamplesService
    {
        Task CreateSamplesAsync(int dataVersion);
        Task UpdateSamplesAsync(int fromDataVersion, int toDataVersion);
    }
}