using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using ODataPad.Core.Interfaces;
using ODataPad.Core.Models;
using ODataPad.Core.Services;
using ODataPad.Core.ViewModels;

namespace ODataPad.Platform.Net45
{
    public class PartialResultLoader
    {
        private const int PageSize = 40;

        public bool HasMoreItems { get; set; }

        private readonly string _serviceUrl;
        private readonly string _collectionName;
        private readonly IEnumerable<CollectionProperty> _collectionProperties;
        private readonly INotifyInProgress _notify;

        public PartialResultLoader(string serviceUrl, string collectionName, IEnumerable<CollectionProperty> collectionProperties, INotifyInProgress notify)
        {
            _serviceUrl = serviceUrl;
            _collectionName = collectionName;
            _collectionProperties = collectionProperties;
            _notify = notify;

            this.HasMoreItems = true;
        }

        public async Task<ObservableCollection<ResultRow>> LoadResults(int skipCount, int maxCount)
        {
            var odataService = new ODataService();

            var result = await odataService.LoadResultsAsync(
                _serviceUrl,
                _collectionName,
                skipCount,
                Math.Max(maxCount, PageSize),
                _notify);

            var resultRows = new ObservableCollection<ResultRow>();
            if (result != null)
            {
                this.HasMoreItems = result.Rows.Any() && !result.IsError;
                resultRows = new ObservableCollection<ResultRow>(result.Rows.Select(row => 
                    new ResultRow(row, _collectionProperties.Where(x => x.IsKey).Select(x => x.Name))));
            }
            else
            {
                this.HasMoreItems = false;
            }

            return resultRows;
        }
    }
}