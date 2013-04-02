using System.Windows.Threading;
using Cirrious.CrossCore.IoC;
using Cirrious.MvvmCross.ViewModels;
using Cirrious.MvvmCross.Wpf.Platform;
using Cirrious.MvvmCross.Wpf.Views;
using ODataPad.Core;
using ODataPad.Core.Interfaces;
using ODataPad.Platform.Net45;

namespace ODataPad.UI.Net45
{
    public class Setup : MvxWpfSetup
    {
        public Setup(Dispatcher dispatcher, IMvxWpfViewPresenter presenter)
            : base(dispatcher, presenter)
        {
        }

        protected override void InitializePlatformServices()
        {
            base.InitializePlatformServices();

            Mvx.RegisterSingleton<IResourceManager>(new ResourceManager());
            Mvx.RegisterSingleton<IServiceLocalStorage>(new ServiceLocalStorage());
            Mvx.RegisterSingleton<IApplicationLocalData>(new ApplicationLocalData());
            Mvx.RegisterSingleton<IImageProvider>(new ImageProvider());
            Mvx.RegisterSingleton<IResultProvider>(new ResultProvider());
        }

        protected override IMvxApplication CreateApp()
        {
            return new ODataPadApp("Samples", "SampleServices.xml");
        }
    }
}
