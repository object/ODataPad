using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace ODataPad.UI.WinRT.Common
{
    public sealed class Base64ImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return ConvertAsync(value, targetType, parameter, language).Result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }

        public async Task<BitmapImage> ConvertAsync(object value, Type targetType, object parameter, string language)
        {
            var image = new BitmapImage();

            if (value != null)
            {
                var bytes = System.Convert.FromBase64String((string)value);

                var ras = new InMemoryRandomAccessStream();
                using (var writer = new DataWriter(ras.GetOutputStreamAt(0)))
                {
                    writer.WriteBytes(bytes);
                    await writer.StoreAsync();
                }

                image.SetSource(ras);
            }
            return image;
        }
    }
}
