using System;
using System.Threading.Tasks;
using Cirrious.MvvmCross.ExtensionMethods;
using Cirrious.MvvmCross.Interfaces.ServiceProvider;
using ODataPad.Core.Interfaces;

namespace ODataPad.Core.Services
{
    public class DataVersioningService
        : IDataVersioningService
        , IMvxServiceConsumer<IResourceManager>
        , IMvxServiceConsumer<IServiceRepository>
        , IMvxServiceConsumer<IServiceLocalStorage>
        , IMvxServiceConsumer<ISamplesService>
    {
        public int CurrentVersion { get; set; }
        public int RequestedVersion { get; set; }

        public DataVersioningService()
        {
            this.CurrentVersion = -1;
            this.RequestedVersion = -1;
        }

        public async Task<bool> SetVersionAsync()
        {
            if (this.CurrentVersion < 0 || this.RequestedVersion < 0)
                throw new InvalidOperationException("Current and requested data versions must be set prior to calling data versioning operations");

            if (this.CurrentVersion != this.RequestedVersion)
            {
                if (this.RequestedVersion <= 1)
                {
                    await this.GetService<IServiceRepository>().ClearServicesAsync();
                }
                else
                {
                    await this.GetService<ISamplesService>().UpdateSamplesAsync();
                }
            }

            return true;
        }
    }
}