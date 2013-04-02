using System;
using System.Windows.Media.Imaging;
using ODataPad.Core.ViewModels;

namespace ODataPad.UI.Net45
{
    public class PlatformFakeHomeViewModel : FakeHomeViewModel
    {
        public PlatformFakeHomeViewModel()
        {
            foreach (var service in this.Services)
            {
                service.Image = new BitmapImage(new Uri(@"pack://application:,,,/ODataPad.UI.Net45;component/Samples/" + service.Name + ".png"));
            }
        }
    }
}
