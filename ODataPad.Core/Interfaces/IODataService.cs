using System.Threading.Tasks;
using ODataPad.Core.Models;

namespace ODataPad.Core.Interfaces
{
    public interface IODataService
    {
        Task<QueryResult> LoadResultsAsync(string serviceUrl, string resourceSetName, int skipCount, int maxCount, INotifyInProgress notify);
    }
}