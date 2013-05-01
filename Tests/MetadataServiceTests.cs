#if NET45
using Microsoft.VisualStudio.TestTools.UnitTesting;
#elif NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#endif
using ODataPad.Core.Services;

namespace ODataPad.Tests.WinRT
{
    [TestClass]
    public class MetadataServiceTests
    {
        [TestMethod]
        public void GetSchemaAsString()
        {
            var schemaString = MetadataService.GetSchemaAsString("http://services.odata.org/Website/odata.svc/");
            Assert.IsTrue(schemaString.Contains("Consumers"));
        }
         
    }
}