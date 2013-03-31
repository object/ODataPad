using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ODataPad.Core.Interfaces;
using ODataPad.Core.Models;
using ODataPad.Core.ViewModels;

namespace ODataPad.Platform.WinRT
{
    public class ResultProvider : IResultProvider
    {
        public Task<ObservableCollection<ResultViewItem>> CollectResultsAsync(
            string serviceUrl, 
            string collectionName, 
            IEnumerable<CollectionProperty> collectionProperties,
            INotifyInProgress notifyInProgress)
        {
            return Task.Factory.StartNew(() =>
                                             {
                                                 ObservableCollection<ResultViewItem> results = 
                                                     new ObservableResultCollection(
                                                     serviceUrl, collectionName, collectionProperties, notifyInProgress);
                                                 return results;
                                             }
                );
        }
    }
}