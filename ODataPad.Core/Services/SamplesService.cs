using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Cirrious.CrossCore.IoC;
using ODataPad.Core.Interfaces;
using ODataPad.Core.Models;

namespace ODataPad.Core.Services
{
    public class SamplesService
        : ISamplesService
    {
        private readonly IResourceManager _resourceManager;
        private readonly IServiceLocalStorage _localStorage;
        private readonly string _moduleName;
        private readonly string _samplesFilename;
        private readonly int _previousDataVersion;
        private readonly int _currentDataVersion;

        private static readonly Dictionary<int, List<string>> NewSamples = new Dictionary<int, List<string>>
            {
                {2,  new List<string>()
                         {
                             "DBpedia",
                             "Devexpress Channel",
                             "ebay",
                             "nerddinner",
                             "Netflix",
                             "Northwind Service",
                             "NuGet",
                             "OData.org",
                             "Stack Overflow",
                             "twitpic",
                         }},
                {3,  new List<string>()
                         {
                             "Pluralsight"
                         }},
            };
        private static readonly Dictionary<int, List<string>> UpdatedSamples = new Dictionary<int, List<string>>
            {
                {3,  new List<string>()
                         {
                             "Stack Overflow"
                         }},
            };
        private static readonly Dictionary<int, List<string>> ExpiredSample = new Dictionary<int, List<string>>
            {
                {3,  new List<string>()
                         {
                             "DBpedia"
                         }},
            };

        private static readonly DateTime SampleCreationTime = new DateTime(2012, 11, 10);

        public SamplesService(
            string moduleName, string samplesFilename,
            int previousDataVersion, int currentDataVersion,
            IResourceManager resourceManager = null, IServiceLocalStorage localStorage = null)
        {
            _resourceManager = resourceManager ?? Mvx.Resolve<IResourceManager>();
            _localStorage = localStorage ?? Mvx.Resolve<IServiceLocalStorage>();

            _moduleName = moduleName;
            _samplesFilename = samplesFilename;
            _previousDataVersion = previousDataVersion;
            _currentDataVersion = currentDataVersion;
        }

        public async Task<IEnumerable<ServiceInfo>> GetAllSamplesAsync()
        {
            var xml = await _resourceManager.LoadResourceAsStringAsync(_moduleName, string.Empty, _samplesFilename);
            var allSamples = ParseSamplesXml(xml);

            var samples = NewSamples.Where(sample => sample.Key > _currentDataVersion)
                .Aggregate(allSamples, (current, sample) => current.Where(x => !sample.Value.Contains(x.Name)));
            samples = ExpiredSample.Where(sample => sample.Key <= _currentDataVersion)
                .Aggregate(samples, (current, sample) => current.Where(x => !sample.Value.Contains(x.Name)));

            return samples.ToList();
        }

        public async Task<IEnumerable<ServiceInfo>> GetNewSamplesAsync()
        {
            var xml = await _resourceManager.LoadResourceAsStringAsync(_moduleName, string.Empty, _samplesFilename);
            var allSamples = ParseSamplesXml(xml);

            var samples = NewSamples
                .Where(sample => sample.Key > _previousDataVersion && sample.Key <= _currentDataVersion)
                .SelectMany(x => allSamples.Where(y => x.Value.Contains(y.Name)));

            return samples.ToList();
        }

        public async Task<IEnumerable<ServiceInfo>> GetUpdatedSamplesAsync()
        {
            var xml = await _resourceManager.LoadResourceAsStringAsync(_moduleName, string.Empty, _samplesFilename);
            var allSamples = ParseSamplesXml(xml);

            var samples = UpdatedSamples
                .Where(sample => sample.Key > _previousDataVersion && sample.Key <= _currentDataVersion)
                .SelectMany(x => allSamples.Where(y => x.Value.Contains(y.Name)));

            return samples.ToList();
        }

        public async Task<IEnumerable<ServiceInfo>> GetExpiredSamplesAsync()
        {
            var xml = await _resourceManager.LoadResourceAsStringAsync(_moduleName, string.Empty, _samplesFilename);
            var allSamples = ParseSamplesXml(xml);

            var samples = ExpiredSample
                .Where(sample => sample.Key > _previousDataVersion && sample.Key <= _currentDataVersion)
                .SelectMany(x => allSamples.Where(y => x.Value.Contains(y.Name)));

            return samples.ToList();
        }

        public async Task CreateSamplesAsync()
        {
            var allSamples = await GetAllSamplesAsync();
            var index = 0;
            foreach (var serviceInfo in allSamples)
            {
                serviceInfo.Index = index;
                ++index;
            }
            await _localStorage.SaveServiceInfosAsync(allSamples);
            var samplesDetails = await GetSamplesDetailsAsync(allSamples);
            foreach (var serviceInfo in samplesDetails)
            {
                await _localStorage.SaveServiceDetailsAsync(serviceInfo);
            }
        }

        public async Task UpdateSamplesAsync()
        {
            var newServices = await GetNewSamplesAsync();
            var updatedServices = await GetUpdatedSamplesAsync();
            var expiredServices = await GetExpiredSamplesAsync();
            var oldServices = await _localStorage.LoadServiceInfosAsync();

            var index = oldServices.Count();
            foreach (var serviceInfo in newServices)
            {
                serviceInfo.Index = index;
                ++index;
            }
            updatedServices = updatedServices.Where(x => oldServices.Any(y => x.Name == y.Name));
            foreach (var serviceInfo in updatedServices)
            {
                var oldService = oldServices.SingleOrDefault(x => x.Name == serviceInfo.Name);
                if (oldService != null)
                {
                    serviceInfo.Index = oldService.Index;
                }
            }

            var allServices = oldServices.Where(x => expiredServices.All(y => x.Name != y.Name)).ToList();
            allServices = allServices.Where(x => updatedServices.All(y => x.Name != y.Name)).ToList();
            allServices = allServices.Union(updatedServices).ToList();
            allServices = allServices.Union(newServices).ToList();
            await _localStorage.SaveServiceInfosAsync(allServices);
            var servicesWithMetadata = await GetSamplesDetailsAsync(allServices);
            foreach (var serviceInfo in servicesWithMetadata)
            {
                await _localStorage.SaveServiceDetailsAsync(serviceInfo);
            }
        }

        public static IEnumerable<ServiceInfo> ParseSamplesXml(string xml)
        {
            XElement element = XElement.Parse(xml);
            var samples = element.Elements("Service").Select(ServiceInfo.Parse);
            return samples;
        }

        private async Task<IEnumerable<ServiceInfo>> GetSamplesDetailsAsync(IEnumerable<ServiceInfo> serviceInfos)
        {
            var samplesDetails = new List<ServiceInfo>();
            foreach (var serviceInfo in serviceInfos)
            {
                var serviceDetails = serviceInfo;
                serviceDetails.MetadataCache = await _resourceManager.LoadResourceAsStringAsync(
                    _moduleName, "Metadata", serviceInfo.MetadataCacheFilename);
                serviceDetails.ImageBase64 = await _resourceManager.LoadResourceAsStringAsync(
                    _moduleName, "ImagesBase64", serviceInfo.ImageBase64Filename);
                serviceDetails.CacheUpdated = new DateTimeOffset(SampleCreationTime);
                samplesDetails.Add(serviceDetails);
            }
            return samplesDetails;
        }
    }
}