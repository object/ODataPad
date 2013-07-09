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
                this.Services = new ObservableCollection<ServiceViewModel>(
                    services.Select(x => new ServiceViewModel(this, x)));

                foreach (var service in this.Services)
                {
                    stream = Assembly.Load(new AssemblyName("ODataPad.Samples"))
                        .GetManifestResourceStream(string.Join(".", namespaceName, "ImagesBase64", service.Name, "png", "base64"));
                    service.ReadImageBase64(stream);
                }

                this.SelectedService = this.Services.First();
                this.IsServiceSelected = true;

                var selectedProperties = new Collection<CollectionProperty>
                                 {
                                     new CollectionProperty("Id", "Int32", true, false),
                                     new CollectionProperty("Name", "String", false, false),
                                     new CollectionProperty("Description", "String", false, false),
                                     new CollectionProperty("ApplicationUrl", "String", false, true),
                                 };

                var selectedAssociations = new Collection<CollectionAssociation>
                {

                };

                var collections = new[]
                                  {
                                      new ServiceCollection("ODataConsumers", selectedProperties,
                                                            selectedAssociations),
                                      new ServiceCollection("ODataProducerApplications", new Collection<CollectionProperty>(),
                                                            new Collection<CollectionAssociation>()),
                                      new ServiceCollection("ODataProducerLiveServices", new Collection<CollectionProperty>(),
                                                            new Collection<CollectionAssociation>()),
                                  };

                this.Collections = new ObservableCollection<CollectionViewModel>(
                    collections.Select(x => new CollectionViewModel(this, x)));

                this.SelectedCollection = this.Collections.First();
            }
        }
    }
}
