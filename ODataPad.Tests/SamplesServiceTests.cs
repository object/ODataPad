using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using ODataPad.Core.Models;
using ODataPad.Core.Services;
using ODataPad.WinRT;

namespace ODataPad.Tests.WinRT
{
    [TestClass]
    public class SampesServiceTests
    {
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
        }

        [TestMethod]
        public async Task UpdateSamplesAsync_from_2_to_3()
        {
            var services = await UpdateSamplesAsync(2, 3);
            Assert.AreEqual(3, services.Count());
            Assert.IsTrue(services.All(x => x.Name != "DBpedia"));
            Assert.IsTrue(services.Any(x => x.Name == "Pluralsight"));
        }

        private async Task<IEnumerable<ServiceInfo>> UpdateSamplesAsync(int currentVersion, int requestedVersion)
        {
            var localStorage = new ServiceLocalStorage();
            var resourceManager = new ResourceManager();

            await localStorage.ClearServicesAsync();

            SamplesService samplesService;
            for (int oldVersion = 1; oldVersion < currentVersion; oldVersion++)
            {
                samplesService = new SamplesService(resourceManager,
                    "Samples", "SampleServices.xml",
                    oldVersion, oldVersion+1);
                await samplesService.UpdateSamplesAsync(localStorage);
            }

            samplesService = new SamplesService(resourceManager, 
                "Samples", "SampleServices.xml", 
                currentVersion, requestedVersion);
            await samplesService.UpdateSamplesAsync(localStorage);

            return await localStorage.LoadServiceInfosAsync();
        }
    }
}