﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Cirrious.MvvmCross.ExtensionMethods;
using Cirrious.MvvmCross.Interfaces.ServiceProvider;
using ODataPad.Core.Interfaces;
using ODataPad.Core.Models;

namespace ODataPad.Core.Services
{
    public class SamplesService
        : ISamplesService
        , IMvxServiceConsumer<IResourceManager>
        , IMvxServiceConsumer<IServiceLocalStorage>
    {
        private readonly string _folderName;
        private readonly string _samplesFilename;
        private int _currentAppVersion;
        private int _requestedAppVersion;

        private static readonly Dictionary<int, List<string>> NewSamples = new Dictionary<int, List<string>>
            {
                {2,  new List<string>()
                         {
                             "DBpedia",
                             "Devexpress Channel",
                             "ebay.org",
                             "nerddinner.org",
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

        public SamplesService(string folderName, string samplesFilename,
                            int currentAppVersion, int requestedAppVersion)
        {
            _folderName = folderName;
            _samplesFilename = samplesFilename;
            _currentAppVersion = currentAppVersion;
            _requestedAppVersion = requestedAppVersion;
        }

        public async Task<IEnumerable<ServiceInfo>> GetAllSamplesAsync()
        {
            var xml = await this.GetService<IResourceManager>().LoadContentAsStringAsync(_folderName, _samplesFilename);
            var allSamples = ParseSamplesXml(xml);

            var samples = NewSamples.Where(sample => sample.Key > _requestedAppVersion)
                .Aggregate(allSamples, (current, sample) => current.Where(x => !sample.Value.Contains(x.Name)));
            samples = ExpiredSample.Where(sample => sample.Key <= _requestedAppVersion)
                .Aggregate(samples, (current, sample) => current.Where(x => !sample.Value.Contains(x.Name)));

            return samples.ToList();
        }

        public async Task<IEnumerable<ServiceInfo>> GetNewSamplesAsync()
        {
            var xml = await this.GetService<IResourceManager>().LoadContentAsStringAsync(_folderName, _samplesFilename);
            var allSamples = ParseSamplesXml(xml);

            var samples = NewSamples
                .Where(sample => sample.Key > _currentAppVersion && sample.Key <= _requestedAppVersion)
                .SelectMany(x => allSamples.Where(y => x.Value.Contains(y.Name)));

            return samples.ToList();
        }

        public async Task<IEnumerable<ServiceInfo>> GetUpdatedSamplesAsync()
        {
            var xml = await this.GetService<IResourceManager>().LoadContentAsStringAsync(_folderName, _samplesFilename);
            var allSamples = ParseSamplesXml(xml);

            var samples = UpdatedSamples
                .Where(sample => sample.Key > _currentAppVersion && sample.Key <= _requestedAppVersion)
                .SelectMany(x => allSamples.Where(y => x.Value.Contains(y.Name)));

            return samples.ToList();
        }

        public async Task<IEnumerable<ServiceInfo>> GetExpiredSamplesAsync()
        {
            var xml = await this.GetService<IResourceManager>().LoadContentAsStringAsync(_folderName, _samplesFilename);
            var allSamples = ParseSamplesXml(xml);

            var samples = ExpiredSample
                .Where(sample => sample.Key > _currentAppVersion && sample.Key <= _requestedAppVersion)
                .SelectMany(x => allSamples.Where(y => x.Value.Contains(y.Name)));

            return samples.ToList();
        }

        public async Task<bool> CreateSamplesAsync()
        {
            var allSamples = await GetAllSamplesAsync();
            var index = 0;
            foreach (var serviceInfo in allSamples)
            {
                serviceInfo.Index = index;
                ++index;
            }
            var result = await this.GetService<IServiceLocalStorage>()
                .SaveServiceInfosAsync(allSamples);
            var samplesWithMetadata = await GetSamplesMetadataAsync(allSamples);
            foreach (var serviceInfo in samplesWithMetadata)
            {
                await this.GetService<IServiceLocalStorage>()
                    .SaveServiceMetadataAsync(serviceInfo.MetadataCacheFilename, serviceInfo.MetadataCache);
            }

            return result;
        }

        public async Task<bool> UpdateSamplesAsync()
        {
            var newServices = await GetNewSamplesAsync();
            var updatedServices = await GetUpdatedSamplesAsync();
            var expiredServices = await GetExpiredSamplesAsync();
            var oldServices = await this.GetService<IServiceLocalStorage>().LoadServiceInfosAsync();

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
            var result = await this.GetService<IServiceLocalStorage>().SaveServiceInfosAsync(allServices);
            var servicesWithMetadata = await GetSamplesMetadataAsync(allServices);
            foreach (var serviceInfo in servicesWithMetadata)
            {
                await this.GetService<IServiceLocalStorage>()
                    .SaveServiceMetadataAsync(serviceInfo.MetadataCacheFilename, serviceInfo.MetadataCache);
            }

            return result;
        }

        private IEnumerable<ServiceInfo> ParseSamplesXml(string xml)
        {
            XElement element = XElement.Parse(xml);
            var samples = element.Elements("Service").Select(ServiceInfo.Parse);
            return samples;
        }

        private async Task<IEnumerable<ServiceInfo>> GetSamplesMetadataAsync(IEnumerable<ServiceInfo> serviceInfos)
        {
            var samplesWithMetadata = new List<ServiceInfo>();
            foreach (var serviceInfo in serviceInfos)
            {
                var serviceInfoWithMetadata = serviceInfo;
                serviceInfoWithMetadata.MetadataCache = await this.GetService<IResourceManager>().LoadContentAsStringAsync(
                    _folderName, serviceInfo.MetadataCacheFilename);
                serviceInfoWithMetadata.CacheUpdated = new DateTimeOffset(SampleCreationTime);
                samplesWithMetadata.Add(serviceInfoWithMetadata);
            }
            return samplesWithMetadata;
        }
    }
}