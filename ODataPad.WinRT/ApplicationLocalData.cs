using System;
using System.Threading.Tasks;
using Cirrious.MvvmCross.ExtensionMethods;
using Cirrious.MvvmCross.Interfaces.ServiceProvider;
using ODataPad.Core.Interfaces;
using Windows.Storage;

namespace ODataPad.WinRT
{
    public class ApplicationLocalData 
        : IApplicationLocalData,
        IMvxServiceConsumer<IDataVersioningService>
    {
        public ApplicationLocalData()
        {
        }

        public async Task<bool> SetDataVersionAsync(int requestedDataVersion)
        {
            await ApplicationData.Current.SetVersionAsync((uint)requestedDataVersion, SetVersionHandlerAsync);
            return true;
        }

        private async void SetVersionHandlerAsync(SetVersionRequest request)
        {
            SetVersionDeferral deferral = request.GetDeferral();
            await this.GetService<IDataVersioningService>().SetDataVersionAsync((int)request.CurrentVersion, (int)request.DesiredVersion);
            deferral.Complete();
        }
    }
}