using System.Collections.Generic;
using System.Threading.Tasks;
using ODataPad.Core.Models;

namespace ODataPad.Core.Interfaces
{
    public interface IServiceRepository
    {
        IList<ServiceInfo> Services { get; }

        Task<bool> AddServiceAsync(ServiceInfo serviceInfo);
        Task<bool> UpdateServiceAsync(string serviceName, ServiceInfo serviceInfo);
        Task<bool> DeleteServiceAsync(ServiceInfo serviceInfo);

        Task<IEnumerable<ServiceInfo>> LoadServicesAsync();
        Task<bool> SaveServicesAsync();
        Task<bool> ClearServicesAsync();
    }
}
