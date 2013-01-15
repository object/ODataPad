using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Cirrious.MvvmCross.ExtensionMethods;
using Cirrious.MvvmCross.ViewModels;
using ODataPad.Core.Interfaces;
using ODataPad.Core.Models;
using ODataPad.Core.Services;

namespace ODataPad.Core.ViewModels
{
    public class HomeViewModel : MvxViewModel
    {
        private const int ResultPageSize = 100;

        public HomeViewModel()
        {
            _services = new ObservableCollection<ServiceItem>();
            this.QueryResults = new ObservableCollection<ResultRow>();

            PopulateAsync();
        }

        private ObservableCollection<ServiceItem> _services;
        public ObservableCollection<ServiceItem> Services
        {
            get { return _services; }
            set { _services = value; RaisePropertyChanged("Services"); }
        }

        public ObservableCollection<ResultRow> QueryResults { get; private set; }

        private IServiceRepository ServiceRepository
        {
            get { return this.GetService<IServiceRepository>(); }
        }

        private IServiceLocalStorage ServiceLocalStorage
        {
            get { return this.GetService<IServiceLocalStorage>(); }
        }

        private IImageProvider ImageProvider
        {
            get { return this.GetService<IImageProvider>(); }
        }

        private async Task<bool> PopulateAsync()
        {
            this.Services.Clear();
            await this.ServiceRepository.LoadServicesAsync();

            foreach (var serviceInfo in this.ServiceRepository.Services)
            {
                var serviceItem = new ServiceItem(serviceInfo);
                serviceItem.Image = this.ImageProvider.GetImage(serviceItem.ImagePath);
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
                ok = await this.ServiceRepository.AddServiceAsync(serviceInfo);
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
                ok = await this.ServiceRepository.UpdateServiceAsync(originalTitle, serviceInfo);
            }
            return ok;
        }

        public async Task<bool> RemoveServiceItemAsync(ServiceItem item)
        {
            this.Services.Remove(item);
            var serviceInfo = new ServiceInfo() { Name = item.Name };
            return await this.ServiceRepository.DeleteServiceAsync(serviceInfo);
            return true;
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
            await this.ServiceLocalStorage.SaveServiceMetadataAsync(service.MetadataCacheFilename, service.MetadataCache);
            return true;
        }
    }
}
