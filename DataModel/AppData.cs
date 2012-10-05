using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Storage;
using Windows.Storage.Search;

namespace ODataPad.DataModel
{
    public class AppData
    {
        public const int CurrentVersion = 2;
        public const string ServicesKey = "Services";
        public IList<ServiceInfo> Services { get; set; }

        public async Task<IEnumerable<ServiceInfo>> LoadServicesAsync()
        {
            var servicesWithMetadata = new List<ServiceInfo>();
            var localSettings = ApplicationData.Current.LocalSettings;
            GetOrCreateContainer(ServicesKey);
            foreach (var kv in localSettings.Containers[ServicesKey].Values)
            {
                var serviceInfo = ParseServiceInfo(kv.Value as string);
                var serviceInfoWithMetadata = serviceInfo;
                serviceInfoWithMetadata.MetadataCache = await LoadSettingFromFileAsync(serviceInfo.MetadataCacheFilename);
                servicesWithMetadata.Add(serviceInfoWithMetadata);
            }

            this.Services = servicesWithMetadata.OrderBy(x => x.Index).Select(x => x).ToList();
            return this.Services;
        }

        public async Task<bool> SaveServicesAsync()
        {
            var localSettings = ApplicationData.Current.LocalSettings;
            GetOrCreateContainer(ServicesKey);

            foreach (var serviceInfo in this.Services)
            {
                var xml = FormatServiceInfo(serviceInfo);
                localSettings.Containers[ServicesKey].Values[serviceInfo.Name] = xml;
                if (!string.IsNullOrEmpty(serviceInfo.MetadataCache))
                {
                    await SaveSettingToFileAsync(serviceInfo.MetadataCacheFilename, serviceInfo.MetadataCache);
                }
            }

            foreach (var kv in localSettings.Containers[ServicesKey].Values
                .Where(x => !this.Services.Any(y => x.Key == y.Name)))
            {
                localSettings.Containers[ServicesKey].Values.Remove(kv);
            }

            return true;
        }

        public void ClearServicesAsync()
        {
            var localSettings = ApplicationData.Current.LocalSettings;
            localSettings.Values.Clear();
            if (localSettings.Containers.ContainsKey(ServicesKey))
            {
                localSettings.DeleteContainer(ServicesKey);
            }
            this.Services = new ServiceInfo[] { };
        }

        public void AddService(ServiceInfo serviceInfo)
        {
            serviceInfo.Index = this.Services.Count;
            this.Services.Add(serviceInfo);
        }

        public void UpdateService(string serviceName, ServiceInfo serviceInfo)
        {
            var originalService = this.Services.Where(x => x.Name == serviceName).Single();
            originalService.Name = serviceInfo.Name;
            originalService.Url = serviceInfo.Url;
            originalService.Description = serviceInfo.Description;
            originalService.Logo = serviceInfo.Logo;
            originalService.MetadataCache = serviceInfo.MetadataCache;
            originalService.CacheUpdated = serviceInfo.CacheUpdated;
        }

        public void DeleteService(ServiceInfo serviceInfo)
        {
            var originalService = this.Services.Where(x => x.Name == serviceInfo.Name).SingleOrDefault();
            if (originalService != null)
            {
                this.Services.Remove(originalService);
            }
            for (int index = 0; index < this.Services.Count; index++)
            {
                this.Services[index].Index = index;
            }
        }

        public async Task<bool> CreateSampleServicesAsync()
        {
            var resourceMap = Windows.ApplicationModel.Resources.Core.ResourceManager.Current.MainResourceMap;
            var file = await resourceMap.GetSubtree("Files/Samples").GetValue("SampleServices.xml").GetValueAsFileAsync();
            var xml = await FileIO.ReadTextAsync(file);

            var services = await CreateSampleServicesMetadataAsync(ParseSampleServicesInfo(xml));

            var localSettings = ApplicationData.Current.LocalSettings;
            GetOrCreateContainer(ServicesKey);
            int index = 0;
            foreach (var serviceInfo in services)
            {
                serviceInfo.Index = index;
                localSettings.Containers[ServicesKey].Values[serviceInfo.Name] = FormatServiceInfo(serviceInfo);
                await SaveSettingToFileAsync(serviceInfo.MetadataCacheFilename, serviceInfo.MetadataCache);
                ++index;
            }
            return true;
        }

