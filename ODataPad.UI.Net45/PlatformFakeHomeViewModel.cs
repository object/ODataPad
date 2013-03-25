using System;
using System.Windows.Media.Imaging;
using ODataPad.Core.ViewModels;

namespace ODataPad.Platform.Net45
{
    public class PlatformFakeHomeViewModel : FakeHomeViewModel
    {
        public PlatformFakeHomeViewModel()
        {
            foreach (var service in this.Services)
            {
                service.Image = new BitmapImage(new Uri(@"D:\Projects\GitHub\ODataPad\ODataPad.UI.Net45\Samples\" + service.Name + ".png"));
                //service.Image = new BitmapImage(new Uri(@"pack://application:,,,component/Samples/" + service.Name + ".png", UriKind.RelativeOrAbsolute));
            }
        }
    }
}
