using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Cirrious.CrossCore.IoC;
using ODataPad.Core.Interfaces;
using ODataPad.Core.Models;

namespace ODataPad.Platform.Net45
{
    public class ApplicationLocalData
        : IApplicationLocalData
    {
        private const string DataVersionAttributeName = "DataVersion";

        public ApplicationLocalData()
        {
        }

        public async Task SetDataVersionAsync(int requestedDataVersion)
        {
            await Mvx.Resolve<IDataVersioningService>().SetDataVersionAsync(GetCurrentDataVersion(), requestedDataVersion);
            SetCurrentDataVersion(requestedDataVersion);
        }

        public int GetCurrentDataVersion()
        {
            var serviceFilePath = Path.Combine(ServiceLocalStorage.ServiceDataFolder, ServiceLocalStorage.ServiceFile);
            if (File.Exists(serviceFilePath))
            {
                var document = XDocument.Load(serviceFilePath);
                var root = document.Element("Services");
                if (root.Attributes().Any(x => x.Name == DataVersionAttributeName))
                {
                    return int.Parse(root.Attribute(DataVersionAttributeName).Value);
                }
            }
            return 0;
        }

        public void SetCurrentDataVersion(int dataVersion)
        {
            var serviceFilePath = Path.Combine(ServiceLocalStorage.ServiceDataFolder, ServiceLocalStorage.ServiceFile);
            var document = XDocument.Load(serviceFilePath);
            document.Element("Services").SetAttributeValue(DataVersionAttributeName, dataVersion);
            document.Save(serviceFilePath);
        }
    }
}