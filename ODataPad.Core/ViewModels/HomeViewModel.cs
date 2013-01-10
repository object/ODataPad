using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using ODataPad.Core.Models;
using ODataPad.Core.Services;

namespace ODataPad.Core.ViewModels
{
    public class HomeViewModel
    {
        private const int ResultPageSize = 100;
        private ServiceRepository _serviceRepository;
        private IServiceLocalStorage _localStorage;

        public ObservableCollection<ServiceItem> Services { get; private set; }
        public ObservableCollection<ResultRow> QueryResults { get; private set; }

        public HomeViewModel(ServiceRepository serviceRepository, IServiceLocalStorage localStorage)
        {
            _serviceRepository = serviceRepository;
            _localStorage = localStorage;

            this.Services = new ObservableCollection<ServiceItem>();
            this.QueryResults = new ObservableCollection<ResultRow>();
        }

        public void Populate(IEnumerable<ServiceInfo> services)
        {
            this.Services.Clear();

            foreach (var serviceInfo in services)
            {
                var serviceItem = new ServiceItem(serviceInfo);
                RefreshServiceCollectionsFromMetadataCache(serviceItem, serviceInfo);
                this.Services.Add(serviceItem);
            }
        }

        public async Task<bool> AddServiceItemAsync(ServiceInfo serviceInfo)
        {
            var serviceItem = new ServiceItem(serviceInfo);
            RefreshServiceCollectionsFromMetadataCache(serviceItem, serviceInfo);
            this.Services.Add(serviceItem);
            bool ok = await RefreshMetadataCacheAsync(serviceInfo);
            if (ok)
            {
                RefreshServiceCollectionsFromMetadataCache(serviceItem, serviceInfo);
                ok = await _serviceRepository.AddServiceAsync(serviceInfo);
            }
            return ok;
        }

        public async Task<bool> UpdateServiceItemAsync(ServiceItem item, ServiceInfo serviceInfo)
        {
            var originalTitle = item.Name;
            item.UpdateDefinition(serviceInfo);
            var ok = await RefreshMetadataCacheAsync(serviceInfo);
            if (ok)
            {
                RefreshServiceCollectionsFromMetadataCache(item, serviceInfo);
                ok = await _serviceRepository.UpdateServiceAsync(originalTitle, serviceInfo);
            }
            return ok;
        }

        public async Task<bool> RemoveServiceItemAsync(ServiceItem item)
        {
            this.Services.Remove(item);
            var serviceInfo = new ServiceInfo() { Name = item.Name };
            return await _serviceRepository.DeleteServiceAsync(serviceInfo);
        }

        private void RefreshServiceCollectionsFromMetadataCache(ServiceItem item, ServiceInfo service)
        {
            item.Collections.Clear();
            if (!string.IsNullOrEmpty(service.MetadataCache))
            {
                var element = XElement.Parse(service.MetadataCache);
                if (element.Name == "Error")
                {
                    //item.Collections.Add(new ServiceError(
                    //    service.Name, 
                    //    "Unable to load service metadata",
                    //    element.Element("Message").Value));
                }
                else
                {
                    var collections = MetadataService.ParseServiceMetadata(service.MetadataCache);
                    foreach (var collection in collections)
                    {
                        item.Collections.Add(collection);
                    }
                }
            }
        }

        private async Task<bool> RefreshMetadataCacheAsync(ServiceInfo service)
        {
            var metadata = await MetadataService.LoadServiceMetadataAsync(service);
            service.MetadataCache = metadata;
            service.CacheUpdated = DateTimeOffset.UtcNow;
            var serviceItem = this.Services.Single(x => x.Name == service.Name);
            if (serviceItem != null)
            {
                serviceItem.UpdateMetadata(service.MetadataCache);
            }
            await _localStorage.SaveServiceMetadataAsync(service.MetadataCacheFilename, service.MetadataCache);
            return true;
        }
    }
}
