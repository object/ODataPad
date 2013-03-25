using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ODataPad.Core.Interfaces;
using ODataPad.Core.Models;
using ODataPad.Core.Services;
using ODataPad.Core.ViewModels;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace ODataPad.Platform.WinRT
{
    public class PartialResultLoader : IAsyncOperation<LoadMoreItemsResult>
    {
        private const int PageSize = 40;
        private AsyncStatus _asyncStatus = AsyncStatus.Started;
        private LoadMoreItemsResult _results;

        public PartialResultLoader(ObservableResultCollection collection, uint count, INotifyInProgress notify)
        {
            LoadResults(collection, count, notify);
        }

        public async Task LoadResults(ObservableResultCollection collection, uint count, INotifyInProgress notify)
        {
            var odataService = new ODataService();

            var result = await odataService.LoadResultsAsync(
                collection.ServiceUrl,
                collection.CollectionName,
                collection.Count,
                Math.Max((int)count, PageSize),
                notify);

            if (result != null)
            {
                collection.HasMoreItems = result.Rows.Any() && !result.IsError;
                foreach (var row in result.Rows)
                {
                    var resultRow = new ResultRow(row, collection.CollectionProperties.Where(x => x.IsKey).Select(x => x.Name));
                    collection.Add(new ResultViewItem(resultRow));
                }
                _results.Count = (uint)result.Rows.Count();
            }
            else
            {
                collection.HasMoreItems = false;
            }

            _asyncStatus = AsyncStatus.Completed;
            if (Completed != null) Completed(this, _asyncStatus);
        }

        public AsyncOperationCompletedHandler<LoadMoreItemsResult> Completed { get; set; }

        public LoadMoreItemsResult GetResults()
        {
            return _results;
        }

        public void Cancel()
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
        }

        public Exception ErrorCode
        {
            get { throw new NotImplementedException(); }
        }

        public uint Id
        {
            get { throw new NotImplementedException(); }
        }

        public AsyncStatus Status
        {
            get { return _asyncStatus; }
        }
    }
}