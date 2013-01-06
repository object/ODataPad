using System.Collections.Generic;
using System.Threading.Tasks;
using ODataPad.Core.Models;

namespace ODataPad.Core.Services
{
    public interface IServiceLocalStorage
    {
        Task<IEnumerable<ODataServiceInfo>> LoadServiceInfosAsync();
        Task<bool> SaveServiceInfosAsync(IEnumerable<ODataServiceInfo> serviceInfos);
        Task<string> LoadServiceMetadataAsync(string filename);
        Task<bool> SaveServiceMetadataAsync(string filename, string metadata);
        Task<bool> ClearServicesAsync();
    }
}