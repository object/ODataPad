using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace ODataPad.UI.WinRT.Common
{
    public class Base64ImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return ConvertAsync(value, targetType, parameter, language).Result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        public async Task<object> ConvertAsync(object value, Type targetType, object parameter, string language)
        {
            byte[] bytes = System.Convert.FromBase64String((string)value);

            var image = new BitmapImage();
            if (bytes == null || bytes.Length <= 0)
                return image;

            using (var stream = new MemoryStream(bytes))
            {
                var ras = new InMemoryRandomAccessStream();
                await stream.CopyToAsync(ras.AsStreamForWrite());
                image.SetSource(ras);
                return image;
            }
        }
    }
}
