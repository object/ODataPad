using System;
using System.Collections.ObjectModel;
using System.Linq;
using ODataPad.Core.Models;
using ODataPad.Core.ViewModels;
using Windows.UI.Xaml.Media.Imaging;

namespace ODataPad.UI.WinRT
{
    public class FakeHomeViewModel : BaseHomeViewModel
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

            this.Services = new ObservableCollection<ServiceItem>(
                services.Select(x =>
                                    {
                                        var item = new ServiceItem(x);
                                        item.Image = new BitmapImage(new Uri("ms-appx:///Samples/" + item.Name + ".png"));
                                        return item;
                                    }));
        }
    }
}
