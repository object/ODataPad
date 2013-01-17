using System.Threading.Tasks;

namespace ODataPad.Core.Interfaces
{
    public interface IDataVersioningService
    {
        int CurrentVersion { get; set; }
        int RequestedVersion { get; set; }

        Task<bool> SetVersionAsync();
    }
}