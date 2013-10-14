using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ODataPad.Core.Interfaces;
using ODataPad.Core.ViewModels;

namespace ODataPad.Core.Models
{
    public class ObservableResultCollection : ObservableCollection<ResultViewModel>
    {
        public string ServiceUrl { get; private set; }
        public string ResourceSetName { get; private set; }
        public IEnumerable<ResourceProperty> ResourceProperties { get; private set; }
        public INotifyInProgress NotifyInProgress { get; private set; }

        public ObservableResultCollection(
            string serviceUrl,
            string resourceSetName,
            IEnumerable<ResourceProperty> resourceProperties,
            INotifyInProgress notifyInProgress)
        {
            this.ServiceUrl = serviceUrl;
            this.ResourceSetName = resourceSetName;
            this.ResourceProperties = resourceProperties;
            this.NotifyInProgress = notifyInProgress;

            this.HasMoreItems = true;
        }

        public bool HasMoreItems { get; set; }
    }
}