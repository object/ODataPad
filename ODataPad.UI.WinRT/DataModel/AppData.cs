using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using ODataPad.WinRT;
using ODataPad.Core.Models;
using ODataPad.Core.Services;

namespace ODataPad.UI.WinRT.DataModel
{
    public class AppData
    {
        public static uint CurrentVersion;
        public const uint DesiredVersion = 3;
        public const string ServicesKey = "Services";
        public IList<ODataServiceInfo> Services { get; set; }

        public async Task<IEnumerable<ODataServiceInfo>> LoadServicesAsync()
        {
            var servicesWithMetadata = new List<ODataServiceInfo>();
            var localSettings = ApplicationData.Current.LocalSettings;
            GetOrCreateContainer(ServicesKey);
            foreach (var kv in localSettings.Containers[ServicesKey].Values)
            {
                var serviceInfo = ODataServiceInfo.Parse(kv.Value as string);
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
                var xml = serviceInfo.Format();
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
            this.Services = new ODataServiceInfo[] { };
        }

        public void AddService(ODataServiceInfo serviceInfo)
        {
            serviceInfo.Index = this.Services.Count;
            this.Services.Add(serviceInfo);
        }

        public void UpdateService(string serviceName, ODataServiceInfo serviceInfo)
        {
            var originalService = this.Services.Single(x => x.Name == serviceName);
            originalService.Name = serviceInfo.Name;
            originalService.Url = serviceInfo.Url;
            originalService.Description = serviceInfo.Description;
            originalService.Logo = serviceInfo.Logo;
            originalService.MetadataCache = serviceInfo.MetadataCache;
            originalService.CacheUpdated = serviceInfo.CacheUpdated;
        }

        public void DeleteService(ODataServiceInfo serviceInfo)
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
            var sampleStore = CreateSampleService();

            var services = await sampleStore.GetAllSamplesAsync();
            int index = 0;
            foreach (var serviceInfo in services)
            {
                serviceInfo.Index = index;
                localSettings.Containers[ServicesKey].Values[serviceInfo.Name] = serviceInfo.Format();
                await SaveSettingToFileAsync(serviceInfo.MetadataCacheFilename, serviceInfo.MetadataCache);
                ++index;
            }
            return true;
        }

        public async Task<bool> UpdateSampleServicesAsync()
        {
            var localSettings = ApplicationData.Current.LocalSettings;
            GetOrCreateContainer(ServicesKey);
            var sampleStore = CreateSampleService();

            var newServices = await sampleStore.GetNewSamplesAsync();
            int index = localSettings.Containers[ServicesKey].Values.Count;
            foreach (var serviceInfo in newServices)
            {
                serviceInfo.Index = index;
                localSettings.Containers[ServicesKey].Values[serviceInfo.Name] = serviceInfo.Format();
                await SaveSettingToFileAsync(serviceInfo.MetadataCacheFilename, serviceInfo.MetadataCache);
                ++index;
            }

            var updatedServices = await sampleStore.GetUpdatedSamplesAsync();
            foreach (var serviceInfo in updatedServices)
            {
                if (localSettings.Containers[ServicesKey].Values.ContainsKey(serviceInfo.Name))
                {
                    await SaveSettingToFileAsync(serviceInfo.MetadataCacheFilename, serviceInfo.MetadataCache);
                }
            }

            var expiredServices = await sampleStore.GetExpiredSamplesAsync();
            foreach (var kv in localSettings.Containers[ServicesKey].Values
                .Where(x => expiredServices.Any(y => x.Key == y.Name)))
            {
                localSettings.Containers[ServicesKey].Values.Remove(kv);
            }

            return true;
        }

        private void GetOrCreateContainer(string containerName)
        {
            var localSettings = ApplicationData.Current.LocalSettings;
            if (!localSettings.Containers.ContainsKey(containerName))
            {
                localSettings.CreateContainer(ServicesKey, ApplicationDataCreateDisposition.Always);
            }
        }

        public async static Task<bool> SaveServiceMetadataCacheAsync(ODataServiceInfo serviceInfo)
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

        private ISamplesService CreateSampleService()
        {
            return new SamplesService(new ResourceManager(), 
                "ODataPad.Core", "Samples", "SampleServices.xml", 
                (int)AppData.CurrentVersion, (int)AppData.DesiredVersion);
        }
    }
}
