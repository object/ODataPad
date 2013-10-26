using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml.Linq;
using Cirrious.MvvmCross.ViewModels;
using ODataPad.Core.Models;
using ODataPad.Core.Services;

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
            _resourceSets = new ResourceSetListViewModel(_serviceInfo.Url);

            RebuildMetadataFromCache();

            AppState.ActiveService = this;
        }

        internal ServiceInfo ServiceInfo { get { return _serviceInfo; }}

        public AppState AppState { get { return AppState.Current; } }

        public string Name { get { return _serviceInfo.Name; } }
        public string Description { get { return _serviceInfo.Description; } }
        public string Url { get { return _serviceInfo.Url; } }
        public string ImageBase64 { get { return _serviceInfo.ImageBase64; } }
        public string MetadataCache { get { return _serviceInfo.MetadataCache; } }

        public ResourceSetListViewModel ResourceSets { get { return _resourceSets; } }

        private void RebuildMetadataFromCache()
        {
            this.ResourceSets.Items.Clear();
            if (!string.IsNullOrEmpty(_serviceInfo.MetadataCache))
            {
                var resourceSets = MetadataService.ParseServiceMetadata(_serviceInfo.MetadataCache);
                foreach (var resourceSet in resourceSets)
                {
                    this.ResourceSets.Items.Add(resourceSet);
                }
            }
        }

        private void RebuildMetadataFromCache(ServiceInfo service)
        {
            //item.Resources.Clear();
            if (!string.IsNullOrEmpty(service.MetadataCache))
            {
                var element = XElement.Parse(service.MetadataCache);
                if (element.Name == "Error")
                {
                    //item.Resources.Add(new ServiceError(
                    //    service.Name, 
                    //    "Unable to load service metadata",
                    //    element.Element("Message").Value));
                }
                else
                {
                    var resources = MetadataService.ParseServiceMetadata(service.MetadataCache);
                    foreach (var resource in resources)
                    {
                        //item.Resources.Add(collection);
                    }
                }
            }
        }
    }
}
