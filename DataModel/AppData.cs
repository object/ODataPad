using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Storage;

namespace ODataPad.DataModel
{
    public class AppData
    {
        public const int CurrentVersion = 2;
        public const string ServicesKey = "Services";
        public IEnumerable<ServiceInfo> Services { get; set; }

        public async Task<IEnumerable<ServiceInfo>> LoadServicesAsync()
        {
            var servicesWithMetadata = new List<ServiceInfo>();
            var localSettings = ApplicationData.Current.LocalSettings;
            var xml = localSettings.Values[ServicesKey] as string;
            if (!string.IsNullOrEmpty(xml))
            {
                var services = ParseServiceInfo(xml);
                foreach (var serviceInfo in services)
                {
                    var serviceInfoWithMetadata = serviceInfo;
                    serviceInfoWithMetadata.MetadataCache = await LoadSettingFromFileAsync(serviceInfo.Name + ".edmx");
                    servicesWithMetadata.Add(serviceInfoWithMetadata);
                }
                this.Services = servicesWithMetadata;
            }
            else
            {
                this.Services = new ServiceInfo[] { };
            }
            return this.Services;
        }

        public async Task<bool> ClearServicesAsync()
        {
            var localSettings = ApplicationData.Current.LocalSettings;
            localSettings.Values.Clear();
            this.Services = new ServiceInfo[] {};
            return true;
        }

        public async Task<bool> CreateSampleServicesAsync()
        {
            var resourceMap = Windows.ApplicationModel.Resources.Core.ResourceManager.Current.MainResourceMap;
            var file = await resourceMap.GetSubtree("Files/Samples").GetValue("SampleServices.xml").GetValueAsFileAsync();
            var xml = await FileIO.ReadTextAsync(file);

            var services = await CreateSampleServicesMetadataAsync(ParseServiceInfo(xml));

            var localSettings = ApplicationData.Current.LocalSettings;
            localSettings.Values[ServicesKey] = xml;
            foreach (var serviceInfo in services)
            {
                await SaveSettingToFileAsync(serviceInfo.Name + ".edmx", serviceInfo.MetadataCache);
            }
            return true;
        }

        public static IEnumerable<ServiceInfo> ParseServiceInfo(string xml)
        {
            XElement element = XElement.Parse(xml);
            var services = from e in element.Elements("Service")
                   select new ServiceInfo()
                   {
                       Name = e.Element("Name").Value,
                       Description = e.Element("Description").Value,
                       Uri = e.Element("Uri").Value,
                       MetadataCache = TryGetElementValue<string>(e, "MetadataCache"),
                       CacheUpdated = TryGetElementValue<DateTimeOffset?>(e, "CacheUpdated")
                   };
            return services;
        }

        private async Task<IEnumerable<ServiceInfo>> CreateSampleServicesMetadataAsync(IEnumerable<ServiceInfo> services)
        {
            var servicesWithMetadata = new List<ServiceInfo>();
            var resourceMap = Windows.ApplicationModel.Resources.Core.ResourceManager.Current.MainResourceMap;
            foreach (var serviceInfo in services)
            {
                var serviceInfoWithMetadata = serviceInfo;
                var file = await resourceMap.GetSubtree("Files/Samples").GetValue(serviceInfo.Name + ".edmx").GetValueAsFileAsync();
                serviceInfoWithMetadata.MetadataCache = await FileIO.ReadTextAsync(file);
                serviceInfoWithMetadata.CacheUpdated = new DateTimeOffset(new DateTime(2012, 10, 1));
                servicesWithMetadata.Add(serviceInfoWithMetadata);
            }
            return servicesWithMetadata;
        }

        private static T TryGetElementValue<T>(XElement parent, string elementName)
        {
            var element = parent.Element(elementName);
            return element == null ? default(T) : (T)Convert.ChangeType(element.Value, typeof(T));
        }

        private async Task<string> LoadSettingFromFileAsync(string filename)
        {
            StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync(filename, CreationCollisionOption.OpenIfExists);
            return await FileIO.ReadTextAsync(file);
        }

        private async Task<bool> SaveSettingToFileAsync(string filename, string text)
        {
            StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(file, text);
            return true;
        }
    }
}
