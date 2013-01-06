using System.Collections.Generic;
using System.Threading.Tasks;
using ODataPad.Core.Models;

namespace ODataPad.Core.Services
{
    public interface ISamplesService
    {
        Task<IEnumerable<ODataServiceInfo>> GetAllSamplesAsync();
        Task<IEnumerable<ODataServiceInfo>> GetNewSamplesAsync();
        Task<IEnumerable<ODataServiceInfo>> GetUpdatedSamplesAsync();
        Task<IEnumerable<ODataServiceInfo>> GetExpiredSamplesAsync();
    }
}