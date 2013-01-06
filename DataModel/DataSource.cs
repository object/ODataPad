using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Xml.Linq;
using ODataPad.Core.Models;
using Simple.OData.Client;

// The data model defined by this file serves as a representative example of a strongly-typed
// model that supports notification when members are added, removed, or modified.  The property
// names chosen coincide with data bindings in the standard item templates.
//
// Applications may use this model as a starting point and build on it, or discard it entirely and
// replace it with something appropriate to their needs.

namespace ODataPad.DataModel
{
    /// <summary>
    /// Creates a collection of groups and items with hard-coded content.
    /// 
    /// SampleDataSource initializes with placeholder data rather than live production
    /// data so that sample data is provided at both design-time and run-time.
    /// </summary>
    public sealed class DataSource
    {
        private static DataSource _instance;
        private RootDataItem _rootItem;
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

        public DataItem RootItem
        {
            get { return _rootItem; }
        }

        public DataItem GetItem(string itemId)
        {
            return GetItem(_rootItem, itemId);
        }

        private DataItem GetItem(DataItem parent, string itemId)
        {
            if (itemId == parent.UniqueId)
                return parent;

            int parentLevels = string.IsNullOrEmpty(parent.UniqueId) ? 0 : parent.UniqueId.Split('/').Count();
            int itemLevels = itemId.Split('/').Count();
            if (itemLevels == parentLevels + 1)
            {
                var matches = parent.Elements.Where(x => x.UniqueId.Equals(itemId));
                if (matches.Count() == 1)
                    return matches.First();
                else
                    return null;
            }
            else
            {
                var matches = parent.Elements.Where(x => itemId.StartsWith(x.UniqueId));
                if (matches.Count() == 1)
                    return GetItem(matches.First(), itemId);
                else
                    return null;
            }
        }

        private void Initialize()
        {
            _rootItem = new RootDataItem();

            if (App.AppData.Services != null)
            {
                foreach (var serviceInfo in App.AppData.Services)
                {
                    var item = CreateServiceDataItem(serviceInfo);
                    _rootItem.Elements.Add(item);
                }
            }
        }

        public async Task<bool> AddServiceDataItemAsync(ODataServiceInfo serviceInfo)
        {
            var serviceItem = this.CreateServiceDataItem(serviceInfo);
            _rootItem.Elements.Add(serviceItem);
            bool ok = await RefreshMetadataCacheAsync(serviceInfo);
            if (ok)
            {
                RefreshServiceCollectionsFromMetadataCache(serviceItem, serviceInfo);
                App.AppData.AddService(serviceInfo);
                ok = await App.AppData.SaveServicesAsync();
            }
            return ok;
        }

        public async Task<bool> UpdateServiceDataItemAsync(ServiceDataItem serviceItem, ODataServiceInfo serviceInfo)
        {
            var originalTitle = serviceItem.Title;
            serviceItem.Title = serviceInfo.Name;
            serviceItem.Subtitle = serviceInfo.Url;
            serviceItem.Description = serviceInfo.Description;
            bool ok = await RefreshMetadataCacheAsync(serviceInfo);
            if (ok)
            {
                RefreshServiceCollectionsFromMetadataCache(serviceItem, serviceInfo);
                App.AppData.UpdateService(originalTitle, serviceInfo);
                ok = await App.AppData.SaveServicesAsync();
            }
            return ok;
        }

        public async Task<bool> RemoveServiceDataItemAsync(ServiceDataItem serviceItem)
        {
            _rootItem.Elements.Remove(serviceItem);
            var serviceInfo = new ODataServiceInfo() { Name = serviceItem.Title };
            App.AppData.DeleteService(serviceInfo);
            bool ok = await App.AppData.SaveServicesAsync();
            return ok;
        }

        private ServiceDataItem CreateServiceDataItem(ODataServiceInfo service)
        {
            var item = new ServiceDataItem(service);
            RefreshServiceCollectionsFromMetadataCache(item, service);
            return item;
        }

        private void RefreshServiceCollectionsFromMetadataCache(ServiceDataItem item, ODataServiceInfo service)
        {
            item.Elements.Clear();
            if (!string.IsNullOrEmpty(service.MetadataCache))
            {
                var element = XElement.Parse(service.MetadataCache);
                if (element.Name == "Error")
                {
                    var errorItem = CreateErrorDataItem(service, element);
                    item.Elements.Add(errorItem);
                }
                else
                {
                    var schema = ODataClient.ParseSchemaString(service.MetadataCache);
                    foreach (var table in schema.Tables)
                    {
                        var collectionItem = CreateCollectionDataItem(service, table);
                        item.Elements.Add(collectionItem);
                    }
                }
            }
        }

        private CollectionDataItem CreateCollectionDataItem(ODataServiceInfo service, Table table)
        {
            var item = new CollectionDataItem(service, table);
            foreach (var column in table.Columns)
            {
                item.Elements.Add(new PropertyDataItem(service, table, column));
            }
            foreach (var association in table.Associations)
            {
                item.Elements.Add(new PropertyDataItem(service, table, association));
            }
            return item;
        }

        private ErrorDataItem CreateErrorDataItem(ODataServiceInfo service, XElement element)
        {
            var item = new ErrorDataItem(service, element);
            return item;
        }

        private async Task<bool> RefreshMetadataCacheAsync(ODataServiceInfo service)
        {
            var metadata = await LoadServiceMetadataAsync(service);
            service.MetadataCache = metadata;
            service.CacheUpdated = DateTimeOffset.UtcNow;
            var serviceItem = GetItem(service.Name) as ServiceDataItem;
            if (serviceItem != null)
            {
                serviceItem.MetadataCache = service.MetadataCache;
            }
            await AppData.SaveServiceMetadataCacheAsync(service);
            return true;
        }

        private async Task<string> LoadServiceMetadataAsync(ODataServiceInfo service)
        {
            var task = Task<string>.Factory.StartNew(() =>
            {
                try
                {
                    return ODataClient.GetSchemaAsString(service.Url);
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
