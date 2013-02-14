using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
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