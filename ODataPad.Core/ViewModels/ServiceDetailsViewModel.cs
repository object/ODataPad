using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml.Linq;
using Cirrious.MvvmCross.ViewModels;
using ODataPad.Core.Models;
using ODataPad.Core.Services;

namespace ODataPad.Core.ViewModels
{
    public partial class ServiceDetailsViewModel : MvxViewModel
    {
        private ResourceSetListViewModel _resourceSets;

        public ServiceDetailsViewModel()
        {
        }

        public ServiceDetailsViewModel(ServiceInfo serviceInfo)
        {
            Init(new NavObject(serviceInfo));
        }

        public AppState AppState { get { return AppState.Current; } }

        public string Name { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string ImageBase64 { get; set; }
        public string MetadataCache { get; set; }

        public ResourceSetListViewModel ResourceSets { get { return _resourceSets; } }

        private void RebuildMetadataFromCache()
        {
            this.ResourceSets.Items.Clear();
            if (!string.IsNullOrEmpty(this.MetadataCache))
            {
                var resourceSets = MetadataService.ParseServiceMetadata(this.MetadataCache);
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
