using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Cirrious.CrossCore;
using ODataPad.Core.Interfaces;

namespace ODataPad.Platform.WP8
{
    public class ApplicationLocalData
        : IApplicationLocalData
    {
        private const string DataVersionAttributeName = "DataVersion";
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
            var serviceFilePath = Path.Combine(ServiceLocalStorage.ServiceDataFolder,
                                               ServiceLocalStorage.ServiceFile);

            using (var reader = ServiceLocalStorage.GetStorageReader(serviceFilePath))
            {
                if (reader != null)
                {
                    var document = XDocument.Load(reader);
                    var root = document.Element("Services");
                    if (root.Attributes().Any(x => x.Name == DataVersionAttributeName))
                    {
                        return int.Parse(root.Attribute(DataVersionAttributeName).Value);
                    }
                }
                return 0;
            }
        }

        private void SetCurrentDataVersion(int dataVersion)
        {
            XDocument document = null;
            var serviceFilePath = Path.Combine(ServiceLocalStorage.ServiceDataFolder, ServiceLocalStorage.ServiceFile);
            using (var reader = ServiceLocalStorage.GetStorageReader(serviceFilePath))
            {
                document = XDocument.Load(reader);
            }

            using (var writer = ServiceLocalStorage.GetStorageWriter(serviceFilePath))
            {
                document.Element("Services").SetAttributeValue(DataVersionAttributeName, dataVersion);
                document.Save(writer);
            }
            _currentDataVersion = dataVersion;
        }
    }
}