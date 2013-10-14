using System.Linq;
using System.Threading;
using ODataPad.Core.ViewModels;
using ODataPad.Specifications.Infrastructure;
using TechTalk.SpecFlow;

namespace ODataPad.Specifications.Steps
{
    [Binding]
    public class SharedSteps
    {
        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            AppDriver.Instance.Initialize();
        }

        [BeforeScenario]
        public void BeforeScenario()
        {
            AppDriver.Instance.ClearData();
            AppDriver.Instance.CreateApp();
        }

        [When(@"I start the application")]
        public void a()
        {
            AppDriver.Instance.EnsureHomeViewModel();
        }

        [When(@"I wait (.*) seconds")]
        public void b(int numberOfSeconds)
        {
            Thread.Sleep(1000 * numberOfSeconds);
        }
    }
}
