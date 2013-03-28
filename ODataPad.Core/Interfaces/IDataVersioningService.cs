using System.Threading.Tasks;

namespace ODataPad.Core.Interfaces
{
    public interface IDataVersioningService
    {
        Task SetDataVersionAsync(int currentVersion, int requestedVersion);
    }
}