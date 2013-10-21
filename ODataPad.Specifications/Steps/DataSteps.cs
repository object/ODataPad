using System.Linq;
using System.Threading;
using Cirrious.MvvmCross.ViewModels;
using ODataPad.Core;
using ODataPad.Core.ViewModels;
using ODataPad.Specifications.Infrastructure;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace ODataPad.Specifications.Steps
{
    [Binding]
    public class DataSteps
    {
        [Given(@"Service repository has no entries")]
        public void a()
        {
            AppDriver.Instance.ClearData();
        }

        [Given(@"Service repository has following entries")]
        public void b(Table table)
        {
            AppDriver.Instance.CreateODataServices(table.Rows.Select(x => x[0]));
        }

        [Given(@"Service repository was created by the previous program version")]
        public void c()
        {
            AppDriver.Instance.SetDataVersion(2);
        }
    }
}
