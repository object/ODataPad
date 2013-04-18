using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;
using Cirrious.CrossCore.IoC;
using Cirrious.MvvmCross.ViewModels;
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
        private readonly IImageProvider _imageProvider;
        private readonly IResultProvider _resultProvider;

        public HomeViewModel()
        {
            _serviceRepository = Mvx.Resolve<IServiceRepository>();
            _localStorage = Mvx.Resolve<IServiceLocalStorage>();
            _localData = Mvx.Resolve<IApplicationLocalData>();
            _imageProvider = Mvx.Resolve<IImageProvider>();
            _resultProvider = Mvx.Resolve<IResultProvider>();

            PrepareApplicationDataAsync();
        }

        private async Task PrepareApplicationDataAsync()
        {
            await EnsureDataVersionAsync();
            await PopulateServicesAsync();

            this.SelectedService = null;
            this.IsServiceSelected = false;
            this.IsQueryInProgress = false;
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
                serviceItem.Image = _imageProvider.GetImage(serviceItem.ImagePath);
                //RefreshServiceCollectionsFromMetadataCache(serviceItem, serviceInfo);
                this.Services.Add(serviceItem);
            }
        }

        public async Task AddServiceItemAsync(ServiceInfo serviceInfo)
        {
            var serviceItem = new ServiceViewModel(this, serviceInfo);
            //RefreshServiceCollectionsFromMetadataCache(serviceItem, serviceInfo);
            this.Services.Add(serviceItem);
            await RefreshMetadataCacheAsync(serviceInfo);
            //RefreshServiceCollectionsFromMetadataCache(serviceItem, serviceInfo);
            await _serviceRepository.AddServiceAsync(serviceInfo);
        }

        public async Task UpdateServiceItemAsync(ServiceViewModel item, ServiceInfo serviceInfo)
        {
            var originalTitle = item.Name;
            item.UpdateDefinition(serviceInfo);
            await RefreshMetadataCacheAsync(serviceInfo);
            //RefreshServiceCollectionsFromMetadataCache(item, serviceInfo);
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
            RefreshServiceCollectionsFromMetadataCache(this.SelectedService);
            this.IsServiceSelected = this.SelectedService != null;
        }

        public override void SelectCollection()
        {
            if (!this.IsPropertyViewSelected && this.SelectedCollection != null)
            {
                RequestCollectionData();
            }
        }

        public override void SelectCollectionMode()
        {
            if (!this.IsPropertyViewSelected && this.SelectedCollection != null)
            {
                RequestCollectionData();
            }
        }

        public override void LoadMoreResults()
        {
            if (this.SelectedCollection.QueryResults != null 
                && this.SelectedCollection.QueryResults.HasMoreItems
                && !this.IsQueryInProgress)
            {
                _resultProvider.AddResultsAsync(this.SelectedCollection.QueryResults);
            }
        }

        public override void SelectResult()
        {
            if (this.SelectedResult != null)
            {
                ShowResultDetails();
            }
        }

        private void RefreshServiceCollectionsFromMetadataCache(ServiceViewModel item)
        {
            this.Collections.Clear();
            if (!string.IsNullOrEmpty(item.MetadataCache))
            {
                var collections = MetadataService.ParseServiceMetadata(item.MetadataCache);
                foreach (var collection in collections)
                {
                    this.Collections.Add(new CollectionViewModel(this, collection));
                }
            }
        }

        private void RefreshServiceCollectionsFromMetadataCache(ServiceViewModel item, ServiceInfo service)
        {
            //item.Collections.Clear();
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
                        //item.Collections.Add(collection);
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
            await _localStorage.SaveServiceMetadataAsync(service.MetadataCacheFilename, service.MetadataCache);
        }

        private async Task RequestCollectionData()
        {
            this.SelectedCollection.QueryResults = _resultProvider.CreateResultCollection(
                this.SelectedService.Url,
                this.SelectedCollection.Name,
                this.SelectedCollection.Properties,
                new QueryInProgress(this));

            await _resultProvider.AddResultsAsync(this.SelectedCollection.QueryResults);
        }

        private void ShowResultDetails()
        {
            this.SelectedResultDetails = string.Join(Environment.NewLine + Environment.NewLine,
                this.SelectedResult.Properties
                    .Select(y => y.Key + Environment.NewLine + (y.Value == null ? "(null)" : y.Value.ToString())));
        }
    }
}
