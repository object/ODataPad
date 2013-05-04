using System.Reflection;
using Android.App;
using MonoDroidUnitTesting;

namespace ODataPad.Tests.Droid
{
    /// <summary>
    /// Example application for running some unit tests.
    /// </summary>
    [Activity(Label = "ODataPad MonoDroid Tests", MainLauncher = true, Icon = "@drawable/icon")]
    public class TestActivity : GuiTestRunnerActivity
    {
        protected override TestRunner CreateTestRunner()
        {
            var runner = new TestRunner();
            // Run all tests from this assembly
            runner.AddTests(Assembly.GetExecutingAssembly());
            return runner;
        }
    }
}
