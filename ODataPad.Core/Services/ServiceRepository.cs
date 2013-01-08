using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ODataPad.Core.Models;

namespace ODataPad.Core.Services
{
    public class ServiceRepository
    {
        private IServiceLocalStorage _localStorage;

        public IList<ServiceInfo> Services { get; set; }

        public ServiceRepository(IServiceLocalStorage localStorage)
        {
            _localStorage = localStorage;
        }

        public async Task<bool> AddServiceAsync(ServiceInfo serviceInfo)
        {
            serviceInfo.Index = this.Services.Count;
            this.Services.Add(serviceInfo);
            return await SaveServicesAsync();
        }

        public async Task<bool> UpdateServiceAsync(string serviceName, ServiceInfo serviceInfo)
        {
            var originalService = this.Services.Single(x => x.Name == serviceName);
            originalService.Name = serviceInfo.Name;
            originalService.Url = serviceInfo.Url;
            originalService.Description = serviceInfo.Description;
            originalService.Logo = serviceInfo.Logo;
            originalService.MetadataCache = serviceInfo.MetadataCache;
            originalService.CacheUpdated = serviceInfo.CacheUpdated;
            return await SaveServicesAsync();
        }

        public async Task<bool> DeleteServiceAsync(ServiceInfo serviceInfo)
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
            return await SaveServicesAsync();
        }

        public async Task<IEnumerable<ServiceInfo>> LoadServicesAsync()
        {
            var servicesWithMetadata = new List<ServiceInfo>();
            var serviceInfos = await _localStorage.LoadServiceInfosAsync();
            foreach (var serviceInfo in serviceInfos)
            {
                var serviceInfoWithMetadata = serviceInfo;
                serviceInfoWithMetadata.MetadataCache = await _localStorage.LoadServiceMetadataAsync(serviceInfo.MetadataCacheFilename);
                servicesWithMetadata.Add(serviceInfoWithMetadata);
            }

            this.Services = servicesWithMetadata.OrderBy(x => x.Index).Select(x => x).ToList();
            return this.Services;
        }

        public async Task<bool> SaveServicesAsync()
        {
            await _localStorage.SaveServiceInfosAsync(this.Services);
            foreach (var serviceInfo in this.Services)
            {
                await _localStorage.SaveServiceMetadataAsync(serviceInfo.MetadataCacheFilename, serviceInfo.MetadataCache);
            }
            return true;
        }

        public async Task<bool> ClearServicesAsync()
        {
            await _localStorage.ClearServicesAsync();
            this.Services = new ServiceInfo[] { };
            return true;
        }
    }
}