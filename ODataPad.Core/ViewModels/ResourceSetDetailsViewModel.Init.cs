using System.Collections.Generic;
using System.Collections.ObjectModel;
using Cirrious.CrossCore;
using Cirrious.CrossCore.Platform;
using ODataPad.Core.Models;

namespace ODataPad.Core.ViewModels
{
    public partial class ResourceSetDetailsViewModel
    {
        public void Init(NavObject navObject)
        {
            var converter = Mvx.Resolve<IMvxJsonConverter>();

            _serviceUrl = navObject.ServiceUrl;
            _resourceSet = new ResourceSet(
                navObject.ResourceSetName,
                converter.DeserializeObject<ObservableCollection<ResourceProperty>>(navObject.SerializedProperties),
                converter.DeserializeObject<ObservableCollection<ResourceAssociation>>(navObject.SerializedAssociations));

            _schema = new SchemaViewModel(_resourceSet.Properties, _resourceSet.Associations);
            _results = new ResultListViewModel(_serviceUrl, _resourceSet.Name, _resourceSet.Properties);

            AppState.ActiveResourceSet = this;
        }

        public class NavObject
        {
            public string ServiceUrl { get; set; }
            public string ResourceSetName { get; set; }
            public string SerializedProperties { get; set; }
            public string SerializedAssociations { get; set; }

            public NavObject() {}

            public NavObject(string serviceUrl, string resourceSetName, 
                IEnumerable<ResourceProperty> properties, IEnumerable<ResourceAssociation> associations)
            {
                var converter = Mvx.Resolve<IMvxJsonConverter>();

                this.ServiceUrl = serviceUrl;
                this.ResourceSetName = resourceSetName;
                this.SerializedProperties = converter.SerializeObject(properties);
                this.SerializedAssociations = converter.SerializeObject(associations);
            }
        }

        public class SavedState
        {
            public string ServiceUrl { get; set; }
            public string ResourceSetName { get; set; }
            public string SerializedProperties { get; set; }
            public string SerializedAssociations { get; set; }
        }

        public SavedState SaveState()
        {
            var converter = Mvx.Resolve<IMvxJsonConverter>();
            return new SavedState()
            {
                ServiceUrl = _serviceUrl,
                ResourceSetName = _resourceSet.Name,
                SerializedProperties = converter.SerializeObject(_resourceSet.Properties),
                SerializedAssociations = converter.SerializeObject(_resourceSet.Associations),
            };
        }

        public void ReloadState(SavedState savedState)
        {
            Init(new NavObject
            {
                ServiceUrl = savedState.ServiceUrl,
                ResourceSetName = savedState.ResourceSetName,
                SerializedProperties = savedState.SerializedProperties,
                SerializedAssociations = savedState.SerializedAssociations,
            });
        }
    }
}