using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cirrious.MvvmCross.Application;
using Cirrious.MvvmCross.ExtensionMethods;
using Cirrious.MvvmCross.Interfaces.ServiceProvider;
using Cirrious.MvvmCross.Interfaces.ViewModels;
using ODataPad.Core.Interfaces;
using ODataPad.Core.Services;
using ODataPad.Core.ViewModels;

namespace ODataPad.Core
{
    public class ODataPadApp 
        : MvxApplication
        , IMvxServiceProducer
    {
        private string _samplesFolder;
        private string _samplesFilename;

        public int CurrentVersion { get; private set; }
        public int RequestedVersion { get; private set; }

        public ODataPadApp(
            string samplesFolder,
            string samplesFilename)
        {
            _samplesFolder = samplesFolder;
            _samplesFilename = samplesFilename;

            InitalizeServices();
            InitializeStartNavigation();
            InitializePlugIns();
        }

        public IServiceRepository ServiceRepository { get; private set; }
        public IResourceManager ResourceManager { get; private set; }
        public IServiceLocalStorage ServiceLocalStorage { get; private set; }
        public ISamplesService SamplesService { get; private set; }
        public IDataVersioningService DataVersioningService { get; private set; }

        public HomeViewModel HomeViewModel { get; private set; }

        private void InitalizeServices()
        {
            this.RegisterServiceInstance<IServiceRepository>(new ServiceRepository());
        }

        private void InitializeStartNavigation()
        {
            var startApplicationObject = new StartNavigation();
            this.RegisterServiceInstance<IMvxStartNavigation>(
                startApplicationObject);
            this.RegisterServiceInstance<ISamplesService>(
                new SamplesService(_samplesFolder, _samplesFilename, this.CurrentVersion, this.RequestedVersion));
            this.RegisterServiceInstance<IDataVersioningService>(
                new DataVersioningService());
        }

        private void InitializePlugIns()
        {
        }
    }
}