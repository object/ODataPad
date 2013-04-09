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
        public Task<ObservableResultCollection> CollectResultsAsync(
            string serviceUrl, 
            string collectionName, 
            IEnumerable<CollectionProperty> collectionProperties,
            INotifyInProgress notifyInProgress)
        {
            return
                Task.Factory.StartNew(() => LoadResults(serviceUrl, collectionName, collectionProperties, notifyInProgress));
        }

        private ObservableResultCollection LoadResults(
            string serviceUrl, 
            string collectionName, 
            IEnumerable<CollectionProperty> collectionProperties, 
            INotifyInProgress notifyInProgress)
        {
            return new ObservableResultCollectionWithLoader(serviceUrl, collectionName, collectionProperties, notifyInProgress);
        }

        public async Task CollectMoreResultsAsync(ObservableResultCollection collection)
        {
            LoadResults(collection.ServiceUrl, collection.CollectionName, collection.CollectionProperties, collection.NotifyInProgress);
        }
    }
}