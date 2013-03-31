using System.Collections.Generic;
using System.Collections.ObjectModel;
using ODataPad.Core.Interfaces;
using ODataPad.Core.Models;
using ODataPad.Core.ViewModels;
using Windows.Foundation;
using Windows.UI.Xaml.Data;

namespace ODataPad.Platform.WinRT
{
    public class ObservableResultCollection : ObservableCollection<ResultViewItem>, ISupportIncrementalLoading
    {
        public string ServiceUrl { get; private set; }
        public string CollectionName { get; private set; }
        public IEnumerable<CollectionProperty> CollectionProperties { get; private set; }

        private readonly INotifyInProgress _notify;

        public ObservableResultCollection(string serviceUrl, string collectionName,
            IEnumerable<CollectionProperty> collectionProperties, INotifyInProgress notify)
        {
            this.ServiceUrl = serviceUrl;
            this.CollectionName = collectionName;
            this.CollectionProperties = collectionProperties;
            _notify = notify;

            this.HasMoreItems = true;
        }

        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            return new PartialResultLoader(this, count, _notify);
        }

        public bool HasMoreItems { get; set; }
    }
}