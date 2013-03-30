using System;
using System.Linq;
using System.Reflection;
using System.Windows.Media.Imaging;
using ODataPad.Core.Interfaces;

namespace ODataPad.Platform.Net45
{
    public class ImageProvider : IImageProvider
    {
        public object GetImage(string imagePath)
        {
            BitmapImage imageSource = null;
            if (!string.IsNullOrEmpty(imagePath))
            {
                var assemblyName = (Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly()).GetName().Name;
                var path = string.Format("pack://application:,,,/{0};component/{1}", assemblyName, imagePath);
                var uri = new Uri(path, UriKind.Absolute);
                imageSource = new BitmapImage(uri);
            }
            return imageSource;
        }
    }
}