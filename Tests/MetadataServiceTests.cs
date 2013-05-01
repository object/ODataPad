using ODataPad.Core.Services;

#if NET45
using Microsoft.VisualStudio.TestTools.UnitTesting;
#elif NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#elif WINDOWS_PHONE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#endif

namespace ODataPad.Tests
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