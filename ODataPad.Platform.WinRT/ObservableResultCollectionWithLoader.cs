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
            string resourceSetName,
            IEnumerable<ResourceProperty> resourceProperties, 
            INotifyInProgress notifyInProgress)
            : base(serviceUrl, resourceSetName, resourceProperties, notifyInProgress)
        {
        }

        public void LoadMoreResults()
        {
            LoadMoreItemsAsync((uint)this.Count);
        }

        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            var loader = new PartialResultLoaderWithAsyncOperation(this);
            loader.LoadResults((int)count, 100);
            return loader;
        }
    }
}