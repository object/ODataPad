using System;
using ODataPad.Core.ViewModels;
using Windows.UI.Xaml.Media.Imaging;

namespace ODataPad.WinRT
{
    public class FakeHomeViewModel : CoreFakeHomeViewModel
    {
        public FakeHomeViewModel()
        {
            foreach (var service in this.Services)
            {
                service.Image = new BitmapImage(new Uri("ms-appx:///Samples/" + service.Name + ".png"));
            }
        }
    }
}
