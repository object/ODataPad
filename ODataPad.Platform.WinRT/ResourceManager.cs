using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using ODataPad.Core.Interfaces;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace ODataPad.Platform.WinRT
{
    public class ResourceManager : IResourceManager
    {
        public async Task<string> LoadContentAsStringAsync(string folderName, string resourceName)
        {
            var resourceMap = Windows.ApplicationModel.Resources.Core.ResourceManager.Current.MainResourceMap;
            var resourceFile = await resourceMap.GetSubtree("Files/" + folderName)
                .GetValue(resourceName)
                .GetValueAsFileAsync();
            return await FileIO.ReadTextAsync(resourceFile);
        }

        public async Task<string> LoadResourceAsStringAsync(string moduleName, string folderName, string resourceName)
        {
            var resourceStream = GetResourceStream(moduleName, folderName, resourceName);
            using (var reader = new StreamReader(resourceStream))
            {
                return await reader.ReadToEndAsync();
            }
        }

        public async Task<BitmapImage> LoadResourceAsImageAsync(string moduleName, string folderName, string resourceName)
        {
            var resourceStream = GetResourceStream(moduleName, folderName, resourceName);
            BitmapImage image = null;
            var ras = new InMemoryRandomAccessStream();
            await resourceStream.CopyToAsync(ras.AsStreamForWrite());
            await Utils.ExecuteOnUIThread(() =>
            {
                image = new BitmapImage();
                image.SetSource(ras);
            });
            return image;
        }

        public Stream GetResourceStream(string moduleName, string folderName, string resourceName)
        {
            var assembly = Assembly.Load(new AssemblyName(moduleName));
            var resourcePath = resourceName;
            if (!string.IsNullOrEmpty(folderName))
                resourcePath = string.Join(".", folderName, resourcePath);
            if (!string.IsNullOrEmpty(moduleName))
                resourcePath = string.Join(".", moduleName, resourcePath);

            return assembly.GetManifestResourceStream(resourcePath);
        }

        public string GetImageResourcePath(string folderName, string imageName)
        {
            var imagePath = imageName;
            if (!string.IsNullOrEmpty(folderName))
                imagePath = string.Join("/", folderName, imagePath);

            return new Uri("ms-appx:///") + imagePath;
        }
    }
}