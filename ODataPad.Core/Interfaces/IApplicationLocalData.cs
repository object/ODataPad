using System.Threading.Tasks;

namespace ODataPad.Core.Interfaces
{
    public interface IApplicationLocalData
    {
        int CurrentDataVersion { get; }
        Task SetDataVersionAsync(int requestedDataVersion);
    }
}