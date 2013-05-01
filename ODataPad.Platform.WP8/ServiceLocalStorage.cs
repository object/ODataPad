﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using ODataPad.Core.Interfaces;
using ODataPad.Core.Models;
using Windows.Storage;

namespace ODataPad.Platform.WP8
{
    public class ServiceLocalStorage : IServiceLocalStorage
    {
        internal static readonly string ServiceDataFolder = "Services";
        internal const string ServiceFile = "Services.xml";

        static ServiceLocalStorage()
        {
            if (!Directory.Exists(ServiceDataFolder))
            {
                Directory.CreateDirectory(ServiceDataFolder);
            }
        }

        public async Task<IEnumerable<ServiceInfo>> LoadServiceInfosAsync()
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
                    await LoadServiceDetailsAsync(serviceInfo);
                    services.Add(serviceInfo);
                }
            }
            return await Task.Factory.StartNew(() => services.Select(x => x));
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

        public async Task ClearServiceInfosAsync()
        {
            await PurgeServiceInfosAsync(new List<ServiceInfo>());

            var serviceFilePath = Path.Combine(ServiceDataFolder, ServiceFile);
            if (File.Exists(serviceFilePath))
            {
                File.Delete(serviceFilePath);
            }
        }

        public async Task LoadServiceDetailsAsync(ServiceInfo serviceInfo)
        {
            var filename = Path.Combine(ServiceDataFolder, serviceInfo.MetadataCacheFilename);
            if (File.Exists(filename))
            {
                using (var reader = new StreamReader(filename))
                {
                    serviceInfo.MetadataCache = await reader.ReadToEndAsync();
                }
            }
            filename = Path.Combine(ServiceDataFolder, serviceInfo.ImageBase64Filename);
            if (File.Exists(filename))
            {
                using (var reader = new StreamReader(filename))
                {
                    serviceInfo.ImageBase64 = await reader.ReadToEndAsync();
                }
            }
        }

        public async Task SaveServiceDetailsAsync(ServiceInfo serviceInfo)
        {
            var filename = Path.Combine(ServiceDataFolder, serviceInfo.MetadataCacheFilename);
            using (var writer = new StreamWriter(filename))
            {
                await writer.WriteAsync(serviceInfo.MetadataCache);
            }
            filename = Path.Combine(ServiceDataFolder, serviceInfo.ImageBase64Filename);
            using (var writer = new StreamWriter(filename))
            {
                await writer.WriteAsync(serviceInfo.ImageBase64);
            }
        }

        public async Task ClearServiceDetailsAsync(ServiceInfo serviceInfo)
        {
            var filename = Path.Combine(ServiceDataFolder, serviceInfo.MetadataCacheFilename);
            File.Delete(filename);
            filename = Path.Combine(ServiceDataFolder, serviceInfo.ImageBase64Filename);
            File.Delete(filename);
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
                    await ClearServiceDetailsAsync(serviceInfo);
                }

                elementsToRemove.Remove();
            }
        }
    }
}