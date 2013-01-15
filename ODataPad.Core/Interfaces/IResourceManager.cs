using System.Threading.Tasks;

namespace ODataPad.Core.Interfaces
{
    public interface IResourceManager
    {
        Task<string> LoadContentAsStringAsync(string folderName, string resourceName);
        Task<string> LoadResourceAsStringAsync(string moduleName, string folderName, string resourceName);
    }
}