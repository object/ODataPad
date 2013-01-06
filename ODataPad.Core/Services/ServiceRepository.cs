using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ODataPad.Core.Models;

namespace ODataPad.Core.Services
{
    public class ServiceRepository
    {
        private IServiceLocalStorage _localStorage;

        public IList<ODataServiceInfo> Services { get; set; }

        public ServiceRepository(IServiceLocalStorage localStorage)
        {
            _localStorage = localStorage;
        }

        public async Task<IEnumerable<ODataServiceInfo>> LoadServicesAsync()
        {
            var servicesWithMetadata = new List<ODataServiceInfo>();
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
            this.Services = new ODataServiceInfo[] { };
            return true;
        }
    }
}