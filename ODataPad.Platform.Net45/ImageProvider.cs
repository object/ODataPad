using System;
using System.Linq;
using System.Windows.Media.Imaging;
using ODataPad.Core.Interfaces;

namespace ODataPad.Platform.Net45
{
    public class ImageProvider : IImageProvider
    {
        private const string _uriBase = "pack://siteoforigin:,,,/";

        public object GetImage(string imagePath)
        {
            BitmapImage imageSource = null;
            if (!string.IsNullOrEmpty(imagePath))
            {
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string path;
                if (baseDirectory.EndsWith(@"\bin\Debug") || baseDirectory.EndsWith(@"\bin\Release"))
                {
                    path = _uriBase + baseDirectory.Split('\\').Last() + "/" + imagePath;
                }
                else
                {
                    path = _uriBase + imagePath;
                }
                var uri = new Uri(path, UriKind.Absolute);
                imageSource = new BitmapImage(uri);
            }
            return imageSource;
        }
    }
}