        public static IEnumerable<ServiceInfo> ParseSampleServicesInfo(string xml)
        {
            XElement element = XElement.Parse(xml);
            var services = from e in element.Elements("Service")
                           select new ServiceInfo()
                           {
                               Name = e.Element("Name").Value,
                               Url = e.Element("Url").Value,
                               Description = e.Element("Description").Value,
                               CacheUpdated = TryGetDateTimeValue(e, "CacheUpdated"),
                           };
            return services;
        }

        public static ServiceInfo ParseServiceInfo(string xml)
        {
            XElement element = XElement.Parse(xml);
                return new ServiceInfo()
                {
                    Name = element.Element("Name").Value,
                    Url = element.Element("Url").Value,
                    Description = element.Element("Description").Value,
                    Logo = element.Element("Logo").Value,
                    CacheUpdated = TryGetDateTimeValue(element, "CacheUpdated"),
                    Index = TryGetIntValue(element, "Index"),
                };
        }

        public static string FormatServiceInfo(ServiceInfo serviceInfo)
        {
            var element = new XElement("Service");
            element.Add(new XElement("Name", serviceInfo.Name));
            element.Add(new XElement("Url", serviceInfo.Url));
            element.Add(new XElement("Description", serviceInfo.Description));
            element.Add(new XElement("Logo", serviceInfo.Logo));
            element.Add(new XElement("CacheUpdated", serviceInfo.CacheUpdated));
            element.Add(new XElement("Index", serviceInfo.Index));
            return element.ToString();
        }

        private async Task<IEnumerable<ServiceInfo>> CreateSampleServicesMetadataAsync(IEnumerable<ServiceInfo> services)
        {
            var servicesWithMetadata = new List<ServiceInfo>();
            var resourceMap = Windows.ApplicationModel.Resources.Core.ResourceManager.Current.MainResourceMap;
            foreach (var serviceInfo in services)
            {
                var serviceWithMetadata = serviceInfo;
                var file = await resourceMap.GetSubtree("Files/Samples").GetValue(serviceInfo.MetadataCacheFilename).GetValueAsFileAsync();
                serviceWithMetadata.MetadataCache = await FileIO.ReadTextAsync(file);
                serviceWithMetadata.CacheUpdated = new DateTimeOffset(new DateTime(2012, 10, 1));
                servicesWithMetadata.Add(serviceWithMetadata);
            }
            return servicesWithMetadata;
        }

        private void GetOrCreateContainer(string containerName)
        {
            var localSettings = ApplicationData.Current.LocalSettings;
            if (!localSettings.Containers.ContainsKey(containerName))
            {
                localSettings.CreateContainer(ServicesKey, ApplicationDataCreateDisposition.Always);
            }
        }

        public async static Task<bool> SaveServiceMetadataCacheAsync(ServiceInfo serviceInfo)
        {
            return await SaveSettingToFileAsync(serviceInfo.MetadataCacheFilename, serviceInfo.MetadataCache);
        }

        private static DateTimeOffset? TryGetDateTimeValue(XElement parent, string elementName)
        {
            var element = parent.Element(elementName);
            return element == null ? 
                null : string.IsNullOrEmpty(element.Value) ?
                null :
                new DateTimeOffset?(DateTimeOffset.Parse(element.Value));
        }

        private static int TryGetIntValue(XElement parent, string elementName)
        {
            var element = parent.Element(elementName);
            return element == null ?
                0 : string.IsNullOrEmpty(element.Value) ?
                0 :
                int.Parse(element.Value);
        }

        private static async Task<string> LoadSettingFromFileAsync(string filename)
        {
            StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync(filename, CreationCollisionOption.OpenIfExists);
            return await FileIO.ReadTextAsync(file);
        }

        private static async Task<bool> SaveSettingToFileAsync(string filename, string text)
        {
            StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(file, text);
            return true;
        }
    }
}
