using System.Threading.Tasks;

namespace ODataPad.Core.Interfaces
{
    public interface IApplicationLocalData
    {
        Task<bool> SetDataVersionAsync(int requestedDataVersion);
    }
}