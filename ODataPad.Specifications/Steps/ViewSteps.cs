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
        private ViewModelDriver _viewModelDriver;
        private const string Ellipsis = " (...)";

        [BeforeScenario]
        public void BeforeScenario()
        {
            _viewModelDriver = new ViewModelDriver();
            ScenarioContext.Current.Add("ViewModelDriver", _viewModelDriver);
        }

        [Given(@"I see a list of services")]
        public void a()
        {
            _viewModelDriver.EnsureHomeViewModel();
        }

        [Then(@"I should see a list of services")]
        public void b(Table table)
        {
            table.CompareToSet(_viewModelDriver.ServiceDetails.Select(x => new { x.Name }));
        }

        [When(@"I select service (.*)")]
        public void c(string serviceName)
        {
            _viewModelDriver.SelectService(serviceName);
        }

        [Then(@"I should see service collections")]
        public void d(Table table)
        {
            table.CompareToSet(_viewModelDriver.ResourceSetDetails.Select(x => new { x.Name }));
        }

        [Given(@"selected service is (.*)")]
        public void e(string serviceName)
        {
            _viewModelDriver.SelectService(serviceName);
        }

        [Given(@"collections are set to show its (.*)")]
        public void f(string mode)
        {
            _viewModelDriver.SelectResourceSetMode(mode);
        }

        [When(@"I select collection (.*)")]
        [Given(@"selected collection is (.*)")]
        public void g(string collectionName)
        {
            _viewModelDriver.SelectResourceSet(collectionName);
        }

        [Then(@"I should see collection schema summary ""(.*)""")]
        public void h(string summary)
        {
            Assert.AreEqual(_viewModelDriver.ResourceSetModes.First(), _viewModelDriver.SelectedResourceSetMode);
            var schemaSummary = _viewModelDriver.SelectedSchemaSummary;
            Assert.AreEqual(summary, schemaSummary);
        }

        [Then(@"I should see collection data rows that contain")]
        public void i(Table table)
        {
            var results = _viewModelDriver.ResultDetails;

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
            _viewModelDriver.SelectResult(key);
        }

        [Given(@"collection data view shows collection data details for a row with key ""(.*)""")]
        public void k(string key)
        {
            _viewModelDriver.SelectResult(key);
        }

        [Then(@"I should see collection data details that contain")]
        public void l(Table table)
        {
            var details = ParseResultSummary(_viewModelDriver.SelectedResultSummary);
            var expectedDetails = table.Rows.SelectMany(x => x.Values).ToList();

            Assert.AreEqual(expectedDetails.Count(), details.Count());
            for (var index = 0; index < details.Count(); index++)
            {
                AssertMatch(expectedDetails[index], details[index]);
            }
        }

        [When(@"I tap within result view")]
        public void m()
        {
            _viewModelDriver.SelectedResultDetails = null;
        }

        [Then(@"I should not see collection data details")]
        public void n()
        {
            Assert.IsFalse(_viewModelDriver.IsSingleResultSelected);
        }

        private static string[] ParseResultSummary(string text)
        {
            return text.Split(new[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries);
        }

        private static void AssertMatch(string expectedText, string actualText)
        {
            if (expectedText.EndsWith(Ellipsis))
                Assert.IsTrue(actualText.StartsWith(RemoveEllipsis(expectedText)), string.Format("Expected \"{0}\"", expectedText));
            else
                Assert.AreEqual(expectedText, actualText);
        }

        private static string RemoveEllipsis(string text)
        {
            return text.EndsWith(Ellipsis) ? text.Replace(Ellipsis, string.Empty) : text;
        }
    }
}
