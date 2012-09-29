using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Storage;

namespace ODataPad.DataModel
{
    class AppData
    {
        public const int CurrentVersion = 1;
        public const string ContainerServicesKey = "Services";
        public IEnumerable<ServiceInfo> Services { get; set; }

        public async void CreateSampleServicesAsync()
        {
            var resourceMap = Windows.ApplicationModel.Resources.Core.ResourceManager.Current.MainResourceMap;
            var file = await resourceMap.GetSubtree("Files/Assets").GetValue("SampleServices.xml").GetValueAsFileAsync();
            var xml = await FileIO.ReadTextAsync(file);

            this.Services = ParseServiceInfo(xml);

            var localSettings = ApplicationData.Current.LocalSettings;
            localSettings.Values["Services"] = xml;
        }

        public static IEnumerable<ServiceInfo> ParseServiceInfo(string xml)
        {
            XElement element = XElement.Parse(xml);
            return from e in element.Elements("Service")
                   select new ServiceInfo()
                   {
                       Name = e.Element("Name").Value,
                       Description = e.Element("Description").Value,
                       Uri = e.Element("Uri").Value
                   };
        }
    }
}
