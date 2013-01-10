using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using ODataPad.Core.Models;

namespace ODataPad.UI.WinRT
{
    public class ObservableResultCollection : ObservableCollection<ResultRow>, ISupportIncrementalLoading
    {
        public string ServiceUrl { get; private set; }
        public string CollectionName { get; private set; }
        public IEnumerable<CollectionProperty> CollectionProperties { get; private set; }
        public MainPage MainPage;

        public ObservableResultCollection(string serviceUrl,
            string collectionName, IEnumerable<CollectionProperty> collectionProperties, MainPage mainPage)
        {
            this.ServiceUrl = serviceUrl;
            this.CollectionName = collectionName;
            this.CollectionProperties = collectionProperties;
            this.MainPage = mainPage;

            this.HasMoreItems = true;
        }

        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            return new PartialResultLoader(this, count);
        }

        public bool HasMoreItems { get; set; }
    }
}