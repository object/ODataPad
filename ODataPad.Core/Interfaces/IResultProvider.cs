using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ODataPad.Core.Models;
using ODataPad.Core.ViewModels;

namespace ODataPad.Core.Interfaces
{
    public interface IResultProvider
    {
        Task<ObservableResultCollection> CollectResultsAsync(
            string serviceUrl, 
            string collectionName, 
            IEnumerable<CollectionProperty> collectionProperties,
            INotifyInProgress notifyInProgress);

        Task CollectMoreResultsAsync(ObservableResultCollection collection);
    }
}