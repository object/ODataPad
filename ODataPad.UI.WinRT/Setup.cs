using Cirrious.MvvmCross.Application;
using Cirrious.MvvmCross.ExtensionMethods;
using Cirrious.MvvmCross.WinRT.Platform;
using ODataPad.Core;
using ODataPad.Core.Interfaces;
using ODataPad.Core.Services;
using ODataPad.WinRT;
using Windows.UI.Xaml.Controls;

namespace ODataPad.UI.WinRT
{
    public class Setup : MvxBaseWinRTSetup
    {
        public Setup(Frame rootFrame)
            : base(rootFrame)
        {
        }

        protected override void InitializePlatformServices()
        {
            base.InitializePlatformServices();

            this.RegisterServiceInstance<IServiceRepository>(new ServiceRepository());
            this.RegisterServiceInstance<IServiceLocalStorage>(new ServiceLocalStorage());
            this.RegisterServiceInstance<IImageProvider>(new ImageProvider());
        }

        protected override MvxApplication CreateApp()
        {
            return new ODataPadApp("Samples", "SampleServices.xml");
        }
    }
}