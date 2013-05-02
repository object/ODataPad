using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cirrious.CrossCore;
using ODataPad.Core.Interfaces;
using ODataPad.Core.Models;

namespace ODataPad.Core.Services
{
    public class ServiceRepository
        : IServiceRepository
    {
        private readonly IServiceLocalStorage _localStorage;

        public ServiceRepository(IServiceLocalStorage localStorage = null)
        {
            _localStorage = localStorage ?? Mvx.Resolve<IServiceLocalStorage>();
        }

        public IList<ServiceInfo> Services { get; private set; }

        public async Task AddServiceAsync(ServiceInfo serviceInfo)
        {
            serviceInfo.Index = this.Services.Count;
            this.Services.Add(serviceInfo);
            await SaveServicesAsync();
        }

        public async Task UpdateServiceAsync(string serviceName, ServiceInfo serviceInfo)
        {
            var originalService = this.Services.Single(x => x.Name == serviceName);
            originalService.Name = serviceInfo.Name;
            originalService.Url = serviceInfo.Url;
            originalService.Description = serviceInfo.Description;
            originalService.Logo = serviceInfo.Logo;
            originalService.MetadataCache = serviceInfo.MetadataCache;
            originalService.CacheUpdated = serviceInfo.CacheUpdated;
            await SaveServicesAsync();
        }

        public async Task DeleteServiceAsync(ServiceInfo serviceInfo)
        {
            var originalService = this.Services.SingleOrDefault(x => x.Name == serviceInfo.Name);
            if (originalService != null)
            {
                this.Services.Remove(originalService);
            }
            for (int index = 0; index < this.Services.Count; index++)
            {
                this.Services[index].Index = index;
            }
            await SaveServicesAsync();
        }

        public async Task<IEnumerable<ServiceInfo>> LoadServicesAsync()
        {
            var services = new List<ServiceInfo>();
            var serviceInfos = await _localStorage.LoadServiceInfosAsync();
            foreach (var serviceInfo in serviceInfos)
            {
                var serviceDetails = serviceInfo;
                await _localStorage.LoadServiceDetailsAsync(serviceDetails);
                services.Add(serviceDetails);
            }

            this.Services = services.OrderBy(x => x.Index).Select(x => x).ToList();
            return this.Services;
        }

        public async Task SaveServicesAsync()
        {
            await _localStorage.SaveServiceInfosAsync(this.Services);
            foreach (var serviceInfo in this.Services)
            {
                await _localStorage
                    .SaveServiceDetailsAsync(serviceInfo);
            }
        }

        public async Task ClearServicesAsync()
        {
            await _localStorage.ClearServiceInfosAsync();
            this.Services = new ServiceInfo[] { };
        }
    }
}
