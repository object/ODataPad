using System;
using System.Threading.Tasks;
using ODataPad.Core.Interfaces;
using Windows.UI.Core;
using Windows.UI.Xaml.Media.Imaging;

namespace ODataPad.WinRT
{
    public class ImageProvider : IImageProvider
    {
        private static readonly Uri _baseUri = new Uri("ms-appx:///");

        public object GetImage(string imagePath)
        {
            BitmapImage imageSource = null;
            if (!string.IsNullOrEmpty(imagePath))
            {
                imageSource = new BitmapImage(new Uri(_baseUri, imagePath));
            }
            return imageSource;
        }
    }
}