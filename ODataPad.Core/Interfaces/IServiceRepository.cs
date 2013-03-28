using System.Collections.Generic;
using System.Threading.Tasks;
using ODataPad.Core.Models;

namespace ODataPad.Core.Interfaces
{
    public interface IServiceRepository
    {
        IList<ServiceInfo> Services { get; }

        Task AddServiceAsync(ServiceInfo serviceInfo);
        Task UpdateServiceAsync(string serviceName, ServiceInfo serviceInfo);
        Task DeleteServiceAsync(ServiceInfo serviceInfo);

        Task<IEnumerable<ServiceInfo>> LoadServicesAsync();
        Task SaveServicesAsync();
        Task ClearServicesAsync();
    }
}
