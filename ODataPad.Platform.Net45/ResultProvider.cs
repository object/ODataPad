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
        public Task<ObservableCollection<ResultViewItem>> CollectResultsAsync(string serviceUrl, string collectionName, IEnumerable<CollectionProperty> collectionProperties, INotifyInProgress notifyInProgress)
        {
            return Task.Factory.StartNew(() => LoadResultsAsync(serviceUrl, collectionName, collectionProperties, notifyInProgress).Result);
        }

        private async Task<ObservableCollection<ResultViewItem>> LoadResultsAsync(string serviceUrl, string collectionName, IEnumerable<CollectionProperty> collectionProperties, INotifyInProgress notifyInProgress)
        {
            var resultCollection = new ObservableResultCollection();

            var resultLoader = new PartialResultLoader(serviceUrl, collectionName, collectionProperties, notifyInProgress);
            var resultRows = await resultLoader.LoadResults(100);

            foreach (var row in resultRows)
            {
                resultCollection.Add(new ResultViewItem(row));
            }

            return resultCollection;
        }
    }
}