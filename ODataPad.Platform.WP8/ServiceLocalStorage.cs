using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ODataPad.Core.Interfaces;
using ODataPad.Core.Models;
using Windows.Storage;

namespace ODataPad.Platform.WP8
{
    public class ServiceLocalStorage : IServiceLocalStorage
    {
        private const string ServicesKey = "Services";

        public ServiceLocalStorage()
        {
        }

        public async Task<IEnumerable<ServiceInfo>> LoadServiceInfosAsync()
        {
            var localSettings = ApplicationData.Current.LocalSettings;
            GetOrCreateContainer(ServicesKey);
            var services = localSettings.Containers[ServicesKey].Values
                .Select(x => ServiceInfo.Parse(x.Value as string));
            foreach (var serviceInfo in services)
            {
                await LoadServiceDetailsAsync(serviceInfo);
            }
            return services;
        }

        public async Task SaveServiceInfosAsync(IEnumerable<ServiceInfo> serviceInfos)
        {
            var localSettings = ApplicationData.Current.LocalSettings;
            GetOrCreateContainer(ServicesKey);

            foreach (var serviceInfo in serviceInfos)
            {
                var xml = serviceInfo.AsString();
                localSettings.Containers[ServicesKey].Values[serviceInfo.Name] = xml;
            }

            await PurgeServiceInfosAsync(serviceInfos);
        }

        public async Task ClearServiceInfosAsync()
        {
            await PurgeServiceInfosAsync(new List<ServiceInfo>());
            var localSettings = ApplicationData.Current.LocalSettings;
            localSettings.Values.Clear();
            if (localSettings.Containers.ContainsKey(ServicesKey))
            {
                localSettings.DeleteContainer(ServicesKey);
            }
        }

        public async Task LoadServiceDetailsAsync(ServiceInfo serviceInfo)
        {
            serviceInfo.MetadataCache = await LoadFromLocalStorageAsync(serviceInfo.MetadataCacheFilename);
            serviceInfo.ImageBase64 = await LoadFromLocalStorageAsync(serviceInfo.ImageBase64Filename);
        }

        public async Task SaveServiceDetailsAsync(ServiceInfo serviceInfo)
        {
            await SaveToLocalStorageAsync(serviceInfo.MetadataCacheFilename, serviceInfo.MetadataCache);
            await SaveToLocalStorageAsync(serviceInfo.ImageBase64Filename, serviceInfo.ImageBase64);
        }

        public async Task ClearServiceDetailsAsync(ServiceInfo serviceInfo)
        {
            await SaveToLocalStorageAsync(serviceInfo.MetadataCacheFilename, null);
            await SaveToLocalStorageAsync(serviceInfo.ImageBase64Filename, null);
        }

        private void GetOrCreateContainer(string containerName)
        {
            var localSettings = ApplicationData.Current.LocalSettings;
            if (!localSettings.Containers.ContainsKey(containerName))
            {
                localSettings.CreateContainer(ServicesKey, ApplicationDataCreateDisposition.Always);
            }
        }

        private async Task PurgeServiceInfosAsync(IEnumerable<ServiceInfo> serviceInfosToKeep)
        {
            var localSettings = ApplicationData.Current.LocalSettings;
            GetOrCreateContainer(ServicesKey);

            foreach (var kv in localSettings.Containers[ServicesKey].Values
                .Where(x => serviceInfosToKeep.All(y => x.Key != y.Name)))
            {
                localSettings.Containers[ServicesKey].Values.Remove(kv);
                var serviceInfo = ServiceInfo.Parse(kv.Value as string);
                await ClearServiceDetailsAsync(serviceInfo);
            }
        }

        private async Task<string> LoadFromLocalStorageAsync(string filename)
        {
            var file = await ApplicationData.Current.LocalFolder
                .CreateFileAsync(filename, CreationCollisionOption.OpenIfExists);

            using (var stream = await file.OpenStreamForReadAsync())
            {
                var reader = new StreamReader(stream);
                return await reader.ReadToEndAsync();
            }
        }

        private async Task SaveToLocalStorageAsync(string filename, string text)
        {
            var file = await ApplicationData.Current.LocalFolder
                .CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
            
            if (!string.IsNullOrEmpty(text))
            {
                using (var stream = await file.OpenStreamForWriteAsync())
                {
                    var reader = new StreamWriter(stream);
                    await reader.WriteAsync(text);
                }
            }
            else
            {
                await file.DeleteAsync();
            }
        }
    }
}