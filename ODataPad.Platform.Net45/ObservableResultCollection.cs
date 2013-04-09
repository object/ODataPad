using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ODataPad.Core.Interfaces;
using ODataPad.Core.Models;
using ODataPad.Core.ViewModels;

namespace ODataPad.Platform.Net45
{
    public class ObservableResultCollection : ObservableCollection<ResultViewItem>
    {
        //public string ServiceUrl { get; private set; }
        //public string CollectionName { get; private set; }
        //public IEnumerable<CollectionProperty> CollectionProperties { get; private set; }

        //private readonly INotifyInProgress _notify;

        public ObservableResultCollection()
        {
        }

        //public ObservableResultCollection(string serviceUrl, string collectionName,
        //    IEnumerable<CollectionProperty> collectionProperties, INotifyInProgress notify)
        //{
        //    this.ServiceUrl = serviceUrl;
        //    this.CollectionName = collectionName;
        //    this.CollectionProperties = collectionProperties;
        //    _notify = notify;

        //    this.HasMoreItems = true;
        //    LoadResults();
        //}

        public bool HasMoreItems { get; set; }

        //private async Task LoadResults()
        //{
        //    await new PartialResultLoader().LoadResults(this, 100, _notify);
        //}
    }
}