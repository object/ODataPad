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
            IsDesignTime = true;

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
                this.Services.Populate(services);

                foreach (var service in this.Services.Items)
                {
                    stream = Assembly.Load(new AssemblyName("ODataPad.Samples"))
                        .GetManifestResourceStream(string.Join(".", namespaceName, "ImagesBase64", service.Name, "png", "base64"));
                    service.ReadImageBase64(stream);
                }

                SelectTopItem(this.Services);

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

                AppState.ActiveService.ResourceSets.Populate(
                    collections.Select(x => new ResourceSetDetailsViewModel(this.Services.SelectedService.Url, x)));
                SelectTopItem(AppState.ActiveService.ResourceSets);
            }
        }

        private void SelectTopItem(ServiceListViewModel services)
        {
            services.SelectedService = services.Items.First();
            services.IsServiceSelected = true;
            AppState.ActiveService = new ServiceDetailsViewModel(services.SelectedService);
        }

        private void SelectTopItem(ResourceSetListViewModel resourceSets)
        {
            resourceSets.SelectedItem = resourceSets.Items.First();
            //AppState.ActiveResourceSet = new ResourceSetDetailsViewModel(AppState.ActiveService.Url, resourceSets.SelectedItem);
        }
    }
}
