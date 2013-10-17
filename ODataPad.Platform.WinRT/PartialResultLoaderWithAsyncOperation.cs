﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using ODataPad.Core.Models;
using ODataPad.Core.Services;
using ODataPad.Core.ViewModels;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace ODataPad.Platform.WinRT
{
    public class PartialResultLoaderWithAsyncOperation : PartialResultLoader, IAsyncOperation<LoadMoreItemsResult>
    {
        private AsyncStatus _asyncStatus = AsyncStatus.Started;
        private LoadMoreItemsResult _results;
        private readonly ObservableResultCollectionWithLoader _resultCollection;

        public PartialResultLoaderWithAsyncOperation(ObservableResultCollectionWithLoader resultCollection)
            : base(resultCollection.ServiceUrl, resultCollection.ResourceSetName, resultCollection.ResourceProperties, resultCollection.NotifyInProgress)
        {
            _resultCollection = resultCollection;
        }

        public override async Task<ObservableCollection<ResultRow>> LoadResults(int skipCount, int maxCount)
        {
            var resultRows = await base.LoadResults(skipCount, maxCount);
            _resultCollection.HasMoreItems = this.HasMoreItems;

            if (resultRows != null)
            {
                foreach (var row in resultRows)
                {
                    _resultCollection.Add(new ResultDetailsViewModel(row));
                }
                _results.Count = (uint)resultRows.Count();
            }

            _asyncStatus = AsyncStatus.Completed;
            if (Completed != null) Completed(this, _asyncStatus);
            return resultRows;
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