using System.Collections.Generic;
using System.Collections.ObjectModel;
using ODataPad.Core.Interfaces;
using ODataPad.Core.Models;
using ODataPad.Core.ViewModels;

namespace ODataPad.Platform.WinRT
{
    public class ResultProvider : IResultProvider
    {
        public ObservableCollection<ResultViewItem> CollectResults(
            string serviceUrl, 
            string collectionName, 
            IEnumerable<CollectionProperty> collectionProperties,
            INotifyInProgress notifyInProgress)
        {
            return new ObservableResultCollection(serviceUrl, collectionName, collectionProperties, notifyInProgress);
        }
    }
}