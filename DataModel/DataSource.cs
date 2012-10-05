using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using ODataPad.DataModel;
using Simple.OData.Client;
using Windows.ApplicationModel.Resources.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using System.Collections.Specialized;

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

        public async Task<bool> AddServiceDataItemAsync(ServiceInfo service)
        {
            await RefreshMetadataCacheAsync(service);
            _rootItem.Elements.Add(this.CreateServiceDataItem(service));
            return await App.AppData.SaveServicesAsync();
        }

        public async Task<bool> UpdateServiceDataItemAsync(ServiceDataItem serviceItem, ServiceInfo service)
        {
            serviceItem.Title = service.Name;
            serviceItem.Subtitle = service.Url;
            serviceItem.Description = service.Description;
            await RefreshMetadataCacheAsync(service);
            return await App.AppData.SaveServicesAsync();
        }

        public async Task<bool> RemoveServiceDataItemAsync(ServiceDataItem serviceItem)
        {
            _rootItem.Elements.Remove(serviceItem);
            return true;
        }

        private ServiceDataItem CreateServiceDataItem(ServiceInfo service)
        {
            var metadata = service.MetadataCache;
            var schema = ODataClient.ParseSchemaString(metadata);
            var item = new ServiceDataItem(service);

            foreach (var table in schema.Tables)
            {
                var subitem = CreateCollectionDataItem(service, table);
                item.Elements.Add(subitem);
            }
            return item;
        }

        private CollectionDataItem CreateCollectionDataItem(ServiceInfo service, Table table)
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

        private async Task<bool> RefreshMetadataCacheAsync(ServiceInfo service)
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

        private async Task<string> LoadServiceMetadataAsync(ServiceInfo service)
        {
            var task = Task<string>.Factory.StartNew(() =>
            {
                var metadata = ODataClient.GetSchemaAsString(service.Url);
                return metadata;
            });
            return task.Result;
        }
    }
}
