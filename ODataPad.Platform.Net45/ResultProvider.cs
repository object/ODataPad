using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using ODataPad.Core.Interfaces;
using ODataPad.Core.Models;
using ODataPad.Core.Services;
using ODataPad.Core.ViewModels;

namespace ODataPad.Platform.Net45
{
    public class ResultProvider : IResultProvider
    {
        public async Task<ObservableResultCollection> CollectResultsAsync(string serviceUrl, string collectionName, IEnumerable<CollectionProperty> collectionProperties, INotifyInProgress notifyInProgress)
        {
            var collection = new ObservableResultCollection(serviceUrl, collectionName, collectionProperties, notifyInProgress);
            await LoadResults(collection);
            return collection;
        }

        public async Task CollectMoreResultsAsync(ObservableResultCollection collection)
        {
            await LoadResults(collection);
        }

        private async Task LoadResults(ObservableResultCollection collection)
        {
            var resultLoader = new PartialResultLoader(collection.ServiceUrl, collection.CollectionName, collection.CollectionProperties, collection.NotifyInProgress);
            var resultRows = await Task.Factory.StartNew(() => resultLoader.LoadResults(collection.Count, 100).Result);

            foreach (var row in resultRows)
            {
                collection.Add(new ResultViewItem(row));
            }
            collection.HasMoreItems = resultLoader.HasMoreItems;
        }
    }
}