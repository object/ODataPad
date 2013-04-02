using System.Threading.Tasks;
using Cirrious.CrossCore.IoC;
using ODataPad.Core.Interfaces;

namespace ODataPad.Platform.Net45
{
    public class ApplicationLocalData
        : IApplicationLocalData
    {
        public ApplicationLocalData()
        {
        }

        public async Task SetDataVersionAsync(int requestedDataVersion)
        {
            await Mvx.Resolve<IDataVersioningService>().SetDataVersionAsync(3, 3);
        }
    }
}