using System;
using System.Threading.Tasks;
using Cirrious.CrossCore.IoC;
using ODataPad.Core.Interfaces;

namespace ODataPad.Core.Services
{
    public class DataVersioningService
        : IDataVersioningService
    {
        private readonly IServiceRepository _serviceRepository;

        public DataVersioningService(IServiceRepository serviceRepository = null)
        {
            _serviceRepository = serviceRepository ?? Mvx.Resolve<IServiceRepository>();
        }

        public async Task SetDataVersionAsync(int currentVersion, int requestedVersion)
        {
            if (currentVersion < 0 || requestedVersion < 0)
                throw new InvalidOperationException("Current and requested data versions must be set prior to calling data versioning operations");

            var samplesService = new SamplesService(ODataPadApp.SamplesFolder, ODataPadApp.SamplesFilename, currentVersion, requestedVersion);
            if (currentVersion != requestedVersion)
            {
                if (requestedVersion <= 1)
                {
                    await _serviceRepository.ClearServicesAsync();
                }
                else
                {
                    await samplesService.UpdateSamplesAsync();
                }
            }
        }
    }
}