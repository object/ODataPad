using System.Collections.Generic;
using System.Threading.Tasks;
using ODataPad.Core.Models;

namespace ODataPad.Core.Interfaces
{
    public interface IServiceLocalStorage
    {
        Task<IEnumerable<ServiceInfo>> LoadServiceInfosAsync();
        Task SaveServiceInfosAsync(IEnumerable<ServiceInfo> serviceInfos);
        Task<string> LoadServiceMetadataAsync(string filename);
        Task SaveServiceMetadataAsync(string filename, string metadata);
        Task ClearServicesAsync();
    }
}