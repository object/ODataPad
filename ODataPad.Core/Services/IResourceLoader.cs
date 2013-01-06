using System.Threading.Tasks;

namespace ODataPad.Core.Services
{
    public interface IResourceLoader
    {
        Task<string> LoadResourceFileAsString(string folderName, string filename);
    }
}