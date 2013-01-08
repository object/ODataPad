﻿using System.Threading.Tasks;

namespace ODataPad.Core.Services
{
    public interface IResourceManager
    {
        Task<string> LoadContentAsStringAsync(string folderName, string resourceName);
        Task<string> LoadResourceAsStringAsync(string moduleName, string folderName, string resourceName);
        string GetImageResourcePath(string folderName, string imageName);
    }
}