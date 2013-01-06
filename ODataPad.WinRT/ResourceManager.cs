using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Windows.Storage;
using ODataPad.Core.Services;

namespace ODataPad.WinRT
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
            var assembly = Assembly.Load(new AssemblyName(moduleName));
            var resourceStream = assembly.GetManifestResourceStream(string.Join(".", moduleName, folderName, resourceName));
            using (var reader = new StreamReader(resourceStream))
            {
                return reader.ReadToEnd();
            }
        }

        public string GetImageResourcePath(string folderName, string imageName)
        {
            return new Uri("ms-appx:///") + string.Join("/", folderName, imageName);
        }
    }
}