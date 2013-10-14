using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using ODataPad.Core.Interfaces;
using ODataPad.Core.Models;
using ODataPad.Core.Services;
using ODataPad.Core.ViewModels;

namespace ODataPad.Platform.Droid
{
    public class ResultProvider : IResultProvider
    {
        public ObservableResultCollection CreateResultCollection(
            string serviceUrl, 
            string resourceSetName,
            IEnumerable<ResourceProperty> resourceProperties, 
            INotifyInProgress notifyInProgress)
        {
            return new ObservableResultCollection(serviceUrl, resourceSetName, resourceProperties, notifyInProgress);
        }

        public Task AddResultsAsync(ObservableResultCollection resultCollection)
        {
            var resultLoader = new PartialResultLoader(resultCollection.ServiceUrl, resultCollection.ResourceSetName, resultCollection.ResourceProperties, resultCollection.NotifyInProgress);
            var resultRows = Task.Factory.StartNew(() => resultLoader.LoadResults(resultCollection.Count, 100).Result).Result;

            foreach (var row in resultRows)            {
                resultCollection.Add(new ResultViewModel(row));
            }
            resultCollection.HasMoreItems = resultLoader.HasMoreItems;
            throw new NotImplementedException();
        }
    }
}