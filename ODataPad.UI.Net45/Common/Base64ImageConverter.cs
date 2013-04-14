using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace ODataPad.UI.Net45.Common
{
    public class Base64ImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            byte[] bytes = System.Convert.FromBase64String((string)value);

            var image = new BitmapImage();
            if (bytes == null || bytes.Length <= 0)
                return image;

            using (var stream = new MemoryStream(bytes))
            {
                image.StreamSource = stream;
                return image;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var encoder = new PngBitmapEncoder();
            var frame = BitmapFrame.Create((BitmapSource) value);
            encoder.Frames.Add(frame);
            using (var stream = new MemoryStream())
            {
                encoder.Save(stream);
                return System.Convert.ToBase64String(stream.ToArray());
            }
        }
    }
}
