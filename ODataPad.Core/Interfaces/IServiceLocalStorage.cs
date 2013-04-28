using System.Collections.Generic;
using System.Threading.Tasks;
using ODataPad.Core.Models;

namespace ODataPad.Core.Interfaces
{
    public interface IServiceLocalStorage
    {
        Task<IEnumerable<ServiceInfo>> LoadServiceInfosAsync();
        Task SaveServiceInfosAsync(IEnumerable<ServiceInfo> serviceInfos);
        Task ClearServiceInfosAsync();
        Task LoadServiceDetailsAsync(ServiceInfo serviceInfo);
        Task SaveServiceDetailsAsync(ServiceInfo serviceInfo);
        Task ClearServiceDetailsAsync(ServiceInfo serviceInfo);
    }
}