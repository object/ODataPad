using System;
using System.Collections.ObjectModel;
using ODataPad.Core.Models;

namespace ODataPad.Core.ViewModels
{
    public class ServiceViewModel
    {
        private readonly ServiceInfo _serviceInfo;

        public ServiceViewModel(HomeViewModelBase home, ServiceInfo serviceInfo)
        {
            this.Home = home;
            _serviceInfo = serviceInfo;
        }

        public HomeViewModelBase Home { get; set; }
        public string Name { get { return _serviceInfo.Name; } }
        public string Description { get { return _serviceInfo.Description; } }
        public string Url { get { return _serviceInfo.Url; } }
        public string ImagePath { get { return GetImagePath(); } }
        public object Image { get; set; }
        public string ImageBase64 { get { return _serviceInfo.ImageBase64; } }
        public string MetadataCache { get { return _serviceInfo.MetadataCache; } }

        public void UpdateDefinition(ServiceInfo serviceInfo)
        {
            _serviceInfo.Name = serviceInfo.Name;
            _serviceInfo.Url = serviceInfo.Url;
            _serviceInfo.Description = serviceInfo.Description;
        }

        public void UpdateImageBase64(string imageBase64)
        {
            _serviceInfo.ImageBase64 = imageBase64;
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
