using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using ODataPad.Core.Models;
using ODataPad.Core.Services;

namespace ODataPad.WinRT
{
    public class ServiceLocalStorage : IServiceLocalStorage
    {
        private const string ServicesKey = "Services";

        public async Task<IEnumerable<ServiceInfo>> LoadServiceInfosAsync()
        {
            var localSettings = ApplicationData.Current.LocalSettings;
            GetOrCreateContainer(ServicesKey);
            return localSettings.Containers[ServicesKey].Values
                .Select(x => ServiceInfo.Parse(x.Value as string));
        }

        public async Task<bool> SaveServiceInfosAsync(IEnumerable<ServiceInfo> serviceInfos)
        {
            var localSettings = ApplicationData.Current.LocalSettings;
            GetOrCreateContainer(ServicesKey);

            foreach (var serviceInfo in serviceInfos)
            {
                var xml = serviceInfo.Format();
                localSettings.Containers[ServicesKey].Values[serviceInfo.Name] = xml;
            }

            await PurgeServiceInfosAsync(serviceInfos);
            return true;
        }

        public Task<string> LoadServiceMetadataAsync(string filename)
        {
            return LoadFromLocalStorageAsync(filename);
        }

        public Task<bool> SaveServiceMetadataAsync(string filename, string metadata)
        {
            return SaveToLocalStorageAsync(filename, metadata);
        }

        public async Task<bool> ClearServicesAsync()
        {
            await PurgeServiceInfosAsync(new List<ServiceInfo>());
            var localSettings = ApplicationData.Current.LocalSettings;
            localSettings.Values.Clear();
            if (localSettings.Containers.ContainsKey(ServicesKey))
            {
                localSettings.DeleteContainer(ServicesKey);
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

        private async Task<bool> PurgeServiceInfosAsync(IEnumerable<ServiceInfo> serviceInfosToKeep)
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

            return true;
        }

        private static async Task<string> LoadFromLocalStorageAsync(string filename)
        {
            var file = await ApplicationData.Current.LocalFolder
                .CreateFileAsync(filename, CreationCollisionOption.OpenIfExists);
            return await FileIO.ReadTextAsync(file);
        }

        private static async Task<bool> SaveToLocalStorageAsync(string filename, string text)
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
            return true;
        }
    }
}