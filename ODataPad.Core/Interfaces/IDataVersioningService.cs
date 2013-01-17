using System.Threading.Tasks;

namespace ODataPad.Core.Interfaces
{
    public interface IDataVersioningService
    {
        Task<bool> SetDataVersionAsync(int currentVersion, int requestedVersion);
    }
}