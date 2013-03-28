using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ODataPad.Core.Interfaces;
using ODataPad.Core.Models;
using Windows.Storage;

namespace ODataPad.Platform.WinRT
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
            return localSettings.Containers[ServicesKey].Values
                .Select(x => ServiceInfo.Parse(x.Value as string));
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

        public async Task<string> LoadServiceMetadataAsync(string filename)
        {
            return await LoadFromLocalStorageAsync(filename);
        }

        public async Task SaveServiceMetadataAsync(string filename, string metadata)
        {
            await SaveToLocalStorageAsync(filename, metadata);
        }

        public async Task ClearServicesAsync()
        {
            await PurgeServiceInfosAsync(new List<ServiceInfo>());
            var localSettings = ApplicationData.Current.LocalSettings;
            localSettings.Values.Clear();
            if (localSettings.Containers.ContainsKey(ServicesKey))
            {
                localSettings.DeleteContainer(ServicesKey);
            }
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
                await SaveServiceMetadataAsync(serviceInfo.MetadataCacheFilename, null);
            }
        }

        private async Task<string> LoadFromLocalStorageAsync(string filename)
        {
            var file = await ApplicationData.Current.LocalFolder
                .CreateFileAsync(filename, CreationCollisionOption.OpenIfExists);
            return await FileIO.ReadTextAsync(file);
        }

        private async Task SaveToLocalStorageAsync(string filename, string text)
        {
            var file = await ApplicationData.Current.LocalFolder
                .CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
            if (!string.IsNullOrEmpty(text))
            {
                await FileIO.WriteTextAsync(file, text);
            }
            else
            {
                await file.DeleteAsync();
            }
        }
    }
}