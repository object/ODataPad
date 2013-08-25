using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Cirrious.CrossCore;
using ODataPad.Core.Interfaces;

namespace ODataPad.Platform.Net45
{
    public class ApplicationLocalData
        : IApplicationLocalData
    {
        private int _currentDataVersion;

        public int CurrentDataVersion { get { return _currentDataVersion; } }

        public async Task SetDataVersionAsync(int requestedDataVersion)
        {
            (Mvx.Resolve<IServiceLocalStorage>() as ServiceLocalStorage).CurrentDataVersion = requestedDataVersion;
            await Mvx.Resolve<IDataVersioningService>().SetDataVersionAsync(GetCurrentDataVersion(), requestedDataVersion);
            SetCurrentDataVersion(requestedDataVersion);
        }

        private int GetCurrentDataVersion()
        {
            var serviceFilePath = Path.Combine(ServiceLocalStorage.ServiceDataFolder, ServiceLocalStorage.ServiceFile);
            if (File.Exists(serviceFilePath))
            {
                var document = XDocument.Load(serviceFilePath);
                var root = document.Element("Services");
                if (root.Attributes().Any(x => x.Name == ServiceLocalStorage.DataVersionAttributeName))
                {
                    return int.Parse(root.Attribute(ServiceLocalStorage.DataVersionAttributeName).Value);
                }
            }
            return 0;
        }

        private void SetCurrentDataVersion(int dataVersion)
        {
            var serviceFilePath = Path.Combine(ServiceLocalStorage.ServiceDataFolder, ServiceLocalStorage.ServiceFile);
            var document = XDocument.Load(serviceFilePath);
            document.Element("Services").SetAttributeValue(ServiceLocalStorage.DataVersionAttributeName, dataVersion);
            document.Save(serviceFilePath);
            _currentDataVersion = dataVersion;
        }
    }
}