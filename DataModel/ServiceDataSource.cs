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

        private ObservableCollection<ServiceDataGroup> _allGroups = new ObservableCollection<ServiceDataGroup>();
        public ObservableCollection<ServiceDataGroup> AllGroups
        {
            get { return this._allGroups; }
        }

        public static IEnumerable<ServiceDataGroup> GetGroups(string uniqueId)
        {
            if (!uniqueId.Equals("ServiceGroups")) throw new ArgumentException("Only 'ServiceGroups' is supported as a collection of groups");
            
            return _serviceDataSource.AllGroups;
        }

        public static ServiceDataGroup GetGroup(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _serviceDataSource.AllGroups.Where((group) => group.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static ServiceDataItem GetItem(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _serviceDataSource.AllGroups.SelectMany(group => group.Items).Where((item) => item.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        private ServiceDataSource()
        {
            var serviceDataGroup = new ServiceDataGroup("ServiceGroup",
                    "OData Services",
                    "Registered OData services",
                    "Assets/DarkGray.png",
                    "Description for registered OData services");
            if (App.AppData.Services != null)
            {
                foreach (var service in App.AppData.Services)
                {
                    var metadata = service.MetadataCache;
                    var schema = ODataClient.ParseSchemaString(metadata);
                    var item = new ServiceDataItem(
                        service.Uri,
                        service.Name,
                        service.Uri,
                        "Samples/" + service.Name + ".png",
                        service.Description,
                        metadata,
                        serviceDataGroup
                        );
                    foreach(var table in schema.Tables)
                    {
                        item.Collections.Add(table.ActualName);
                    }
                    serviceDataGroup.Items.Add(item);
                }
            }
            this.AllGroups.Add(serviceDataGroup);
        }
    }
}
