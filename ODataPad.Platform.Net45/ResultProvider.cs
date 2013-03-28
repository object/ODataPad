using ODataPad.Core.Interfaces;

namespace ODataPad.Platform.Net45
{
    public class ResultProvider : IResultProvider
    {
        public System.Collections.ObjectModel.ObservableCollection<Core.ViewModels.ResultViewItem> CollectResults(string serviceUrl, string collectionName, System.Collections.Generic.IEnumerable<Core.Models.CollectionProperty> collectionProperties, INotifyInProgress notifyInProgress)
        {
            throw new System.NotImplementedException();
        }
    }
}