using System.Linq;
using ODataPad.Core.ViewModels;
using ODataPad.Specifications.Infrastructure;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace ODataPad.Specifications.Steps
{
    [Binding]
    public class ViewSteps
    {
        [Given(@"I see a list of services")]
        public void a()
        {
            AppDriver.Instance.EnsureHomeViewModel();
        }

        [Then(@"I should see a list of services")]
        public void b(Table table)
        {
            var viewModel = GetHomeViewModel();
            table.CompareToSet(viewModel.Services.Select(x => new { x.Name }));
        }

        [When(@"I select service (.*)")]
        public void c(string serviceName)
        {
            var viewModel = GetHomeViewModel();
            new ViewModelDriver().SelectService(viewModel, serviceName);
        }

        [Then(@"I should see service collections")]
        public void d(Table table)
        {
            var viewModel = GetHomeViewModel();
            table.CompareToSet(viewModel.Resources.Select(x => new { x.Name }));
        }

        [Given(@"selected service is (.*)")]
        public void e(string serviceName)
        {
            var viewModel = GetHomeViewModel();
            new ViewModelDriver().SelectService(viewModel, serviceName);
        }

        [Given(@"collections are set to show its (.*)")]
        public void f(string mode)
        {
            ScenarioContext.Current.Pending();
        }

        [When(@"I select collection (.*)")]
        public void g(string mode)
        {
            ScenarioContext.Current.Pending();
        }

        private HomeViewModel GetHomeViewModel()
        {
            return ScenarioContext.Current["HomeViewModel"] as HomeViewModel;
        }
    }
}