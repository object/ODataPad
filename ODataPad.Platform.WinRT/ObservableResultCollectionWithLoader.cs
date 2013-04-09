using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ODataPad.Core.Interfaces;
using ODataPad.Core.Models;
using ODataPad.Core.ViewModels;
using Windows.Foundation;
using Windows.UI.Xaml.Data;

namespace ODataPad.Platform.WinRT
{
    public class ObservableResultCollectionWithLoader : ObservableResultCollection, ISupportIncrementalLoading
    {
        public ObservableResultCollectionWithLoader(
            string serviceUrl, 
            string collectionName,
            IEnumerable<CollectionProperty> collectionProperties, 
            INotifyInProgress notifyInProgress)
            : base(serviceUrl, collectionName, collectionProperties, notifyInProgress)
        {
        }

        public void LoadMoreResults()
        {
            LoadMoreItemsAsync((uint)this.Count);
        }

        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            return new PartialResultLoader(this, count, this.NotifyInProgress);
        }
    }
}