using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            table.CompareToSet(_viewModelDriver.Home.Services.Items.Select(x => new { x.Name }));
        }

        [When(@"I select service (.*)")]
        [Given(@"selected service is (.*)")]
        public void c(string serviceName)
        {
            _viewModelDriver.SelectService(serviceName);
        }

        [Given(@"no service is selected")]
        public void d()
        {
            _viewModelDriver.Home.Services.SelectedService = null;
        }

        [Then(@"I should see service information")]
        public void e(Table table)
        {
            Assert.AreEqual(table.Rows[0]["Name"], _viewModelDriver.AppState.ActiveService.Name);
            Assert.AreEqual(table.Rows[0]["URL"], _viewModelDriver.AppState.ActiveService.Url);
        }

        [Then(@"I should see service collections")]
        public void f(Table table)
        {
            table.CompareToSet(_viewModelDriver.AppState.ActiveService.ResourceSets.Items.Select(x => new { x.Name }));
        }

        [Then(@"I shouldn't see collection details")]
        public void g()
        {
            Assert.IsNull(_viewModelDriver.AppState.ActiveResourceSet);
        }

        [Given(@"collection details mode is set to (.*)")]
        public void h(string mode)
        {
            _viewModelDriver.SelectResourceSetMode(mode);
        }

        [When(@"I select collection (.*)")]
        [Given(@"selected collection is (.*)")]
        public void i(string collectionName)
        {
            _viewModelDriver.SelectResourceSet(collectionName);
        }

        [Then(@"I should see collection schema summary ""(.*)""")]
        public void j(string summary)
        {
            Assert.AreEqual(_viewModelDriver.AppState.ResourceSetModes.First(), _viewModelDriver.AppState.ActiveResourceSetMode);
            var schemaSummary = _viewModelDriver.AppState.ActiveResourceSet.Summary;
            Assert.AreEqual(summary, schemaSummary);
        }

        [Then(@"I should see collection data rows that contain")]
        public void k(Table table)
        {
            var results = _viewModelDriver.AppState.ActiveResourceSet.Results.QueryResults;

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
        public void l(string key)
        {
            _viewModelDriver.SelectResult(key);
        }

        [Given(@"result view shows details for a row with key ""(.*)""")]
        public void m(string key)
        {
            _viewModelDriver.SelectResult(key);
        }

        [Then(@"I should see result details that contain")]
        public void n(Table table)
        {
            var details = ParseResultSummary(_viewModelDriver.AppState.ActiveResult.Text);
            var expectedDetails = table.Rows.SelectMany(x => x.Values).ToList();

            Assert.AreEqual(expectedDetails.Count(), details.Count());
            for (var index = 0; index < details.Count(); index++)
            {
                AssertMatch(expectedDetails[index], details[index]);
            }
        }

        [When(@"I tap within result view")]
        public void o()
        {
            _viewModelDriver.AppState.ActiveResourceSet.Results.SelectedResult = null;
        }

        [Then(@"I should not see result details")]
        public void p()
        {
            Assert.IsFalse(_viewModelDriver.AppState.ActiveResourceSet.Results.IsSingleResultSelected);
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
