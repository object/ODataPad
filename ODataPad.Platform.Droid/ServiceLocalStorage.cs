using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using ODataPad.Core.Interfaces;
using ODataPad.Core.Models;

namespace ODataPad.Platform.Droid
{
    public class ServiceLocalStorage : IServiceLocalStorage
    {
        internal static readonly string ServiceDataFolder = "Services";
        internal const string ServiceFile = "Services.xml";
        internal const string DataVersionAttributeName = "DataVersion";
        private const string ServiceCollectionElementName = "Services";
        private const string ServiceElementName = "Service";

        public int CurrentDataVersion { get; set; }

        public Task<IEnumerable<ServiceInfo>> LoadServiceInfosAsync()
        {
            var services = new List<ServiceInfo>();
            var serviceFilePath = Path.Combine(ServiceDataFolder, ServiceFile);

            using (var reader = GetStorageReader(serviceFilePath))
            {
                if (reader != null)
                {
                    var document = XDocument.Load(reader);
                    var root = document.Element(ServiceCollectionElementName);
                    var elements = root.Elements(ServiceElementName);
                    foreach (var element in elements)
                    {
                        var serviceInfo = ServiceInfo.Parse(element.ToString());
                        LoadServiceDetailsAsync(serviceInfo);
                        services.Add(serviceInfo);
                    }
                }
            }
            return Task.Factory.StartNew(() => services.Select(x => x));
        }

        public async Task SaveServiceInfosAsync(IEnumerable<ServiceInfo> serviceInfos)
        {
            var element = new XElement(ServiceCollectionElementName);
            foreach (var serviceInfo in serviceInfos)
            {
                element.Add(serviceInfo.AsXElement());
            }

            var serviceFilePath = Path.Combine(ServiceDataFolder, ServiceFile);
            using (var writer = GetStorageWriter(serviceFilePath))
            {
                await writer.WriteAsync(element.ToString());
            }
        }

        public async Task ClearServiceInfosAsync()
        {
            await PurgeServiceInfosAsync(new List<ServiceInfo>());

            var serviceFilePath = Path.Combine(ServiceDataFolder, ServiceFile);
            using (var storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (storage.FileExists(serviceFilePath))
                {
                    storage.DeleteFile(serviceFilePath);
                }
            }
        }

        public async Task LoadServiceDetailsAsync(ServiceInfo serviceInfo)
        {
            var filename = Path.Combine(ServiceDataFolder, serviceInfo.MetadataCacheFilename);
            using (var reader = GetStorageReader(filename))
            {
                if (reader != null)
                {
                    serviceInfo.MetadataCache = await reader.ReadToEndAsync();
                }
            }
            filename = Path.Combine(ServiceDataFolder, serviceInfo.ImageBase64Filename);
            using (var reader = GetStorageReader(filename))
            {
                if (reader != null)
                {
                    serviceInfo.ImageBase64 = await reader.ReadToEndAsync();
                }
            }
        }

        public async Task SaveServiceDetailsAsync(ServiceInfo serviceInfo)
        {
            var filename = Path.Combine(ServiceDataFolder, serviceInfo.MetadataCacheFilename);
            using (var writer = GetStorageWriter(filename))
            {
                await writer.WriteAsync(serviceInfo.MetadataCache);
            }
            filename = Path.Combine(ServiceDataFolder, serviceInfo.ImageBase64Filename);
            using (var writer = GetStorageWriter(filename))
            {
                await writer.WriteAsync(serviceInfo.ImageBase64);
            }
        }

        public async Task ClearServiceDetailsAsync(ServiceInfo serviceInfo)
        {
            using (var storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                var filename = Path.Combine(ServiceDataFolder, serviceInfo.MetadataCacheFilename);
                if (storage.FileExists(filename))
                {
                    storage.DeleteFile(filename);
                }
                filename = Path.Combine(ServiceDataFolder, serviceInfo.ImageBase64Filename);
                if (storage.FileExists(filename))
                {
                    storage.DeleteFile(filename);
                }
            }
        }

        private async Task PurgeServiceInfosAsync(IEnumerable<ServiceInfo> serviceInfosToKeep)
        {
            var serviceFilePath = Path.Combine(ServiceDataFolder, ServiceFile);
            using (var reader = GetStorageReader(serviceFilePath))
            {
                if (reader != null)
                {
                    var document = XDocument.Load(reader);
                    var root = document.Element("Services");
                    var elements = root.Elements("Service");

                    var elementsToRemove = new List<XElement>();
                    foreach (var element in elements
                        .Where(x => serviceInfosToKeep.All(y => ServiceInfo.Parse(x).Name != y.Name)))
                    {
                        elementsToRemove.Add(element);
                        var serviceInfo = ServiceInfo.Parse(element.ToString());
                        await ClearServiceDetailsAsync(serviceInfo);
                    }

                    elementsToRemove.Remove();
                }
            }
        }

        internal static Stream GetStorageStream(string filename, FileMode fileMode, bool checkIfExists = false)
        {
            using (var storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (!checkIfExists || storage.FileExists(filename))
                {
                    var directory = Path.GetDirectoryName(filename);
                    if (!storage.DirectoryExists(directory))
                    {
                        storage.CreateDirectory(directory);
                    }
                    return storage.OpenFile(filename, fileMode);
                }
                else
                {
                    return null;
                }
            }
        }

        internal static StreamReader GetStorageReader(string filename)
        {
            var stream = GetStorageStream(filename, FileMode.Open, true);
            return stream != null ? new StreamReader(stream) : null;
        }

        internal static StreamWriter GetStorageWriter(string filename)
        {
            var stream = GetStorageStream(filename, FileMode.Create, false);
            return new StreamWriter(stream);
        }
    }
}