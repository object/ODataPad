using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ODataPad.Core.Models;
using ODataPad.Core.Services;

namespace ODataPad.Core
{
    public class ODataPadApp
    {
        private IServiceLocalStorage _localStorage;
        private IResourceManager _resourceManager;
        public ServiceRepository ServiceRepository  { get; private set; }
        private string _samplesFolder;
        private string _samplesFilename;

        public int CurrentVersion { get; private set; }
        public int RequestedVersion { get; private set; }

        public ODataPadApp(
            IServiceLocalStorage localStorage, 
            IResourceManager resourceManager,
            string samplesFolder,
            string samplesFilename)
        {
            _localStorage = localStorage;
            _resourceManager = resourceManager;
            this.ServiceRepository = new ServiceRepository(_localStorage);
            _samplesFolder = samplesFolder;
            _samplesFilename = samplesFilename;
        }

        public async Task<IEnumerable<ServiceInfo>> InitializeODataServicesAsync()
        {
            return await this.ServiceRepository.LoadServicesAsync();
        }

        public async Task<bool> SetVersionAsync(int currentVersion, int requestedVersion)
        {
            this.CurrentVersion = currentVersion;
            this.RequestedVersion = requestedVersion;

            var samplesService = new SamplesService(
                _resourceManager, _samplesFolder, _samplesFilename, 
                this.CurrentVersion, this.RequestedVersion);

            if (this.RequestedVersion == 1)
            {
                await this.ServiceRepository.ClearServicesAsync();
            }
            else if (this.CurrentVersion <= 1 && this.RequestedVersion > 1)
            {
                await samplesService.CreateSamplesAsync(_localStorage);
            }
            else if (this.CurrentVersion == 2 && this.RequestedVersion > 2)
            {
                await samplesService.UpdateSamplesAsync(_localStorage);
            }

            this.CurrentVersion = this.RequestedVersion;
            return true;
        }
    }
}