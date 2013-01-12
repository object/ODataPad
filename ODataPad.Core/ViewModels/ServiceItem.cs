using System;
using System.Collections.ObjectModel;
using ODataPad.Core.Models;

namespace ODataPad.Core.ViewModels
{
    public class ServiceItem
    {
        private ServiceInfo _serviceInfo;

        public ServiceItem(ServiceInfo serviceInfo)
        {
            _serviceInfo = serviceInfo;
            this.Collections = new ObservableCollection<ServiceCollection>();
        }

        public string Name { get { return _serviceInfo.Name; } }
        public string Description { get { return _serviceInfo.Description; } }
        public string Url { get { return _serviceInfo.Url; } }
        public string ImagePath { get { return GetImagePath(); } }
        public object Image { get; set; }

        public ObservableCollection<ServiceCollection> Collections { get; private set; }

        public void UpdateDefinition(ServiceInfo serviceInfo)
        {
            _serviceInfo.Name = serviceInfo.Name;
            _serviceInfo.Url = serviceInfo.Url;
            _serviceInfo.Description = serviceInfo.Description;
        }

        public void UpdateMetadata(string metadata)
        {
            _serviceInfo.MetadataCache = metadata;
        }

        private string GetImagePath()
        {
            return "Samples/" + (string.IsNullOrEmpty(_serviceInfo.Logo) ? this.Name : _serviceInfo.Logo) + ".png";
        }
    }
}
