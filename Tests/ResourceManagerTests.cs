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
#elif MonoDroid
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ODataPad.Platform.Droid;
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
#if !MonoDroid
        public async Task LoadContentAsStringAsync()
#else
        public void LoadContentAsStringAsync()
#endif
        {
            var resourceManager = new ResourceManager();
            var text = 
#if !MonoDroid
                await 
#endif
                resourceManager.LoadContentAsStringAsync("Samples", "ContentSampleServices.xml");
            Assert.IsNotNull(text);
        }

        [TestMethod]
#if !MonoDroid
        public async Task LoadResourceAsStringAsync()
#else
        public void LoadResourceAsStringAsync()
#endif
        {
            const string moduleName =
#if NET45
            "ODataPad.Tests.Net45";
#elif NETFX_CORE
            "ODataPad.Tests.WinRT";
#elif WINDOWS_PHONE
            "ODataPad.Tests.WP8";
#elif MonoDroid
            "ODataPad.Tests.Droid";
#endif
            var resourceManager = new ResourceManager();
            var text =
#if !MonoDroid
                await 
#endif
                resourceManager.LoadResourceAsStringAsync(
                moduleName, "Samples", "EmbeddedSampleServices.xml");
            Assert.IsNotNull(text);
        }
    }
}
