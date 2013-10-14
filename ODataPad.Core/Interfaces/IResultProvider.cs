using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ODataPad.Core.Models;
using ODataPad.Core.ViewModels;

namespace ODataPad.Core.Interfaces
{
    public interface IResultProvider
    {
        ObservableResultCollection CreateResultCollection(
            string serviceUrl,
            string resourceSetName,
            IEnumerable<ResourceProperty> resourceProperties,
            INotifyInProgress notifyInProgress);

        Task AddResultsAsync(ObservableResultCollection resultCollection);
    }
}