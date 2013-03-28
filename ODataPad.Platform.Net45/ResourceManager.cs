﻿using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using ODataPad.Core.Interfaces;

namespace ODataPad.Platform.Net45
{
    public class ResourceManager : IResourceManager
    {
        public async Task<string> LoadContentAsStringAsync(string folderName, string resourceName)
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            using (var reader = new StreamReader(Path.Combine(baseDirectory, folderName, resourceName)))
            {
                return await reader.ReadToEndAsync();
            }
        }

        public async Task<string> LoadResourceAsStringAsync(string moduleName, string folderName, string resourceName)
        {
            var resourceStream = GetResourceStream(moduleName, folderName, resourceName);
            using (var reader = new StreamReader(resourceStream))
            {
                return await reader.ReadToEndAsync();
            }
        }

        public Uri GetResourceUri(string folderName, string resourceName)
        {
            var resourcePath = resourceName;
            if (!string.IsNullOrEmpty(folderName))
                resourcePath = string.Join("/", folderName, resourcePath);

            return new Uri("pack://siteoforigin:,,,/" + resourcePath);
        }

        public Uri GetResourceUri(string moduleName, string folderName, string resourceName)
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