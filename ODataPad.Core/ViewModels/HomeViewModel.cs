using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Cirrious.CrossCore;
using ODataPad.Core.Interfaces;
using ODataPad.Core.Models;
using ODataPad.Core.Services;

namespace ODataPad.Core.ViewModels
{
    public class HomeViewModel : HomeViewModelBase
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly IServiceLocalStorage _localStorage;
        private readonly IApplicationLocalData _localData;

        public HomeViewModel()
        {
            IsDesignTime = false;

            _serviceRepository = Mvx.Resolve<IServiceRepository>();
            _localStorage = Mvx.Resolve<IServiceLocalStorage>();
            _localData = Mvx.Resolve<IApplicationLocalData>();
        }

        public override async Task InitAsync()
        {
            await EnsureDataVersionAsync();
            await PopulateServicesAsync();
        }

        private async Task EnsureDataVersionAsync()
        {
            await _localData.SetDataVersionAsync(ODataPadApp.ApplicationDataVersion);
        }

        private async Task PopulateServicesAsync()
        {
            this.Services.Items.Clear();
            await _serviceRepository.LoadServicesAsync();

            foreach (var serviceInfo in _serviceRepository.Services)
            {
                this.Services.Items.Add(serviceInfo);
            }
        }

        public async Task AddServiceItemAsync(ServiceInfo serviceInfo)
        {
            this.Services.Items.Add(serviceInfo);
            await RefreshMetadataCacheAsync(serviceInfo);
            await _serviceRepository.AddServiceAsync(serviceInfo);
        }

        public async Task UpdateServiceItemAsync(ServiceInfo item, ServiceInfo updatedServiceInfo)
        {
            var originalTitle = item.Name;
            item.UpdateDefinition(updatedServiceInfo);
            await RefreshMetadataCacheAsync(updatedServiceInfo);
            //RefreshServiceResourcesFromMetadataCache(item, serviceInfo);
            await _serviceRepository.UpdateServiceAsync(originalTitle, updatedServiceInfo);
        }

        public async Task RemoveServiceItemAsync(ServiceInfo item)
        {
            this.Services.Items.Remove(item);
            var serviceInfo = new ServiceInfo() { Name = item.Name };
            await _serviceRepository.DeleteServiceAsync(serviceInfo);
        }

        private async Task RefreshMetadataCacheAsync(ServiceInfo service)
        {
            var metadata = await MetadataService.LoadServiceMetadataAsync(service);
            service.MetadataCache = metadata;
            service.CacheUpdated = DateTimeOffset.UtcNow;
            var serviceItem = this.Services.Items.Single(x => x.Name == service.Name);
            if (serviceItem != null)
            {
                serviceItem.UpdateMetadata(service.MetadataCache);
            }
            await _localStorage.SaveServiceDetailsAsync(service);
        }
    }
}
