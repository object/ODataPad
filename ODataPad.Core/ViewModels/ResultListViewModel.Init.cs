using System.Collections.Generic;
using Cirrious.CrossCore;
using ODataPad.Core.Interfaces;
using ODataPad.Core.Models;

namespace ODataPad.Core.ViewModels
{
    public partial class ResultListViewModel
    {
        public void Init(NavObject navObject)
        {
            if (!HomeViewModelBase.IsDesignTime)
                _resultProvider = Mvx.Resolve<IResultProvider>();

            _serviceUrl = navObject.ServiceUrl;
            _resourceSetName = navObject.ResourceSetName;
            _properties = navObject.Properties;
        }

        public class NavObject
        {
            public string ServiceUrl { get; set; }
            public string ResourceSetName { get; set; }
            public List<ResourceProperty> Properties { get; set; }
        }

        public class SavedState
        {
            public string ServiceUrl { get; set; }
            public string ResourceSetName { get; set; }
            public List<ResourceProperty> Properties { get; set; }
        }

        public SavedState SaveState()
        {
            return new SavedState()
            {
                ServiceUrl = _serviceUrl,
                ResourceSetName = _resourceSetName,
                Properties = _properties,
            };
        }

        public void ReloadState(SavedState savedState)
        {
            Init(new NavObject
            {
                ServiceUrl = savedState.ServiceUrl,
                ResourceSetName = savedState.ResourceSetName,
                Properties = savedState.Properties
            });
        }
    }
}