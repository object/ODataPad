using System;
using System.Collections.ObjectModel;
using System.Linq;
using ODataPad.Core.Models;

namespace ODataPad.Core.ViewModels
{
    public class FakeHomeViewModel : HomeViewModelBase
    {
        public FakeHomeViewModel()
        {
            var services = new ServiceInfo[]
                               {
                                   new ServiceInfo
                                       {
                                           Name = "OData.org", 
                                           Description = "A service that exposes information from OData.org - like Producers and Consumers - as OData",
                                           Url = "http://services.odata.org/Website/odata.svc/",
                                       },
                                   new ServiceInfo
                                       {
                                           Name = "Netflix", 
                                           Description = "The complete netflix on-demand media catalog",
                                           Url = "http://odata.netflix.com/v2/Catalog/",
                                       },
                                   new ServiceInfo
                                       {
                                           Name = "Stack Overflow", 
                                           Description = "Q&A for programmers",
                                           Url = "http://data.stackexchange.com/stackoverflow/atom",
                                       },
                                   new ServiceInfo
                                       {
                                           Name = "NuGet", 
                                           Description = "Visual Studio extension that makes it easy to install and update open source libraries and tools in Visual Studio",
                                           Url = "http://packages.nuget.org/v1/FeedService.svc/",
                                       },
                               };

            this.Services = new ObservableCollection<ServiceViewItem>(
                services.Select(x => new ServiceViewItem(this, x)));

            this.SelectedService = this.Services.First();

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

            var collections = new ServiceCollection[]
                                  {
                                      new ServiceCollection("ODataConsumers", selectedProperties,
                                                            selectedAssociations),
                                      new ServiceCollection("ODataProducerApplications", new Collection<CollectionProperty>(),
                                                            new Collection<CollectionAssociation>()),
                                      new ServiceCollection("ODataProducerLiveServices", new Collection<CollectionProperty>(),
                                                            new Collection<CollectionAssociation>()),
                                  };

            this.Collections = new ObservableCollection<CollectionViewItem>(
                collections.Select(x => new CollectionViewItem(this, x)));

            this.SelectedCollection = this.Collections.First();
        }
    }
}
