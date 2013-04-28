using Cirrious.CrossCore.IoC;
using Cirrious.MvvmCross.ViewModels;
using Cirrious.MvvmCross.WindowsStore.Platform;
using ODataPad.Core;
using ODataPad.Core.Interfaces;
using ODataPad.Platform.WinRT;
using Windows.UI.Xaml.Controls;

namespace ODataPad.UI.WinRT
{
    public class Setup : MvxStoreSetup
    {
        public Setup(Frame rootFrame)
            : base(rootFrame)
        {
        }

        protected override void InitializePlatformServices()
        {
            base.InitializePlatformServices();

            Mvx.RegisterSingleton<IResourceManager>(new ResourceManager());
            Mvx.RegisterSingleton<IServiceLocalStorage>(new ServiceLocalStorage());
            Mvx.RegisterSingleton<IApplicationLocalData>(new ApplicationLocalData());
            Mvx.RegisterSingleton<IResultProvider>(new ResultProvider());
        }

        protected override IMvxApplication CreateApp()
        {
            return new ODataPadApp("ODataPad.Samples", "SampleServices.xml");
        }
    }
}