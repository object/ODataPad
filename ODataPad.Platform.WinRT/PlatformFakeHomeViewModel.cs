using System;
using ODataPad.Core.ViewModels;
using Windows.UI.Xaml.Media.Imaging;

namespace ODataPad.Platform.WinRT
{
    public class PlatformFakeHomeViewModel : FakeHomeViewModel
    {
        public PlatformFakeHomeViewModel()
        {
            foreach (var service in this.Services)
            {
                service.Image = new BitmapImage(new Uri("ms-appx:///Samples/" + service.Name + ".png"));
            }
        }
    }
}
