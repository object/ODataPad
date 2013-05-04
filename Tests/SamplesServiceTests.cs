using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ODataPad.Core.Models;
using ODataPad.Core.Services;
using ODataPad.Samples;

#if NET45
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ODataPad.Platform.Net45;
#elif NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using ODataPad.Platform.WinRT;
#elif WINDOWS_PHONE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using ODataPad.Platform.WP8;
#elif MonoDroid
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ODataPad.Platform.Droid;
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
            _localStorage.ClearServiceInfosAsync().Wait();
        }

        [TestMethod]
#if !MonoDroid
        public async Task UpdateSamplesAsync_from_1_to_1()
#else
        public void UpdateSamplesAsync_from_1_to_1()
#endif
        {
#if !MonoDroid
            var services = await UpdateSamplesAsync(1, 1);
#else
            var services = UpdateSamplesAsync(1, 1).Result;
#endif

            Assert.AreEqual(0, services.Count());
        }

        [TestMethod]
#if !MonoDroid
        public async Task UpdateSamplesAsync_from_1_to_2()
#else
        public void UpdateSamplesAsync_from_1_to_2()
#endif
        {
#if !MonoDroid
            var services = await UpdateSamplesAsync(1, 2);
#else
            var services = UpdateSamplesAsync(1, 2).Result;
#endif
            Assert.AreEqual(10, services.Count());
            Assert.IsTrue(services.Any(x => x.Name == "DBpedia"));
            Assert.IsTrue(services.All(x => x.Name != "Pluralsight"));
            Assert.AreEqual(0, services.Single(x => x.Name == "OData.org").Index);
            Assert.AreNotEqual(0, services.Single(x => x.Name == "Stack Overflow").Index);
            var service = services.Single(x => x.Name == "OData.org");
#if !MonoDroid
            await
#endif
            _localStorage.LoadServiceDetailsAsync(service);
            Assert.IsNotNull(service.MetadataCache);
            Assert.IsNotNull(service.ImageBase64);
        }

        [TestMethod]
#if !MonoDroid
        public async Task UpdateSamplesAsync_from_2_to_3()
#else
        public void UpdateSamplesAsync_from_2_to_3()
#endif
        {
#if !MonoDroid
            var services = await UpdateSamplesAsync(2, 3);
#else
            var services = UpdateSamplesAsync(2, 3).Result;
#endif
            Assert.AreEqual(10, services.Count());
            Assert.IsTrue(services.All(x => x.Name != "DBpedia"));
            Assert.IsTrue(services.Any(x => x.Name == "Pluralsight"));
            Assert.AreEqual(0, services.Single(x => x.Name == "OData.org").Index);
            Assert.AreNotEqual(0, services.Single(x => x.Name == "Stack Overflow").Index);
            var service = services.Single(x => x.Name == "OData.org");
#if !MonoDroid
            await 
#endif
            _localStorage.LoadServiceDetailsAsync(service);
            Assert.IsNotNull(service.MetadataCache);
            Assert.IsNotNull(service.ImageBase64);
        }

        private
#if !MonoDroid
            async
#endif
            Task<IEnumerable<ServiceInfo>> UpdateSamplesAsync(int currentVersion, int requestedVersion)
        {
            SamplesService samplesService;
            for (int oldVersion = 1; oldVersion < currentVersion; oldVersion++)
            {
                samplesService = new SamplesService(
                    typeof(ODataPad.Samples.DesignData).Namespace, "SampleServices.xml",
                    new ResourceManager(), new ServiceLocalStorage());
#if !MonoDroid
                await 
#endif
                samplesService.UpdateSamplesAsync(oldVersion, oldVersion + 1);
            }

            samplesService = new SamplesService(
                typeof(ODataPad.Samples.DesignData).Namespace, "SampleServices.xml",
                new ResourceManager(), new ServiceLocalStorage());
#if !MonoDroid
            await 
#endif
            samplesService.UpdateSamplesAsync(currentVersion, requestedVersion);

            return
#if !MonoDroid
                await 
#endif
                _localStorage.LoadServiceInfosAsync();
        }
    }
}