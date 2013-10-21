using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cirrious.MvvmCross.Platform;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ODataPad.Core.ViewModels;
using ODataPad.Specifications.Infrastructure;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace ODataPad.Specifications.Steps
{
    [Binding]
    public class ViewSteps
    {
        private readonly ViewModelDriver _viewModelDriver;
        private const string Ellipsis = " (...)";

        public ViewSteps()
        {
            _viewModelDriver = new ViewModelDriver();
        }

        [Given(@"I see a list of services")]
        public void a()
        {
            AppDriver.Instance.EnsureHomeViewModel();
        }

        [Then(@"I should see a list of services")]
        public void b(Table table)
        {
            var viewModel = GetHomeViewModel();
            table.CompareToSet(viewModel.Services.Items.Select(x => new { x.Name }));
        }

        [When(@"I select service (.*)")]
        public void c(string serviceName)
        {
            var viewModel = GetHomeViewModel();
            _viewModelDriver.SelectService(viewModel.Services, serviceName);
        }

        [Then(@"I should see service collections")]
        public void d(Table table)
        {
            var viewModel = GetHomeViewModel();
            table.CompareToSet(viewModel.Services.SelectedService.ResourceSets.Items.Select(x => new { x.Name }));
        }

        [Given(@"selected service is (.*)")]
        public void e(string serviceName)
        {
            var viewModel = GetHomeViewModel();
            _viewModelDriver.SelectService(viewModel.Services, serviceName);
        }

        [Given(@"collections are set to show its (.*)")]
        public void f(string mode)
        {
            var viewModel = GetHomeViewModel();
            _viewModelDriver.SelectResourceSetMode(viewModel, mode);
        }

        [When(@"I select collection (.*)")]
        [Given(@"selected collection is (.*)")]
        public void g(string collectionName)
        {
            var viewModel = GetHomeViewModel();
            _viewModelDriver.SelectResourceSet(viewModel.Services.SelectedService.ResourceSets, collectionName);
            
            if (viewModel.SelectedResourceSetMode == viewModel.ResourceSetModes.Last())
                WaitForResults(viewModel.Services.SelectedService.ResourceSets.SelectedItem.Results, 5);
        }

        [Then(@"I should see collection schema summary ""(.*)""")]
        public void h(string summary)
        {
            var viewModel = GetHomeViewModel();
            Assert.AreEqual(viewModel.ResourceSetModes.First(), viewModel.SelectedResourceSetMode);
            var schemaSummary = _viewModelDriver.GetSchemaSummary(viewModel.Services.SelectedService.ResourceSets.SelectedItem);
            Assert.AreEqual(summary, schemaSummary);
        }

        [Then(@"I should see collection data rows that contain")]
        public void i(Table table)
        {
            var viewModel = GetHomeViewModel();
            var results = _viewModelDriver.GetQueryResults(viewModel.Services.SelectedService.ResourceSets.SelectedItem.Results);

            Func<string> messageFunc = null;
            try
            {
                var resultKeys = results.Select(x => x.Properties["Id"].ToString()).ToList();
                messageFunc = () => string.Format("Failed to find property with key {0}", table.Rows.First(x => !resultKeys.Contains(x[0]))[0]);
                Assert.IsTrue(table.Rows.All(x => resultKeys.Contains(x[0])));

                var resultData = results.Select(x => string.Join(string.Empty, x.Properties.Select(y => y.Value == null ? string.Empty : y.Value.ToString()))).ToList();
                messageFunc = () => string.Format("Failed to find data {0}", table.Rows.First(x => !resultData.Any(y => y.Contains(RemoveEllipsis(x[1]))))[1]);
                Assert.IsTrue(table.Rows.All(x => resultData.Any(y => y.Contains(RemoveEllipsis(x[1])))));
            }
            catch (AssertFailedException)
            {
                Assert.Fail(messageFunc());
            }
        }

        [When(@"I select result row with key ""(.*)""")]
        public void j(string key)
        {
            var viewModel = GetHomeViewModel();
            _viewModelDriver.SelectResult(viewModel.Services.SelectedService.ResourceSets.SelectedItem.Results, key);
        }

        [Given(@"collection data view shows collection data details for a row with key ""(.*)""")]
        public void k(string key)
        {
            var viewModel = GetHomeViewModel();
            _viewModelDriver.SelectResult(viewModel.Services.SelectedService.ResourceSets.SelectedItem.Results, key);
        }

        [Then(@"I should see collection data details that contain")]
        public void l(Table table)
        {
            var viewModel = GetHomeViewModel();
            var details = viewModel.Services.SelectedService.ResourceSets.SelectedItem.Results.SelectedResultDetails
                .Split(new[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries);
            var expectedDetails = table.Rows.SelectMany(x => x.Values).ToList();
            Assert.AreEqual(expectedDetails.Count(), details.Count());
            for (var index = 0; index < details.Count(); index++)
            {
                if (expectedDetails[index].EndsWith(Ellipsis))
                    Assert.IsTrue(details[index].StartsWith(RemoveEllipsis(expectedDetails[index])), string.Format("Expected \"{0}\"", expectedDetails[index]));
                else
                    Assert.AreEqual(expectedDetails[index], details[index]);
            }
        }

        [When(@"I tap within result view")]
        public void m()
        {
            var viewModel = GetHomeViewModel();
            viewModel.Services.SelectedService.ResourceSets.SelectedItem.Results.SelectedResult = null;
        }

        [Then(@"I should not see collection data details")]
        public void n()
        {
            var viewModel = GetHomeViewModel();
            Assert.IsFalse(viewModel.Services.SelectedService.ResourceSets.SelectedItem.Results.IsSingleResultSelected);
        }

        private HomeViewModel GetHomeViewModel()
        {
            return ScenarioContext.Current["HomeViewModel"] as HomeViewModel;
        }

        private void WaitForResults(ResultListViewModel viewModel, int maxSeconds)
        {
            for (var seconds = 0; seconds < maxSeconds; seconds++)
            {
                Thread.Sleep(1000);
                if (viewModel.QueryResults.Any())
                    break;
            }
        }

        private string RemoveEllipsis(string text)
        {
            return text.EndsWith(Ellipsis) ? text.Replace(Ellipsis, string.Empty) : text;
        }
    }
}
