using System.Threading.Tasks;

namespace ODataPad.Core.Services
{
    public interface IImageService
    {
        object GetImage(string imagePath);
    }
}