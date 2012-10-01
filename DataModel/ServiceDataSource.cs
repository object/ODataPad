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

        private ObservableCollection<DataGroup> _allGroups = new ObservableCollection<DataGroup>();
        public ObservableCollection<DataGroup> AllGroups
        {
            get { return this._allGroups; }
        }

        public static IEnumerable<DataGroup> GetGroups(string uniqueId)
        {
            if (!uniqueId.Equals("ServiceGroups")) throw new ArgumentException("Only 'ServiceGroups' is supported as a collection of groups");

            return _serviceDataSource.AllGroups;
        }

        public static DataGroup GetGroup(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _serviceDataSource.AllGroups.Where((group) => group.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static DataItem GetItem(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _serviceDataSource.AllGroups.SelectMany(group => group.Items).Where((item) => item.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        private ServiceDataSource()
        {
            var serviceDataGroup = new DataGroup("ServiceGroup",
                    "ODataPad",
                    "Registered OData services",
                    "Assets/DarkGray.png",
                    "Description for registered OData services");
            if (App.AppData.Services != null)
            {
                foreach (var service in App.AppData.Services)
                {
                    var item = CreateServiceItem(serviceDataGroup, service);
                    serviceDataGroup.Items.Add(item);
                }
            }
            this.AllGroups.Add(serviceDataGroup);
        }

        private DataItem CreateServiceItem(DataGroup serviceDataGroup, ServiceInfo service)
        {
            var metadata = service.MetadataCache;
            var schema = ODataClient.ParseSchemaString(metadata);
            var item = new DataItem(
                service.Uri,
                service.Name,
                service.Uri,
                "Samples/" + service.Name + ".png",
                service.Description,
                metadata,
                serviceDataGroup
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
                service.Name + ":" + table.ActualName,
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
                service.Name + ":" + table.ActualName + "." + column.ActualName,
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
                service.Name + ":" + table.ActualName + "." + association.ActualName,
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
