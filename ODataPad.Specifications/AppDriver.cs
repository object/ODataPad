using Cirrious.CrossCore;
using Cirrious.MvvmCross.Test.Core;
using Cirrious.MvvmCross.ViewModels;
using ODataPad.Core;
using ODataPad.Core.Interfaces;
using ODataPad.Core.ViewModels;
using ODataPad.Platform.Net45;

namespace ODataPad.Specifications
{
    public class AppDriver : MvxIoCSupportingTest
    {
        public void Initialize()
        {
            base.ClearAll();
            InitializeServices();
        }

        public IMvxApplication CreateApp()
        {
            return new ODataPadApp("ODataPad.Samples", "SampleServices.xml");
        }

        private void InitializeServices()
        {
            Mvx.RegisterSingleton<IResourceManager>(new ResourceManager());
            Mvx.RegisterSingleton<IServiceLocalStorage>(new ServiceLocalStorage());
            Mvx.RegisterSingleton<IApplicationLocalData>(new ApplicationLocalData());
            Mvx.RegisterSingleton<IResultProvider>(new ResultProvider());
        }
    }
}
