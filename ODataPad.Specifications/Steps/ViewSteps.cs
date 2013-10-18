using System;
using System.Linq;
using System.Net.Mime;
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
        private ViewModelDriver _viewModelDriver;

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
                messageFunc = () => string.Format("Failed to find data {0}", table.Rows.First(x => !resultData.Any(y => y.Contains(x[1].Replace("(...)", string.Empty).Trim())))[1]);
                Assert.IsTrue(table.Rows.All(x => resultData.Any(y => y.Contains(x[1].Replace("(...)", string.Empty).Trim()))));
            }
            catch (AssertFailedException)
            {
                Assert.Fail(messageFunc());
            }
        }

        [When(@"I tap on a collection data row with key ""(.*)""")]
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

        private HomeViewModel GetHomeViewModel()
        {
            return ScenarioContext.Current["HomeViewModel"] as HomeViewModel;
        }
    }
}