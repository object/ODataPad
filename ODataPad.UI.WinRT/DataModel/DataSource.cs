using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;
using ODataPad.WinRT;
using ODataPad.Core.Models;
using Simple.OData.Client;

// The data model defined by this file serves as a representative example of a strongly-typed
// model that supports notification when members are added, removed, or modified.  The property
// names chosen coincide with data bindings in the standard item templates.
//
// Applications may use this model as a starting point and build on it, or discard it entirely and
// replace it with something appropriate to their needs.

namespace ODataPad.UI.WinRT.DataModel
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

            if (App.theApp.ServiceRepository.Services != null)
            {
                foreach (var serviceInfo in App.theApp.ServiceRepository.Services)
                {
                    var item = CreateServiceDataItem(serviceInfo);
                    _rootItem.Elements.Add(item);
                }
            }
        }

        public async Task<bool> AddServiceDataItemAsync(ServiceInfo serviceInfo)
        {
            var serviceItem = this.CreateServiceDataItem(serviceInfo);
            _rootItem.Elements.Add(serviceItem);
            bool ok = await RefreshMetadataCacheAsync(serviceInfo);
            if (ok)
            {
                RefreshServiceCollectionsFromMetadataCache(serviceItem, serviceInfo);
                ok = await App.theApp.ServiceRepository.AddServiceAsync(serviceInfo);
            }
            return ok;
        }

        public async Task<bool> UpdateServiceDataItemAsync(ServiceDataItem serviceItem, ServiceInfo serviceInfo)
        {
            var originalTitle = serviceItem.Title;
            serviceItem.Title = serviceInfo.Name;
            serviceItem.Subtitle = serviceInfo.Url;
            serviceItem.Description = serviceInfo.Description;
            var ok = await RefreshMetadataCacheAsync(serviceInfo);
            if (ok)
            {
                RefreshServiceCollectionsFromMetadataCache(serviceItem, serviceInfo);
                ok = await App.theApp.ServiceRepository.UpdateServiceAsync(originalTitle, serviceInfo);
            }
            return ok;
        }

        public async Task<bool> RemoveServiceDataItemAsync(ServiceDataItem serviceItem)
        {
            _rootItem.Elements.Remove(serviceItem);
            var serviceInfo = new ServiceInfo() { Name = serviceItem.Title };
            return await App.theApp.ServiceRepository.DeleteServiceAsync(serviceInfo);
        }

        private ServiceDataItem CreateServiceDataItem(ServiceInfo service)
        {
            var item = new ServiceDataItem(service);
            RefreshServiceCollectionsFromMetadataCache(item, service);
            return item;
        }

        private void RefreshServiceCollectionsFromMetadataCache(ServiceDataItem item, ServiceInfo service)
        {
            item.Elements.Clear();
            if (!string.IsNullOrEmpty(service.MetadataCache))
            {
                var element = XElement.Parse(service.MetadataCache);
                if (element.Name == "Error")
                {
                    item.Elements.Add(new ErrorDataItem(service, element));
                }
                else
                {
                    var schema = ODataClient.ParseSchemaString(service.MetadataCache);
                    foreach (var table in schema.Tables)
                    {
                        item.Collections.Add(CreateCollectionItem(table));
                    }
                }
            }
        }

        private ViewableItem CreateCollectionItem(Table table)
        {
            var properties = table.Columns.Select(x => 
                new CollectionProperty(
                    x.ActualName,
                    x.PropertyType.Name.Split('.').Last(), 
                    table.GetKeyNames().Contains(x.ActualName), 
                    x.IsNullable));

            var associations = table.Associations.Select(x =>
                new CollectionAssociation(
                    x.ActualName,
                    x.Multiplicity));

            var collection = new ServiceCollection(table.ActualName, properties, associations);
            var item = new ViewableItem(collection);
            item.Elements = new ObservableCollection<ViewableItem>(collection.SchemaItems.Select(x => new ViewableItem(x)));
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
            await new ServiceLocalStorage().SaveServiceMetadataAsync(service.MetadataCacheFilename, service.MetadataCache);
            return true;
        }

        private async Task<string> LoadServiceMetadataAsync(ServiceInfo service)
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
