using System.Windows.Threading;
using Cirrious.MvvmCross.Application;
using Cirrious.MvvmCross.ExtensionMethods;
using Cirrious.MvvmCross.Wpf.Interfaces;
using Cirrious.MvvmCross.Wpf.Platform;
using ODataPad.Core;
using ODataPad.Core.Interfaces;
using ODataPad.Platform.Net45;

namespace ODataPad.UI.Net45
{
    public class Setup : MvxBaseWpfSetup
    {
        public Setup(Dispatcher dispatcher, IMvxWpfViewPresenter presenter)
            : base(dispatcher, presenter)
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

        protected override void InitializeDefaultTextSerializer()
        {
            Cirrious.MvvmCross.Plugins.Json.PluginLoader.Instance.EnsureLoaded(true);
        }
    }
}
