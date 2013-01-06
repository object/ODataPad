using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using ODataPad.Core.Models;

namespace ODataPad.Core.Services
{
    public class SamplesService : ISamplesService
    {
        private readonly IResourceLoader _resourceLoader;
        private readonly string _samplesFolder;
        private readonly string _samplesFilename;
        private readonly int _currentAppVersion;
        private readonly int _desiredAppVersion;

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

        private static readonly DateTime SampleCreationTime = new DateTime(2012, 11, 10);

        public SamplesService(IResourceLoader resourceLoader, 
            string samplesFolder, string samplesFilename,
            int currentAppVersion, int desiredAppVersion)
        {
            _resourceLoader = resourceLoader;
            _samplesFolder = samplesFolder;
            _samplesFilename = samplesFilename;
            _currentAppVersion = currentAppVersion;
            _desiredAppVersion = desiredAppVersion;
        }

        public async Task<IEnumerable<ODataServiceInfo>> GetAllSamplesAsync()
        {
            var xml = await _resourceLoader.LoadResourceFileAsString(_samplesFolder, _samplesFilename);
            var allSamples = ParseSamplesXml(xml);

            var samples = NewSamples.Where(sample => sample.Key > _desiredAppVersion)
                .Aggregate(allSamples, (current, sample) => current.Where(x => !sample.Value.Contains(x.Name)));
            samples = ExpiredSample.Where(sample => sample.Key <= _desiredAppVersion)
                .Aggregate(samples, (current, sample) => current.Where(x => !sample.Value.Contains(x.Name)));
            var allSamplesWithMetadata = await GetSamplesMetadataAsync(samples);

            return allSamplesWithMetadata;
        }

        public async Task<IEnumerable<ODataServiceInfo>> GetNewSamplesAsync()
        {
            var xml = await _resourceLoader.LoadResourceFileAsString(_samplesFolder, _samplesFilename);
            var allSamples = ParseSamplesXml(xml);

            var newSamples = NewSamples
                .Where(sample => sample.Key > _currentAppVersion && sample.Key <= _desiredAppVersion)
                .SelectMany(x => allSamples.Where(y => x.Value.Contains(y.Name)));
            var newSamplesWithMetadata = await GetSamplesMetadataAsync(newSamples);

            return newSamplesWithMetadata;
        }

        public async Task<IEnumerable<ODataServiceInfo>> GetUpdatedSamplesAsync()
        {
            var xml = await _resourceLoader.LoadResourceFileAsString(_samplesFolder, _samplesFilename);
            var allSamples = ParseSamplesXml(xml);

            var updatedSamples = UpdatedSamples
                .Where(sample => sample.Key > _currentAppVersion && sample.Key <= _desiredAppVersion)
                .SelectMany(x => allSamples.Where(y => x.Value.Contains(y.Name)));
            var updatedSamplesWithMetadata = await GetSamplesMetadataAsync(updatedSamples);

            return updatedSamplesWithMetadata;
        }

        public async Task<IEnumerable<ODataServiceInfo>> GetExpiredSamplesAsync()
        {
            var xml = await _resourceLoader.LoadResourceFileAsString(_samplesFolder, _samplesFilename);
            var allSamples = ParseSamplesXml(xml);

            var expiredSamples = ExpiredSample
                .Where(sample => sample.Key > _currentAppVersion && sample.Key <= _desiredAppVersion)
                .SelectMany(x => allSamples.Where(y => x.Value.Contains(y.Name)));
            return expiredSamples;
        }

        private IEnumerable<ODataServiceInfo> ParseSamplesXml(string xml)
        {
            XElement element = XElement.Parse(xml);
            var samples = element.Elements("Service").Select(ODataServiceInfo.Parse);
            return samples;
        }

        private async Task<IEnumerable<ODataServiceInfo>> GetSamplesMetadataAsync(IEnumerable<ODataServiceInfo> serviceInfos)
        {
            var samplesWithMetadata = new List<ODataServiceInfo>();
            foreach (var serviceInfo in serviceInfos)
            {
                var serviceInfoWithMetadata = serviceInfo;
                serviceInfoWithMetadata.MetadataCache = await _resourceLoader.LoadResourceFileAsString(
                    _samplesFolder, serviceInfo.MetadataCacheFilename);
                serviceInfoWithMetadata.CacheUpdated = new DateTimeOffset(SampleCreationTime);
                samplesWithMetadata.Add(serviceInfoWithMetadata);
            }
            return samplesWithMetadata;
        }
    }
}