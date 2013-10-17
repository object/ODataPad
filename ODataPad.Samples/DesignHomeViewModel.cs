using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using ODataPad.Core.Models;
using ODataPad.Core.Services;
using ODataPad.Core.ViewModels;

namespace ODataPad.Samples
{
    public class DesignHomeViewModel : HomeViewModelBase
    {
        public DesignHomeViewModel()
        {
            IEnumerable<ServiceInfo> services = null;
            var namespaceName = typeof(DesignHomeViewModel).Namespace;

            var stream = Assembly.Load(new AssemblyName("ODataPad.Samples"))
                .GetManifestResourceStream(string.Join(".", namespaceName, "SampleServices.xml"));
            using (var reader = new StreamReader(stream))
            {
                services = SamplesService.ParseSamplesXml(reader.ReadToEnd())
                    .Where(x => DesignData.ServiceNames.Contains(x.Name));
            }

            if (services != null)
            {
                this.Services.Items = new ObservableCollection<ServiceDetailsViewModel>(
                    services.Select(x => new ServiceDetailsViewModel(this, x)));

                foreach (var service in this.Services.Items)
                {
                    stream = Assembly.Load(new AssemblyName("ODataPad.Samples"))
                        .GetManifestResourceStream(string.Join(".", namespaceName, "ImagesBase64", service.Name, "png", "base64"));
                    service.ReadImageBase64(stream);
                }

                this.Services.SelectedService = this.Services.Items.First();
                this.Services.IsServiceSelected = true;

                var selectedProperties = new Collection<ResourceProperty>
                                 {
                                     new ResourceProperty("Id", "Int32", true, false),
                                     new ResourceProperty("Name", "String", false, false),
                                     new ResourceProperty("Description", "String", false, false),
                                     new ResourceProperty("ApplicationUrl", "String", false, true),
                                 };

                var selectedAssociations = new Collection<ResourceAssociation>
                {

                };

                var collections = new[]
                                  {
                                      new ResourceSet("ODataConsumers", selectedProperties,
                                                            selectedAssociations),
                                      new ResourceSet("ODataProducerApplications", new Collection<ResourceProperty>(),
                                                            new Collection<ResourceAssociation>()),
                                      new ResourceSet("ODataProducerLiveServices", new Collection<ResourceProperty>(),
                                                            new Collection<ResourceAssociation>()),
                                  };

                this.Services.SelectedService.ResourceSets.Items = new ObservableCollection<ResourceSetDetailsViewModel>(
                    collections.Select(x => new ResourceSetDetailsViewModel(this, x)));

                this.Services.SelectedService.ResourceSets.SelectedItem = this.Services.SelectedService.ResourceSets.Items.First();
            }
        }
    }
}
