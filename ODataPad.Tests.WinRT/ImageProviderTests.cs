using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using ODataPad.Platform.WinRT;

namespace ODataPad.Tests.WinRT
{
    [TestClass]
    public class ImageProviderTests
    {
        [TestMethod]
        public async Task GetImage()
        {
            var imageProvider = new ImageProvider();
            object image = null;
            await Utils.ExecuteOnUIThread(() => { image = imageProvider.GetImage("Samples/OData.org.png"); });
            Assert.IsNotNull(image);
        }
    }
}