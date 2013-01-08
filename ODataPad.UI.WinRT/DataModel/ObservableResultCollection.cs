using System.Collections.ObjectModel;
using Simple.OData.Client;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace ODataPad.UI.WinRT.DataModel
{
    public class ObservableResultCollection : ObservableCollection<ResultDataItem>, ISupportIncrementalLoading
    {
        public string ServiceUrl { get; private set; }
        public string CollectionName { get; private set; }
        public Table Table { get; private set; }
        public MainPage MainPage;

        public ObservableResultCollection(string serviceUrl, string collectionName, Table table, MainPage mainPage)
        {
            this.ServiceUrl = serviceUrl;
            this.CollectionName = collectionName;
            this.Table = table;
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