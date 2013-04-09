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
        public Task<ObservableResultCollection> CollectResultsAsync(string serviceUrl, string collectionName, IEnumerable<CollectionProperty> collectionProperties, INotifyInProgress notifyInProgress)
        {
            return Task.Factory.StartNew(() => LoadResults(serviceUrl, collectionName, collectionProperties, notifyInProgress).Result);
        }

        public Task CollectMoreResultsAsync(ObservableResultCollection collection)
        {
            return Task.Factory.StartNew(() => LoadResults(collection).Result);
        }

        private async Task<ObservableResultCollection> LoadResults(
            string serviceUrl, string collectionName, IEnumerable<CollectionProperty> collectionProperties, INotifyInProgress notifyInProgress)
        {
            var collection = new ObservableResultCollection(serviceUrl, collectionName, collectionProperties, notifyInProgress);
            return await LoadResults(collection);
        }

        private async Task<ObservableResultCollection> LoadResults(ObservableResultCollection collection)
        {
            var resultLoader = new PartialResultLoader(collection.ServiceUrl, collection.CollectionName, collection.CollectionProperties, collection.NotifyInProgress);
            var resultRows = await resultLoader.LoadResults(collection.Count, 100);

            foreach (var row in resultRows)
            {
                collection.Add(new ResultViewItem(row));
            }
            collection.HasMoreItems = resultLoader.HasMoreItems;
            return collection;
        }
    }
}