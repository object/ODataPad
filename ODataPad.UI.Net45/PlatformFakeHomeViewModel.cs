using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using ODataPad.Core.Models;
using ODataPad.Core.ViewModels;
using ODataPad.Samples;

namespace ODataPad.UI.Net45
{
    public class PlatformFakeHomeViewModel : FakeHomeViewModel
    {
        public PlatformFakeHomeViewModel()
        {
            foreach (var service in this.Services)
            {
                service.Image = new BitmapImage(new Uri(@"pack://application:,,,/ODataPad.UI.Net45;component/Samples/" + service.Name + ".png"));

                //var stream = Assembly.GetAssembly(typeof(DesignData))
                //    .GetManifestResourceStream(string.Join(".", "ODataPad", "Samples", "ImagesBase64", service.Name, "png", "base64"));
                //using (var reader = new StreamReader(stream))
                //{
                //    service.ImageBase64 = reader.ReadToEnd();
                //}
            }
        }
    }
}
