using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using ODataPad.Core.Interfaces;
using ODataPad.Core.Models;
using ODataPad.Core.Services;
using ODataPad.Core.ViewModels;

namespace ODataPad.Platform.Droid
{
    public class ResultProvider : IResultProvider
    {
        public ObservableResultCollection CreateResultCollection(
            string serviceUrl, 
            string collectionName,
            IEnumerable<CollectionProperty> collectionProperties, 
            INotifyInProgress notifyInProgress)
        {
            return new ObservableResultCollection(serviceUrl, collectionName, collectionProperties, notifyInProgress);
        }

        public Task AddResultsAsync(ObservableResultCollection collection)
        {
            var resultLoader = new PartialResultLoader(collection.ServiceUrl, collection.CollectionName, collection.CollectionProperties, collection.NotifyInProgress);
            var resultRows = Task.Factory.StartNew(() => resultLoader.LoadResults(collection.Count, 100).Result).Result;

            foreach (var row in resultRows)            {
                collection.Add(new ResultViewModel(row));
            }
            collection.HasMoreItems = resultLoader.HasMoreItems;
            throw new NotImplementedException();
        }
    }
}