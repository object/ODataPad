using System.Threading.Tasks;

namespace ODataPad.Core.Interfaces
{
    public interface IApplicationLocalData
    {
        Task SetDataVersionAsync(int requestedDataVersion);
    }
}