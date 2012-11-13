using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Foundation;
using Windows.Storage;

namespace ODataPad.DataModel
{
    public class SampleStore
    {
        private static readonly Dictionary<int, List<string>> NewSamples = new Dictionary<int, List<string>>
            {
                {3,  new List<string>(){"Pluralsight"}},
            };
        private static readonly Dictionary<int, List<string>> UpdatedSamples = new Dictionary<int, List<string>>
            {
                {3,  new List<string>(){"Stack Overflow"}},
            };
        private static readonly Dictionary<int, List<string>> ExpiredSample = new Dictionary<int, List<string>>
            {
                {3,  new List<string>(){"DBpedia"}},
            };

        private const string SamplesFolder = "Files/Samples";
        private static readonly DateTime SampleCreationTime = new DateTime(2012, 11, 10);

        public async Task<IEnumerable<ServiceInfo>> GetAllSamplesAsync(uint currentVersion, uint desiredVersion)
        {
            var xml = await LoadSamplesXmlAsync();
            var allServices = ParseSamplesXml(xml);

            var services = NewSamples.Where(sample => sample.Key > desiredVersion)
                .Aggregate(allServices, (current, sample) => current.Where(x => !sample.Value.Contains(x.Name)));
            services = ExpiredSample.Where(sample => sample.Key <= desiredVersion)
                .Aggregate(services, (current, sample) => current.Where(x => !sample.Value.Contains(x.Name)));
            var servicesWithMetadata = await GetSamplesMetadataAsync(services);

            return servicesWithMetadata;
        }

        public async Task<IEnumerable<ServiceInfo>> GetNewSamplesAsync(uint currentVersion, uint desiredVersion)
        {
            var xml = await LoadSamplesXmlAsync();
            var allServices = ParseSamplesXml(xml);

            var newServices = NewSamples.Where(sample => sample.Key > currentVersion && sample.Key <= desiredVersion)
                .SelectMany(x => allServices.Where(y => x.Value.Contains(y.Name)));
            var newServicesWithMetadata = await GetSamplesMetadataAsync(newServices);

            return newServicesWithMetadata;
        }

        public async Task<IEnumerable<ServiceInfo>> GetUpdatedSamplesAsync(uint currentVersion, uint desiredVersion)
        {
            var xml = await LoadSamplesXmlAsync();
            var allServices = ParseSamplesXml(xml);

            var updatedServices = UpdatedSamples.Where(sample => sample.Key > currentVersion && sample.Key <= desiredVersion)
                .SelectMany(x => allServices.Where(y => x.Value.Contains(y.Name)));
            var updatedServicesWithMetadata = await GetSamplesMetadataAsync(updatedServices);

            return updatedServicesWithMetadata;
        }

        public async Task<IEnumerable<ServiceInfo>> GetExpiredSamplesAsync(uint currentVersion, uint desiredVersion)
        {
            var xml = await LoadSamplesXmlAsync();
            var allServices = ParseSamplesXml(xml);

            var expiredServices = ExpiredSample.Where(sample => sample.Key > currentVersion && sample.Key <= desiredVersion)
                .SelectMany(x => allServices.Where(y => x.Value.Contains(y.Name)));
            return expiredServices;
        }

        private async Task<string> LoadSamplesXmlAsync()
        {
            var resourceMap = Windows.ApplicationModel.Resources.Core.ResourceManager.Current.MainResourceMap;
            var file = await resourceMap.GetSubtree(SamplesFolder).GetValue("SampleServices.xml").GetValueAsFileAsync();
            return await FileIO.ReadTextAsync(file);
        }

        private IEnumerable<ServiceInfo> ParseSamplesXml(string xml)
        {
            XElement element = XElement.Parse(xml);
            var services = from e in element.Elements("Service")
                           select AppData.ParseServiceInfo(e);
            return services;
        }

        private async Task<IEnumerable<ServiceInfo>> GetSamplesMetadataAsync(IEnumerable<ServiceInfo> services)
        {
            var servicesWithMetadata = new List<ServiceInfo>();
            var resourceMap = Windows.ApplicationModel.Resources.Core.ResourceManager.Current.MainResourceMap;
            foreach (var serviceInfo in services)
            {
                var serviceWithMetadata = serviceInfo;
                var res = resourceMap.GetSubtree(SamplesFolder).GetValue(serviceInfo.MetadataCacheFilename);
                var file = await res.GetValueAsFileAsync();

                serviceWithMetadata.MetadataCache = await FileIO.ReadTextAsync(file);
                serviceWithMetadata.CacheUpdated = new DateTimeOffset(SampleCreationTime);
                servicesWithMetadata.Add(serviceWithMetadata);
            }
            return servicesWithMetadata;
        }
    }
}