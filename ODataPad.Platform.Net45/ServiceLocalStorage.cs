using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using ODataPad.Core.Interfaces;
using ODataPad.Core.Models;

namespace ODataPad.Platform.Net45
{
    public class ServiceLocalStorage : IServiceLocalStorage
    {
        private static readonly string ServiceDataFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Services");
        private const string ServiceFile = "Services.xml";

        static ServiceLocalStorage()
        {
            if (!Directory.Exists(ServiceDataFolder))
            {
                Directory.CreateDirectory(ServiceDataFolder);
            }
        }

        public Task<IEnumerable<ServiceInfo>> LoadServiceInfosAsync()
        {
            var services = new List<ServiceInfo>();
            var serviceFilePath = Path.Combine(ServiceDataFolder, ServiceFile);
            if (File.Exists(serviceFilePath))
            {
                var document = XDocument.Load(serviceFilePath);
                var root = document.Element("Services");
                var elements = root.Elements("Service");
                foreach (var element in elements)
                {
                    var serviceInfo = ServiceInfo.Parse(element.ToString());
                    services.Add(serviceInfo);
                }
            }
            return Task.Factory.StartNew(() => services.Select(x => x));
        }

        public async Task SaveServiceInfosAsync(IEnumerable<ServiceInfo> serviceInfos)
        {
            var element = new XElement("Services");
            foreach (var serviceInfo in serviceInfos)
            {
                element.Add(serviceInfo.AsXElement());
            }
            var serviceFilePath = Path.Combine(ServiceDataFolder, ServiceFile);
            using (var writer = new StreamWriter(serviceFilePath))
            {
                await writer.WriteAsync(element.ToString());
            }
        }

        public async Task<string> LoadServiceMetadataAsync(string filename)
        {
            var metadataFilePath = Path.Combine(ServiceDataFolder, filename);
            using (var reader = new StreamReader(metadataFilePath))
            {
                return await reader.ReadToEndAsync();
            }
        }

        public async Task SaveServiceMetadataAsync(string filename, string metadata)
        {
            var metadataFilePath = Path.Combine(ServiceDataFolder, filename);
            using (var writer = new StreamWriter(metadataFilePath))
            {
                await writer.WriteAsync(metadata);
            }
        }

        public async Task ClearServicesAsync()
        {
            await PurgeServiceInfosAsync(new List<ServiceInfo>());

            var serviceFilePath = Path.Combine(ServiceDataFolder, ServiceFile);
            if (File.Exists(serviceFilePath))
            {
                File.Delete(serviceFilePath);
            }
        }

        private async Task PurgeServiceInfosAsync(IEnumerable<ServiceInfo> serviceInfosToKeep)
        {
            var serviceFilePath = Path.Combine(ServiceDataFolder, ServiceFile);
            if (File.Exists(serviceFilePath))
            {
                var document = XDocument.Load(serviceFilePath);
                var root = document.Element("Services");
                var elements = root.Elements("Service");

                var elementsToRemove = new List<XElement>();
                foreach (var element in elements
                    .Where(x => serviceInfosToKeep.All(y => ServiceInfo.Parse(x).Name != y.Name)))
                {
                    elementsToRemove.Add(element);
                    var serviceInfo = ServiceInfo.Parse(element.ToString());
                    await SaveServiceMetadataAsync(serviceInfo.MetadataCacheFilename, null);
                }

                elementsToRemove.Remove();
            }
        }
    }
}