using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;
using Cirrious.MvvmCross.Commands;
using Cirrious.MvvmCross.ExtensionMethods;
using Cirrious.MvvmCross.ViewModels;
using ODataPad.Core.Interfaces;
using ODataPad.Core.Models;
using ODataPad.Core.Services;

namespace ODataPad.Core.ViewModels
{
    public class HomeViewModel : BaseHomeViewModel
    {
        private const int ResultPageSize = 100;
        private readonly IServiceRepository _serviceRepository;
        private readonly IServiceLocalStorage _localStorage;
        private readonly IApplicationLocalData _localData;
        private readonly IImageProvider _imageProvider;

        public HomeViewModel()
        {
            _serviceRepository = this.GetService<IServiceRepository>();
            _localStorage = this.GetService<IServiceLocalStorage>();
            _localData = this.GetService<IApplicationLocalData>();
            _imageProvider = this.GetService<IImageProvider>();

            this.QueryResults = new ObservableCollection<ResultRow>();

            PrepareApplicationDataAsync();
        }

        public ObservableCollection<ResultRow> QueryResults { get; private set; }

        private async Task<bool> PrepareApplicationDataAsync()
        {
            await EnsureDataVersionAsync();
            await PopulateServicesAsync();
            return true;
        }

        private async Task<bool> EnsureDataVersionAsync()
        {
            return await _localData.SetDataVersionAsync(ODataPadApp.ApplicationDataVersion);
        }

        private async Task<bool> PopulateServicesAsync()
        {
            this.Services.Clear();
            await _serviceRepository.LoadServicesAsync();

            foreach (var serviceInfo in _serviceRepository.Services)
            {
                var serviceItem = new ServiceItem(serviceInfo);
                serviceItem.Image = _imageProvider.GetImage(serviceItem.ImagePath);
                RefreshServiceCollectionsFromMetadataCache(serviceItem, serviceInfo);
                this.Services.Add(serviceItem);
            }
            return true;
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

        public ICommand CollectionModeCommand
        {
            get
            {
                return new MvxRelayCommand(DoSelectCollectionMode);
            }
        }

        public void DoSelectCollectionMode()
        {
            // TODO
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
