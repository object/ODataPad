using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ODataPad.Core.Interfaces;
using ODataPad.Core.ViewModels;

namespace ODataPad.Core.Models
{
    public class ObservableResultCollection : ObservableCollection<ResultViewItem>
    {
        public string ServiceUrl { get; private set; }
        public string CollectionName { get; private set; }
        public IEnumerable<CollectionProperty> CollectionProperties { get; private set; }
        public INotifyInProgress NotifyInProgress { get; private set; }

        public ObservableResultCollection(
            string serviceUrl,
            string collectionName,
            IEnumerable<CollectionProperty> collectionProperties,
            INotifyInProgress notifyInProgress)
        {
            this.ServiceUrl = serviceUrl;
            this.CollectionName = collectionName;
            this.CollectionProperties = collectionProperties;
            this.NotifyInProgress = notifyInProgress;

            this.HasMoreItems = true;
        }

        public bool HasMoreItems { get; set; }
    }
}