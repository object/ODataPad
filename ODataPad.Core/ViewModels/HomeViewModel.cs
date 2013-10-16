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

            this.SelectedService = null;
            this.IsServiceSelected = false;
        }

        public override bool IsDesignTime { get { return false; } }

        public async Task Init(NavigationParameters parameters)
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
            this.Services.Clear();
            await _serviceRepository.LoadServicesAsync();

            foreach (var serviceInfo in _serviceRepository.Services)
            {
                var serviceItem = new ServiceViewModel(this, serviceInfo);
                //RefreshServiceResourcesFromMetadataCache(serviceItem, serviceInfo);
                this.Services.Add(serviceItem);
            }
        }

        public async Task AddServiceItemAsync(ServiceInfo serviceInfo)
        {
            var serviceItem = new ServiceViewModel(this, serviceInfo);
            //RefreshServiceResourcesFromMetadataCache(serviceItem, serviceInfo);
            this.Services.Add(serviceItem);
            await RefreshMetadataCacheAsync(serviceInfo);
            //RefreshServiceResourcesFromMetadataCache(serviceItem, serviceInfo);
            await _serviceRepository.AddServiceAsync(serviceInfo);
        }

        public async Task UpdateServiceItemAsync(ServiceViewModel item, ServiceInfo serviceInfo)
        {
            var originalTitle = item.Name;
            item.UpdateDefinition(serviceInfo);
            await RefreshMetadataCacheAsync(serviceInfo);
            //RefreshServiceResourcesFromMetadataCache(item, serviceInfo);
            await _serviceRepository.UpdateServiceAsync(originalTitle, serviceInfo);
        }

        public async Task RemoveServiceItemAsync(ServiceViewModel item)
        {
            this.Services.Remove(item);
            var serviceInfo = new ServiceInfo() { Name = item.Name };
            await _serviceRepository.DeleteServiceAsync(serviceInfo);
        }

        public override void AddService()
        {
            this.IsServiceEditInProgress = true;
            this.EditedService = null;
        }

        public override void EditService()
        {
            this.IsServiceEditInProgress = true;
            this.EditedService = this.SelectedService;
        }

        public override void RemoveService()
        {
            this.IsServiceEditInProgress = true;
            this.EditedService = this.SelectedService;
        }

        public override void SelectService()
        {
            if (this.SelectedService != null)
                RefreshServiceResourcesFromMetadataCache(this.SelectedService);
            this.IsServiceSelected = this.SelectedService != null;
        }

        public async override void SelectResourceSet()
        {
            if (this.IsResultViewSelected && this.SelectedResourceSet != null)
            {
                await this.SelectedResourceSet.Results.RequestResourceData();
            }
        }

        public async override void SelectResourceMode()
        {
            if (this.IsResultViewSelected && this.SelectedResourceSet != null)
            {
                await this.SelectedResourceSet.Results.RequestResourceData();
            }
        }

        private void RefreshServiceResourcesFromMetadataCache(ServiceViewModel item)
        {
            this.ResourceSets.Clear();
            if (!string.IsNullOrEmpty(item.MetadataCache))
            {
                var resources = MetadataService.ParseServiceMetadata(item.MetadataCache);
                foreach (var resource in resources)
                {
                    this.ResourceSets.Add(new ResourceSetViewModel(this, resource));
                }
            }
        }

        private void RefreshServiceResourcesFromMetadataCache(ServiceViewModel item, ServiceInfo service)
        {
            //item.Resources.Clear();
            if (!string.IsNullOrEmpty(service.MetadataCache))
            {
                var element = XElement.Parse(service.MetadataCache);
                if (element.Name == "Error")
                {
                    //item.Resources.Add(new ServiceError(
                    //    service.Name, 
                    //    "Unable to load service metadata",
                    //    element.Element("Message").Value));
                }
                else
                {
                    var resources = MetadataService.ParseServiceMetadata(service.MetadataCache);
                    foreach (var resource in resources)
                    {
                        //item.Resources.Add(collection);
                    }
                }
            }
        }

        private async Task RefreshMetadataCacheAsync(ServiceInfo service)
        {
            var metadata = await MetadataService.LoadServiceMetadataAsync(service);
            service.MetadataCache = metadata;
            service.CacheUpdated = DateTimeOffset.UtcNow;
            var serviceItem = this.Services.Single(x => x.Name == service.Name);
            if (serviceItem != null)
            {
                serviceItem.UpdateMetadata(service.MetadataCache);
            }
            await _localStorage.SaveServiceDetailsAsync(service);
        }
    }
}
