using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using ODataPad.WinRT;

namespace ODataPad.Tests.WinRT
{
    [TestClass]
    public class ResourceManagerTests
    {
        [TestMethod]
        public async Task LoadContentAsStringAsync()
        {
            var resourceLoader = new ResourceManager();
            var text = await resourceLoader.LoadContentAsStringAsync("Samples", "ContentSampleServices.xml");
            Assert.IsNotNull(text);
        }

        [TestMethod]
        public async Task LoadResourceAsStringAsync()
        {
            var resourceLoader = new ResourceManager();
            var text = await resourceLoader.LoadResourceAsStringAsync("ODataPad.Tests.WinRT", "Samples", "EmbeddedSampleServices.xml");
            Assert.IsNotNull(text);
        }

        [TestMethod]
        public void GetImageResourcePath()
        {
            var resourceLoader = new ResourceManager();
            var path = resourceLoader.GetImageResourcePath("Samples", "OData.org.png");
            Assert.AreEqual("ms-appx:///Samples/OData.org.png", path);
        }
    }
}
