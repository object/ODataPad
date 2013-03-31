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
            return Task.Factory.StartNew(() => LoadResultsAsync(serviceUrl, collectionName, collectionProperties, notifyInProgress));
        }

        private ObservableCollection<ResultViewItem> LoadResultsAsync(string serviceUrl, string collectionName, IEnumerable<CollectionProperty> collectionProperties, INotifyInProgress notifyInProgress)
        {
            var odataService = new ODataService();

            var result = odataService.LoadResultsAsync(
                serviceUrl,
                collectionName,
                10,
                10,
                notifyInProgress).Result;

            var resultCollection = new ObservableCollection<ResultViewItem>();
            foreach (var row in result.Rows)
            {
                var resultRow = new ResultRow(row, collectionProperties.Where(x => x.IsKey).Select(x => x.Name));
                resultCollection.Add(new ResultViewItem(resultRow));
            }

            return resultCollection;
        }
    }
}