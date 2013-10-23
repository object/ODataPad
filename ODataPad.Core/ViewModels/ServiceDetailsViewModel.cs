using System;
using System.Collections.ObjectModel;
using System.IO;
using Cirrious.MvvmCross.ViewModels;
using ODataPad.Core.Models;

namespace ODataPad.Core.ViewModels
{
    public class ServiceDetailsViewModel : MvxViewModel
    {
        private ServiceInfo _serviceInfo;
        private ResourceSetListViewModel _resourceSets;

        public ServiceDetailsViewModel()
        {
        }

        public ServiceDetailsViewModel(ServiceInfo serviceInfo)
        {
            Init(serviceInfo);
        }

        public void Init(ServiceInfo serviceInfo)
        {
            _serviceInfo = serviceInfo;
            _resourceSets = new ResourceSetListViewModel();
        }

        internal ServiceInfo ServiceInfo { get { return _serviceInfo; }}

        public string Name { get { return _serviceInfo.Name; } }
        public string Description { get { return _serviceInfo.Description; } }
        public string Url { get { return _serviceInfo.Url; } }
        public string ImageBase64 { get { return _serviceInfo.ImageBase64; } }
        public string MetadataCache { get { return _serviceInfo.MetadataCache; } }

        public ResourceSetListViewModel ResourceSets { get { return _resourceSets; } }

        public void UpdateDefinition(ServiceInfo serviceInfo)
        {
            _serviceInfo.Name = serviceInfo.Name;
            _serviceInfo.Url = serviceInfo.Url;
            _serviceInfo.Description = serviceInfo.Description;
        }

        public void ReadImageBase64(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                _serviceInfo.ImageBase64 = reader.ReadToEnd();
            }
        }

        public void UpdateMetadata(string metadata)
        {
            _serviceInfo.MetadataCache = metadata;
        }
    }
}
