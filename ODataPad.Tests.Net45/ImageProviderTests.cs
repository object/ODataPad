using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ODataPad.Platform.Net45;

namespace ODataPad.Tests.Net45
{
    [TestClass]
    public class ImageProviderTests
    {
        [TestInitialize]
        public void TestInitialize()
        {
            if (!UriParser.IsKnownScheme("pack"))
                new System.Windows.Application();
        }

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