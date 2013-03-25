using Cirrious.MvvmCross.Application;
using Cirrious.MvvmCross.ExtensionMethods;
using Cirrious.MvvmCross.WinRT.Platform;
using ODataPad.Core;
using ODataPad.Core.Interfaces;
using ODataPad.Platform.WinRT;
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

            this.RegisterServiceInstance<IResourceManager>(new ResourceManager());
            this.RegisterServiceInstance<IServiceLocalStorage>(new ServiceLocalStorage());
            this.RegisterServiceInstance<IApplicationLocalData>(new ApplicationLocalData());
            this.RegisterServiceInstance<IImageProvider>(new ImageProvider());
            this.RegisterServiceInstance<IResultProvider>(new ResultProvider());
        }

        protected override MvxApplication CreateApp()
        {
            return new ODataPadApp("Samples", "SampleServices.xml");
        }
    }
}