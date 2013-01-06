using System;
using System.Collections.Generic;
using System.Linq;
using ODataPad.Core.Models;
using ODataPad.Core.Services;
using ODataPad.WinRT;

namespace ODataPad.UI.WinRT.DataModel
{
    public class AppData
    {
        public static uint CurrentVersion;
        public const uint DesiredVersion = 2;
        public const string ServicesKey = "Services";
        public readonly ServiceRepository ServiceRepository;

        public AppData()
        {
            this.ServiceRepository = new ServiceRepository(new ServiceLocalStorage());
        }

        public void AddService(ODataServiceInfo serviceInfo)
        {
            serviceInfo.Index = this.ServiceRepository.Services.Count;
            this.ServiceRepository.Services.Add(serviceInfo);
        }

        public void UpdateService(string serviceName, ODataServiceInfo serviceInfo)
        {
            var originalService = this.ServiceRepository.Services.Single(x => x.Name == serviceName);
            originalService.Name = serviceInfo.Name;
            originalService.Url = serviceInfo.Url;
            originalService.Description = serviceInfo.Description;
            originalService.Logo = serviceInfo.Logo;
            originalService.MetadataCache = serviceInfo.MetadataCache;
            originalService.CacheUpdated = serviceInfo.CacheUpdated;
        }

        public void DeleteService(ODataServiceInfo serviceInfo)
        {
            var originalService = this.ServiceRepository.Services.SingleOrDefault(x => x.Name == serviceInfo.Name);
            if (originalService != null)
            {
                this.ServiceRepository.Services.Remove(originalService);
            }
            for (int index = 0; index < this.ServiceRepository.Services.Count; index++)
            {
                this.ServiceRepository.Services[index].Index = index;
            }
        }
    }
}
