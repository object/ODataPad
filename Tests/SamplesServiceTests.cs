using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ODataPad.Core.Models;
using ODataPad.Core.Services;
#if NET45
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ODataPad.Platform.Net45;
#elif NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using ODataPad.Platform.WinRT;
#endif

namespace ODataPad.Tests
{
    [TestClass]
    public class SampesServiceTests
    {
        private ServiceLocalStorage _localStorage;

        [TestInitialize]
        public void TestInitialize()
        {
            _localStorage = new ServiceLocalStorage();
            _localStorage.ClearServicesAsync().Wait();
        }

        [TestMethod]
        public async Task UpdateSamplesAsync_from_1_to_1()
        {
            var services = await UpdateSamplesAsync(1, 1);
            Assert.AreEqual(0, services.Count());
        }

        [TestMethod]
        public async Task UpdateSamplesAsync_from_1_to_2()
        {
            var services = await UpdateSamplesAsync(1, 2);
            Assert.AreEqual(3, services.Count());
            Assert.IsTrue(services.Any(x => x.Name == "DBpedia"));
            Assert.IsTrue(services.All(x => x.Name != "Pluralsight"));
            Assert.AreEqual(0, services.Single(x => x.Name == "OData.org").Index);
            Assert.AreNotEqual(0, services.Single(x => x.Name == "Stack Overflow").Index);
            var filename = services.Single(x => x.Name == "OData.org").MetadataCacheFilename;
            var metadata = await _localStorage.LoadServiceMetadataAsync(filename);
            Assert.IsNotNull(metadata);
        }

        [TestMethod]
        public async Task UpdateSamplesAsync_from_2_to_3()
        {
            var services = await UpdateSamplesAsync(2, 3);
            Assert.AreEqual(3, services.Count());
            Assert.IsTrue(services.All(x => x.Name != "DBpedia"));
            Assert.IsTrue(services.Any(x => x.Name == "Pluralsight"));
            Assert.AreEqual(0, services.Single(x => x.Name == "OData.org").Index);
            Assert.AreNotEqual(0, services.Single(x => x.Name == "Stack Overflow").Index);
            var filename = services.Single(x => x.Name == "OData.org").MetadataCacheFilename;
            var metadata = await _localStorage.LoadServiceMetadataAsync(filename);
            Assert.IsNotNull(metadata);
        }

        private async Task<IEnumerable<ServiceInfo>> UpdateSamplesAsync(int currentVersion, int requestedVersion)
        {
            SamplesService samplesService;
            for (int oldVersion = 1; oldVersion < currentVersion; oldVersion++)
            {
                samplesService = new SamplesService(
                    "Samples", "SampleServices.xml",
                    oldVersion, oldVersion + 1,
                    new ResourceManager(), new ServiceLocalStorage());
                await samplesService.UpdateSamplesAsync();
            }

            samplesService = new SamplesService(
                "Samples", "SampleServices.xml",
                currentVersion, requestedVersion,
                new ResourceManager(), new ServiceLocalStorage());
            await samplesService.UpdateSamplesAsync();

            return await _localStorage.LoadServiceInfosAsync();
        }
    }
}