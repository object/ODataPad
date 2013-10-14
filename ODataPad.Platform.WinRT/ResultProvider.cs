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
        public ObservableResultCollection CreateResultCollection(
            string serviceUrl,
            string resourceSetName,
            IEnumerable<ResourceProperty> resourceProperties,
            INotifyInProgress notifyInProgress)
        {
            return new ObservableResultCollectionWithLoader(serviceUrl, resourceSetName, resourceProperties, notifyInProgress);
        }

        public async Task AddResultsAsync(ObservableResultCollection resultCollection)
        {
        }
    }
}