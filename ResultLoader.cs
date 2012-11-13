using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Simple.OData.Client;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using ODataPad.DataModel;

namespace ODataPad
{
    public class ResultLoader : IAsyncOperation<LoadMoreItemsResult>
    {
        private const int PageSize = 40;
        private AsyncStatus _asyncStatus = AsyncStatus.Started;
        private LoadMoreItemsResult _results;

        public ResultLoader(ObservableResultCollection collection, uint count)
        {
            LoadResults(collection, count);
        }

        public async Task LoadResults(ObservableResultCollection collection, uint count)
        {
            var task = Task<IEnumerable<IDictionary<string, object>>>.Factory.StartNew(() =>
            {
                try
                {
                    var odataClient = new ODataClient(collection.ServiceUrl);
                    return odataClient
                        .From(collection.CollectionName)
                        .Skip(collection.Count)
                        .Top(Math.Max((int)count, PageSize))
                        .FindEntries();
                }
                catch (Exception exception)
                {
                    return null;
                }
            });

            collection.MainPage.EnableResultProgressBar(true);
            var results = await task;
            collection.MainPage.EnableResultProgressBar(false);

            if (results != null)
            {
                collection.HasMoreItems = results.Any();
                foreach (var result in results)
                {
                    collection.Add(new ResultDataItem(result, collection.Table));
                }
                _results.Count = (uint)results.Count();
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