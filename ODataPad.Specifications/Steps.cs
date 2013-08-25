using System.Deployment.Internal;
using System.Linq;
using System.Threading;
using Cirrious.MvvmCross.ViewModels;
using ODataPad.Core;
using ODataPad.Core.ViewModels;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace ODataPad.Specifications
{
    [Binding]
    public class Steps
    {
        private static readonly AppDriver _appDriver = new AppDriver();

        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            _appDriver.Initialize();
        }

        [BeforeScenario]
        public void BeforeScenario()
        {
            _appDriver.ClearData();
            _appDriver.CreateApp();
        }

        [Given(@"Service repository has no entries")]
        public void c()
        {
            _appDriver.ClearData();
        }

        [Given(@"Service repository has following entries")]
        public void d(Table table)
        {
            _appDriver.CreateServices(table.Rows.Select(x => x[0]));
        }

        [Given(@"Service repository was created by the previous program version")]
        public void e()
        {
            _appDriver.SetDataVersion(2);
        }

        [When(@"I start the application")]
        public void a()
        {
            var viewModel = new HomeViewModel();
            ScenarioContext.Current.Add("HomeViewModel", viewModel);
            viewModel.Init(null).Wait();
        }

        [Then(@"I should see a list of services")]
        public void b(Table table)
        {
            var viewModel = ScenarioContext.Current["HomeViewModel"] as HomeViewModel;
            table.CompareToSet(viewModel.Services.Select(x => new { x.Name }));
        }
    }
}
