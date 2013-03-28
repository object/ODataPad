using System;
using System.Threading.Tasks;
using Cirrious.MvvmCross.ExtensionMethods;
using Cirrious.MvvmCross.Interfaces.ServiceProvider;
using ODataPad.Core.Interfaces;

namespace ODataPad.Core.Services
{
    public class DataVersioningService
        : IDataVersioningService
        , IMvxServiceConsumer<IServiceRepository>
        , IMvxServiceConsumer<ISamplesService>
    {
        private readonly IServiceRepository _serviceRepository;

        public DataVersioningService(IServiceRepository serviceRepository = null)
        {
            _serviceRepository = serviceRepository ?? this.GetService<IServiceRepository>();
        }

        public async Task SetDataVersionAsync(int currentVersion, int requestedVersion)
        {
            if (currentVersion < 0 || requestedVersion < 0)
                throw new InvalidOperationException("Current and requested data versions must be set prior to calling data versioning operations");

            var ssamplesService = new SamplesService(ODataPadApp.SamplesFolder, ODataPadApp.SamplesFilename, currentVersion, requestedVersion);
            if (currentVersion != requestedVersion)
            {
                if (requestedVersion <= 1)
                {
                    await _serviceRepository.ClearServicesAsync();
                }
                else
                {
                    await ssamplesService.UpdateSamplesAsync();
                }
            }
        }
    }
}