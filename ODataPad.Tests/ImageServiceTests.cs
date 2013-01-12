using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using ODataPad.WinRT;

namespace ODataPad.Tests.WinRT
{
    [TestClass]
    public class ImageServiceTests
    {
        [TestMethod]
        public async Task GetImage()
        {
            var imageService = new ImageService();
            object image = null;
            await Utils.ExecuteOnUIThread(() => { image = imageService.GetImage("Samples/OData.org.png"); });
            Assert.IsNotNull(image);
        }
    }
}