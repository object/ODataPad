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
        public class NavigationParameters
        {
        }

        private readonly IServiceRepository _serviceRepository;
        private readonly IServiceLocalStorage _localStorage;
        private readonly IApplicationLocalData _localData;

        public HomeViewModel()
        {
            _serviceRepository = Mvx.Resolve<IServiceRepository>();
            _localStorage = Mvx.Resolve<IServiceLocalStorage>();
            _localData = Mvx.Resolve<IApplicationLocalData>();
        }

        public override bool IsDesignTime { get { return false; } }

        public override async Task InitAsync(NavigationParameters parameters)
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
                var serviceItem = new ServiceDetailsViewModel(serviceInfo);
                //RefreshServiceResourcesFromMetadataCache(serviceItem, serviceInfo);
                this.Services.Items.Add(serviceItem);
            }
        }

        public async Task AddServiceItemAsync(ServiceInfo serviceInfo)
        {
            var serviceItem = new ServiceDetailsViewModel(serviceInfo);
            //RefreshServiceResourcesFromMetadataCache(serviceItem, serviceInfo);
            this.Services.Items.Add(serviceItem);
            await RefreshMetadataCacheAsync(serviceInfo);
            //RefreshServiceResourcesFromMetadataCache(serviceItem, serviceInfo);
            await _serviceRepository.AddServiceAsync(serviceInfo);
        }

        public async Task UpdateServiceItemAsync(ServiceDetailsViewModel item, ServiceInfo serviceInfo)
        {
            var originalTitle = item.Name;
            item.UpdateDefinition(serviceInfo);
            await RefreshMetadataCacheAsync(serviceInfo);
            //RefreshServiceResourcesFromMetadataCache(item, serviceInfo);
            await _serviceRepository.UpdateServiceAsync(originalTitle, serviceInfo);
        }

        public async Task RemoveServiceItemAsync(ServiceDetailsViewModel item)
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
