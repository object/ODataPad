using System.Collections.Generic;
using System.Threading.Tasks;
using ODataPad.Core.Models;

namespace ODataPad.Core.Interfaces
{
    public interface IServiceLocalStorage
    {
        Task<IEnumerable<ServiceInfo>> LoadServiceInfosAsync();
        Task<bool> SaveServiceInfosAsync(IEnumerable<ServiceInfo> serviceInfos);
        Task<string> LoadServiceMetadataAsync(string filename);
        Task<bool> SaveServiceMetadataAsync(string filename, string metadata);
        Task<bool> ClearServicesAsync();
    }
}