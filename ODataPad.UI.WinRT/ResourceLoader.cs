using System;
using System.Threading.Tasks;
using ODataPad.Core.Services;
using Windows.Storage;

namespace ODataPad
{
    public class ResourceLoader : IResourceLoader
    {
        public async Task<string> LoadResourceFileAsString(string folderName, string filename)
        {
            var resourceMap = Windows.ApplicationModel.Resources.Core.ResourceManager.Current.MainResourceMap;
            var file = await resourceMap.GetSubtree(folderName).GetValue(filename).GetValueAsFileAsync();
            return await FileIO.ReadTextAsync(file);
        }
    }
}