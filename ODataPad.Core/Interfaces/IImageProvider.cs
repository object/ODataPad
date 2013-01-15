using System.Threading.Tasks;

namespace ODataPad.Core.Interfaces
{
    public interface IImageProvider
    {
        object GetImage(string imagePath);
    }
}