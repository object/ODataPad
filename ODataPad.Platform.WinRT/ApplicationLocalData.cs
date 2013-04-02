using System;
using System.Threading.Tasks;
using Cirrious.CrossCore.IoC;
using ODataPad.Core.Interfaces;
using Windows.Storage;

namespace ODataPad.Platform.WinRT
{
    public class ApplicationLocalData 
        : IApplicationLocalData
    {
        public ApplicationLocalData()
        {
        }

        public async Task SetDataVersionAsync(int requestedDataVersion)
        {
            await ApplicationData.Current.SetVersionAsync((uint)requestedDataVersion, SetVersionHandlerAsync);
        }

        private async void SetVersionHandlerAsync(SetVersionRequest request)
        {
            SetVersionDeferral deferral = request.GetDeferral();
            await Mvx.Resolve<IDataVersioningService>().SetDataVersionAsync((int)request.CurrentVersion, (int)request.DesiredVersion);
            deferral.Complete();
        }
    }
}