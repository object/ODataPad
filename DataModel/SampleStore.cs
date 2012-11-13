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
        private static readonly Dictionary<int, List<string>> ExpiredSample = new Dictionary<int, List<string>>
            {
                {3,  new List<string>(){"DBpedia"}},
            };

        private const string SamplesFolder = "Files/Samples";
        private static readonly DateTime SampleCreationTime = new DateTime(2012, 11, 10);

        public async Task<IEnumerable<ServiceInfo>> CreateAllSamplesAsync(uint currentVersion, uint desiredVersion)
        {
            var xml = await LoadSamplesXmlAsync();
            var allServices = ParseSamplesXml(xml);

            var services = GetCurrentSampleServices(allServices, currentVersion, desiredVersion);
            var servicesWithMetadata = await CreateSamplesMetadataAsync(services);

            return servicesWithMetadata;
        }

        public async Task<IEnumerable<ServiceInfo>> CreateNewSamplesAsync(uint currentVersion, uint desiredVersion)
        {
            var xml = await LoadSamplesXmlAsync();
            var allServices = ParseSamplesXml(xml);

            var newServices = GetNewSampleServices(allServices, currentVersion, desiredVersion);
            var newServicesWithMetadata = await CreateSamplesMetadataAsync(newServices);

            return newServicesWithMetadata;
        }

        public async Task<IEnumerable<ServiceInfo>> DeleteExpiredSamplesAsync(uint currentVersion, uint desiredVersion)
        {
            var xml = await LoadSamplesXmlAsync();
            var allServices = ParseSamplesXml(xml);

            var expiredServices = GetExpiredSampleServices(allServices, currentVersion, desiredVersion);
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

        private IEnumerable<ServiceInfo> GetCurrentSampleServices(IEnumerable<ServiceInfo> allServices, uint currentVersion, uint desiredVersion)
        {
            var services = NewSamples.Where(sample => sample.Key > desiredVersion)
                .Aggregate(allServices, (current, sample) => current.Where(x => !sample.Value.Contains(x.Name)));
            services = ExpiredSample.Where(sample => sample.Key <= desiredVersion)
                .Aggregate(services, (current, sample) => current.Where(x => !sample.Value.Contains(x.Name)));
            return services;
        }

        private IEnumerable<ServiceInfo> GetNewSampleServices(IEnumerable<ServiceInfo> allServices, uint currentVersion, uint desiredVersion)
        {
            var newSamples = NewSamples.Where(sample => sample.Key > currentVersion && sample.Key <= desiredVersion)
                .SelectMany(x => allServices.Where(y => x.Value.Contains(y.Name)));
            return newSamples;
        }

        private IEnumerable<ServiceInfo> GetExpiredSampleServices(IEnumerable<ServiceInfo> allServices, uint currentVersion, uint desiredVersion)
        {
            var expiredSamples = ExpiredSample.Where(sample => sample.Key > currentVersion && sample.Key <= desiredVersion)
                .SelectMany(x => allServices.Where(y => x.Value.Contains(y.Name)));
            return expiredSamples;
        }

        private async Task<IEnumerable<ServiceInfo>> CreateSamplesMetadataAsync(IEnumerable<ServiceInfo> services)
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