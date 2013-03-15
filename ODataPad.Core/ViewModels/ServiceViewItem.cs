using System;
using System.Collections.ObjectModel;
using ODataPad.Core.Models;

namespace ODataPad.Core.ViewModels
{
    public class ServiceViewItem
    {
        private readonly ServiceInfo _serviceInfo;

        public ServiceViewItem(HomeViewModelBase viewModel, ServiceInfo serviceInfo)
        {
            this.ViewModel = viewModel;
            _serviceInfo = serviceInfo;
        }

        public HomeViewModelBase ViewModel { get; set; }
        public string Name { get { return _serviceInfo.Name; } }
        public string Description { get { return _serviceInfo.Description; } }
        public string Url { get { return _serviceInfo.Url; } }
        public string ImagePath { get { return GetImagePath(); } }
        public object Image { get; set; }
        public string MetadataCache { get { return _serviceInfo.MetadataCache; } }

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
