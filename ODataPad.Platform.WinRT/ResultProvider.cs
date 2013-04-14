using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ODataPad.Core.Interfaces;
using ODataPad.Core.Models;
using ODataPad.Core.ViewModels;

namespace ODataPad.Platform.WinRT
{
    public class ResultProvider : IResultProvider
    {
        public ObservableResultCollection CreateResultCollection(
            string serviceUrl,
            string collectionName,
            IEnumerable<CollectionProperty> collectionProperties,
            INotifyInProgress notifyInProgress)
        {
            return new ObservableResultCollectionWithLoader(serviceUrl, collectionName, collectionProperties, notifyInProgress);
        }

        public async Task AddResultsAsync(ObservableResultCollection collection)
        {
        }
    }
}