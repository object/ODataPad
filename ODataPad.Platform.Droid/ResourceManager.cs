using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Android.App;
using Android.Graphics;
using ODataPad.Core;
using ODataPad.Core.Interfaces;
using Path = System.IO.Path;

namespace ODataPad.Platform.Droid
{
    public class ResourceManager : IResourceManager
    {
        public Task<string> LoadContentAsStringAsync(string folderName, string resourceName)
        {
            throw new NotImplementedException();
            //var streamInfo = Application.GetResourceStream(new Uri(Path.Combine(folderName, resourceName), UriKind.Relative));
            //using (var reader = new StreamReader(streamInfo.Stream))
            //{
            //    return await reader.ReadToEndAsync();
            //}
        }

        public Task<string> LoadResourceAsStringAsync(string moduleName, string folderName, string resourceName)
        {
            throw new NotImplementedException();
            //var resourceStream = GetResourceStream(moduleName, folderName, resourceName);
            //using (var reader = new StreamReader(resourceStream))
            //{
            //    return await reader.ReadToEndAsync();
            //}
        }

        public Uri GetResourceUri(string folderName, string resourceName)
        {
            throw new NotImplementedException();
            //var resourcePath = resourceName;
            //if (!string.IsNullOrEmpty(folderName))
            //    resourcePath = string.Join("/", folderName, resourcePath);

            //return new Uri("ms-appx:///" + resourcePath);
        }

        public Uri GetResourceUri(string moduleName, string folderName, string resourceName)
        {
            throw new NotImplementedException();
        }

        public Task<Bitmap> LoadResourceAsImageAsync(string moduleName, string folderName, string resourceName)
        {
            throw new NotImplementedException();
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
    }
}