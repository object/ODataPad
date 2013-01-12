using System;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml.Media.Imaging;
using ODataPad.Core.Services;

namespace ODataPad.WinRT
{
    public class ImageService : IImageService
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