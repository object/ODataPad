using System.Collections.Generic;
using System.Collections.ObjectModel;
using ODataPad.Core.Models;
using ODataPad.Core.ViewModels;

namespace ODataPad.Core.Interfaces
{
    public interface IResultProvider
    {
        ObservableCollection<ResultViewItem> CollectResults(
            string serviceUrl, 
            string collectionName, 
            IEnumerable<CollectionProperty> collectionProperties,
            INotifyInProgress notifyInProgress);
    }
}