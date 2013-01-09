using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;
using ODataPad.Core.Models;
using ODataPad.Core.Services;
using ODataPad.WinRT;
using ODataPad.UI.WinRT.Common;

namespace ODataPad.UI.WinRT
{
    public sealed class DataSource : BindableBase
    {
        private static DataSource _instance;
        private const int ResultPageSize = 100;

        private DataSource()
        {
            Initialize();
        }

        public static DataSource Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DataSource();
                }
                return _instance;
            }
        }

        private ObservableCollection<ViewableItem> _services = new ObservableCollection<ViewableItem>();
        public ObservableCollection<ViewableItem> Services
        {
            get { return this._services; }
            set { this.SetProperty(ref this._services, value); }
        }

        private void Initialize()
        {
            if (App.theApp.ServiceRepository.Services != null)
            {
                foreach (var serviceInfo in App.theApp.ServiceRepository.Services)
                {
                    var item = CreateServiceItem(serviceInfo);
                    this.Services.Add(item);
                }
            }
        }

        public async Task<bool> AddServiceItemAsync(ServiceInfo serviceInfo)
        {
            var serviceItem = this.CreateServiceItem(serviceInfo);
            this.Services.Add(serviceItem);
            bool ok = await RefreshMetadataCacheAsync(serviceInfo);
            if (ok)
            {
                RefreshServiceCollectionsFromMetadataCache(serviceItem, serviceInfo);
                ok = await App.theApp.ServiceRepository.AddServiceAsync(serviceInfo);
            }
            return ok;
        }

        public async Task<bool> UpdateServiceItemAsync(ViewableItem item, ServiceInfo serviceInfo)
        {
            var serviceItem = item.Data as ServiceInfo;
            var originalTitle = serviceItem.Name;
            serviceItem.Name = serviceInfo.Name;
            serviceItem.Url = serviceInfo.Url;
            serviceItem.Description = serviceInfo.Description;
            var ok = await RefreshMetadataCacheAsync(serviceInfo);
            if (ok)
            {
                RefreshServiceCollectionsFromMetadataCache(item, serviceInfo);
                ok = await App.theApp.ServiceRepository.UpdateServiceAsync(originalTitle, serviceInfo);
            }
            return ok;
        }

        public async Task<bool> RemoveServiceItemAsync(ViewableItem item)
        {
            this.Services.Remove(item);
            var serviceInfo = new ServiceInfo() { Name = (item.Data as ServiceInfo).Name };
            return await App.theApp.ServiceRepository.DeleteServiceAsync(serviceInfo);
        }

        private ViewableItem CreateServiceItem(ServiceInfo service)
        {
            var item = new ViewableItem(service);
            RefreshServiceCollectionsFromMetadataCache(item, service);
            return item;
        }

        private void RefreshServiceCollectionsFromMetadataCache(ViewableItem item, ServiceInfo service)
        {
            item.Elements.Clear();
            if (!string.IsNullOrEmpty(service.MetadataCache))
            {
                var element = XElement.Parse(service.MetadataCache);
                if (element.Name == "Error")
                {
                    item.Elements.Add(new ViewableItem(new ServiceError(
                        service.Name, 
                        "Unable to load service metadata",
                        element.Element("Message").Value)));
                }
                else
                {
                    var collections = MetadataService.ParseServiceMetadata(service.MetadataCache);
                    foreach (var collection in collections)
                    {
                        var subitem = new ViewableItem(collection);
                        subitem.Elements = new ObservableCollection<ViewableItem>(
                            collection.SchemaItems.Select(x => new ViewableItem(x)));
                        item.Elements.Add(subitem);
                    }
                }
            }
        }

        private async Task<bool> RefreshMetadataCacheAsync(ServiceInfo service)
        {
            var metadata = await LoadServiceMetadataAsync(service);
            service.MetadataCache = metadata;
            service.CacheUpdated = DateTimeOffset.UtcNow;
            var serviceItem = DataSource.Instance.Services.Single(x => x.Title == service.Name).Data as ServiceInfo;
            if (serviceItem != null)
            {
                serviceItem.MetadataCache = service.MetadataCache;
            }
            await new ServiceLocalStorage().SaveServiceMetadataAsync(service.MetadataCacheFilename, service.MetadataCache);
            return true;
        }

        private async Task<string> LoadServiceMetadataAsync(ServiceInfo service)
        {
            var task = Task<string>.Factory.StartNew(() =>
            {
                try
                {
                    return MetadataService.GetSchemaAsString(service.Url);
                }
                catch (Exception exception)
                {
                    var element = new XElement("Error");
                    element.Add(new XElement("Message", exception.Message));
                    return element.ToString();
                }
            });
            return await task;
        }
    }
}
