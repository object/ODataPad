using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

#if NET45
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ODataPad.Platform.Net45;
#elif NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using ODataPad.Platform.WinRT;
#elif WINDOWS_PHONE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using ODataPad.Platform.WP8;
#endif

namespace ODataPad.Tests
{
    [TestClass]
    public class ResourceManagerTests
    {
        [TestInitialize]
        public void TestInitialize()
        {
#if NET45
            if (!UriParser.IsKnownScheme("pack"))
                new System.Windows.Application();
#endif
        }

        [TestMethod]
        public async Task LoadContentAsStringAsync()
        {
            var resourceManager = new ResourceManager();
            var text = await resourceManager.LoadContentAsStringAsync("Samples", "ContentSampleServices.xml");
            Assert.IsNotNull(text);
        }

        [TestMethod]
        public async Task LoadResourceAsStringAsync()
        {
            const string moduleName =
#if NET45
            "ODataPad.Tests.Net45";
#elif NETFX_CORE
            "ODataPad.Tests.WinRT";
#elif WINDOWS_PHONE
            "ODataPad.Tests.WP8";
#endif
            var resourceManager = new ResourceManager();
            var text = await resourceManager.LoadResourceAsStringAsync(
                moduleName, "Samples", "EmbeddedSampleServices.xml");
            Assert.IsNotNull(text);
        }
    }
}
