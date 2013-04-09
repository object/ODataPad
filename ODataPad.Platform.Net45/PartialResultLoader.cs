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
    public enum AsyncStatus
    {
        Started,
        Completed,
        Canceled,
        Error,
    }

    //public struct LoadMoreItemsResult
    //{
    //    public uint Count { get; set; }
    //}

    public class PartialResultLoader
    {
        private const int PageSize = 40;
        private AsyncStatus _asyncStatus = AsyncStatus.Started;
        //private LoadMoreItemsResult _results;

        public string ServiceUrl { get; private set; }
        public string CollectionName { get; private set; }
        public IEnumerable<CollectionProperty> CollectionProperties { get; private set; }
        
        private readonly INotifyInProgress _notify;

        public PartialResultLoader(string serviceUrl, string collectionName, IEnumerable<CollectionProperty> collectionProperties, INotifyInProgress notify)
        {
            this.ServiceUrl = serviceUrl;
            this.CollectionName = collectionName;
            this.CollectionProperties = collectionProperties;
            _notify = notify;
        }

        public async Task<ObservableCollection<ResultRow>> LoadResults(uint count)
        {
            var odataService = new ODataService();

            var offset = 0;
            var result = await odataService.LoadResultsAsync(
                this.ServiceUrl,
                this.CollectionName,
                0,
                Math.Max((int)count, PageSize),
                _notify);

            bool moreItems = false;
            ObservableCollection<ResultRow> resultRows = new ObservableCollection<ResultRow>();
            if (result != null)
            {
                moreItems = result.Rows.Any() && !result.IsError;
                resultRows = new ObservableCollection<ResultRow>(result.Rows.Select(row => new ResultRow(row,
                                                                         this.CollectionProperties.Where(
                                                                             x => x.IsKey).Select(x => x.Name))));
            }
            else
            {
                moreItems = false;
            }

            _asyncStatus = AsyncStatus.Completed;
            //            if (Completed != null) Completed(this, _asyncStatus);

            return resultRows;
        }

        //        public AsyncOperationCompletedHandler<LoadMoreItemsResult> Completed { get; set; }

        //public LoadMoreItemsResult GetResults()
        //{
        //    return _results;
        //}

        public AsyncStatus Status
        {
            get { return _asyncStatus; }
        }
    }
}