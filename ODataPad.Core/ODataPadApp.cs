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

        public HomeViewModel HomeViewModel { get; private set; }

        private void InitalizeServices()
        {
            this.RegisterServiceInstance<IServiceRepository>(new ServiceRepository());
        }

        private void InitializeStartNavigation()
        {
            var startApplicationObject = new StartNavigation();
            this.RegisterServiceInstance<IMvxStartNavigation>(startApplicationObject);
        }

        private void InitializePlugIns()
        {
        }

        public async Task<bool> SetVersionAsync(int currentVersion, int requestedVersion)
        {
            this.CurrentVersion = currentVersion;
            this.RequestedVersion = requestedVersion;

            if (this.CurrentVersion != this.RequestedVersion)
            {
                var samplesService = new SamplesService(
                    this.ResourceManager, _samplesFolder, _samplesFilename,
                    this.CurrentVersion, this.RequestedVersion);

                if (this.RequestedVersion <= 1)
                {
                    await this.ServiceRepository.ClearServicesAsync();
                }
                else
                {
                    await samplesService.UpdateSamplesAsync(this.ServiceLocalStorage);
                }

                this.CurrentVersion = this.RequestedVersion;
            }

            return true;
        }
    }
}