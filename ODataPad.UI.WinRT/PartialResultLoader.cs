using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ODataPad.Core.Services;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using ODataPad.UI.WinRT.DataModel;

namespace ODataPad.UI.WinRT
{
    public class PartialResultLoader : IAsyncOperation<LoadMoreItemsResult>
    {
        private const int PageSize = 40;
        private AsyncStatus _asyncStatus = AsyncStatus.Started;
        private LoadMoreItemsResult _results;

        public PartialResultLoader(ObservableResultCollection collection, uint count)
        {
            LoadResults(collection, count);
        }

        public async Task LoadResults(ObservableResultCollection collection, uint count)
        {
            var queryService = new QueryService();

            collection.MainPage.EnableResultProgressBar(true);
            var result = await queryService.LoadResultsAsync(
                collection.ServiceUrl,
                collection.CollectionName,
                collection.Count,
                Math.Max((int)count, PageSize));
            collection.MainPage.EnableResultProgressBar(false);

            if (result != null)
            {
                collection.HasMoreItems = result.Rows.Any() && !result.IsError;
                foreach (var row in result.Rows)
                {
                    collection.Add(new ResultDataItem(row, collection.Table));
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