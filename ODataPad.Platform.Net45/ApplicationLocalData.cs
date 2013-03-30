using System.Threading.Tasks;
using Cirrious.MvvmCross.ExtensionMethods;
using Cirrious.MvvmCross.Interfaces.ServiceProvider;
using ODataPad.Core.Interfaces;

namespace ODataPad.Platform.Net45
{
    public class ApplicationLocalData
        : IApplicationLocalData,
        IMvxServiceConsumer<IDataVersioningService>
    {
        public ApplicationLocalData()
        {
        }

        public async Task SetDataVersionAsync(int requestedDataVersion)
        {
            await this.GetService<IDataVersioningService>().SetDataVersionAsync(3, 3);
        }
    }
}