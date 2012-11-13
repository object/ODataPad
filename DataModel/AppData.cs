using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Storage;

namespace ODataPad.DataModel
{
    public class AppData
    {
        public static uint CurrentVersion;
        public const uint DesiredVersion = 3;
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
                await SaveSettingToFileAsync(serviceInfo.MetadataCacheFilename, serviceInfo.MetadataCache);
            }

            foreach (var kv in localSettings.Containers[ServicesKey].Values
                .Where(x => this.Services.All(y => x.Key != y.Name)))
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
            var originalService = this.Services.Single(x => x.Name == serviceName);
            originalService.Name = serviceInfo.Name;
            originalService.Url = serviceInfo.Url;
            originalService.Description = serviceInfo.Description;
            originalService.Logo = serviceInfo.Logo;
            originalService.MetadataCache = serviceInfo.MetadataCache;
            originalService.CacheUpdated = serviceInfo.CacheUpdated;
        }

        public void DeleteService(ServiceInfo serviceInfo)
        {
            var originalService = this.Services.SingleOrDefault(x => x.Name == serviceInfo.Name);
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
            var localSettings = ApplicationData.Current.LocalSettings;
            GetOrCreateContainer(ServicesKey);
            var sampleStore = new SampleStore();

            var services = await sampleStore.CreateAllSamplesAsync(AppData.CurrentVersion, AppData.DesiredVersion);

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

        public async Task<bool> UpdateSampleServicesAsync()
        {
            var localSettings = ApplicationData.Current.LocalSettings;
            GetOrCreateContainer(ServicesKey);
            var sampleStore = new SampleStore();

            var newServices = await sampleStore.CreateNewSamplesAsync(AppData.CurrentVersion, AppData.DesiredVersion);

            int index = localSettings.Containers[ServicesKey].Values.Count;
            foreach (var serviceInfo in newServices)
            {
                serviceInfo.Index = index;
                localSettings.Containers[ServicesKey].Values[serviceInfo.Name] = FormatServiceInfo(serviceInfo);
                await SaveSettingToFileAsync(serviceInfo.MetadataCacheFilename, serviceInfo.MetadataCache);
                ++index;
            }

            var expiredServices = await sampleStore.DeleteExpiredSamplesAsync(AppData.CurrentVersion, AppData.DesiredVersion);
            foreach (var kv in localSettings.Containers[ServicesKey].Values
                .Where(x => expiredServices.Any(y => x.Key == y.Name)))
            {
                localSettings.Containers[ServicesKey].Values.Remove(kv);
            }

            return true;
        }

        public static ServiceInfo ParseServiceInfo(string xml)
        {
            return ParseServiceInfo(XElement.Parse(xml));
        }

        public static ServiceInfo ParseServiceInfo(XElement element)
        {
            return new ServiceInfo()
            {
                Name = Utils.TryGetStringValue(element, "Name"),
                Url = Utils.TryGetStringValue(element, "Url"),
                Description = Utils.TryGetStringValue(element, "Description"),
                Logo = Utils.TryGetStringValue(element, "Logo"),
                CacheUpdated = Utils.TryGetDateTimeValue(element, "CacheUpdated"),
                Index = Utils.TryGetIntValue(element, "Index"),
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

        private static async Task<string> LoadSettingFromFileAsync(string filename)
        {
            StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync(filename, CreationCollisionOption.OpenIfExists);
            return await FileIO.ReadTextAsync(file);
        }

        private static async Task<bool> SaveSettingToFileAsync(string filename, string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(file, text);
            }
            else
            {
                StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
                await file.DeleteAsync();
            }
            return true;
        }
    }
}
