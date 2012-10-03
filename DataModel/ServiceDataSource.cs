using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ODataPad.DataModel;
using Simple.OData.Client;
using Windows.ApplicationModel.Resources.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
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
    public sealed class ServiceDataSource
    {
        private static ServiceDataSource _serviceDataSource = new ServiceDataSource();
        private DataItem _rootItem;

        public DataItem RootItem
        {
            get { return _rootItem; }
        }

        public static DataItem GetItem(string itemId)
        {
            return GetItem(_serviceDataSource.RootItem, itemId);
        }

        private static DataItem GetItem(DataItem parent, string itemId)
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
            return null;
        }

        public static void AddServiceItem(ServiceInfo service)
        {
            _serviceDataSource.CreateServiceItem(service);
        }

        public static void UpdateServiceItem(DataItem serviceItem, ServiceInfo service)
        {
            serviceItem.Title = service.Name;
            serviceItem.Subtitle = service.Uri;
            serviceItem.Description = service.Description;
        }

        private ServiceDataSource()
        {
            _rootItem = new DataItem("",
                    "ODataPad",
                    "Registered OData services",
                    "Assets/DarkGray.png",
                    "Description for registered OData services",
                    null,
                    null);

            if (App.AppData.Services != null)
            {
                foreach (var service in App.AppData.Services)
                {
                    var item = CreateServiceItem(service);
                    _rootItem.Elements.Add(item);
                }
            }
        }

        private DataItem CreateServiceItem(ServiceInfo service)
        {
            var metadata = service.MetadataCache;
            var schema = ODataClient.ParseSchemaString(metadata);
            var item = new DataItem(
                service.Name,
                service.Name,
                service.Uri,
                "Samples/" + service.Name + ".png",
                service.Description,
                metadata,
                null
                );
            foreach (var table in schema.Tables)
            {
                var subitem = CreateTableItem(service, table);
                item.Elements.Add(subitem);
            }
            return item;
        }

        private DataItem CreateTableItem(ServiceInfo service, Table table)
        {
            var item = new DataItem(
                service.Name + "/" + table.ActualName,
                table.ActualName,
                string.Format("{0} properties, {1} relations", table.Columns.Count(), table.Associations.Count()),
                null,
                null,
                null,
                null);
            foreach (var column in table.Columns)
            {
                var subitem = CreateColumnItem(service, table, column);
                item.Elements.Add(subitem);
            }
            foreach (var association in table.Associations)
            {
                var subitem = CreateAssociationItem(service, table, association);
                item.Elements.Add(subitem);
            }
            return item;
        }

        private DataItem CreateColumnItem(ServiceInfo service, Table table, Column column)
        {
            var item = new DataItem(
                service.Name + "/" + table.ActualName + "/" + column.ActualName,
                column.ActualName,
                column.PropertyType.ToString().Split('.').Last() + (column.IsNullable ? ("(null)") : null),
                null,
                null,
                null,
                null);
            return item;
        }

        private DataItem CreateAssociationItem(ServiceInfo service, Table table, Association association)
        {
            var item = new DataItem(
                service.Name + "/" + table.ActualName + "/" + association.ActualName,
                association.ActualName,
                association.Multiplicity,
                null,
                null,
                null,
                null);
            return item;
        }
    }
}
