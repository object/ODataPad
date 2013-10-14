using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ODataPad.Core.Interfaces;
using ODataPad.Core.Models;

namespace ODataPad.Core.Services
{
    public class PartialResultLoader
    {
        private const int PageSize = 40;

        public bool HasMoreItems { get; set; }

        private readonly string _serviceUrl;
        private readonly string _resourceSetName;
        private readonly IEnumerable<ResourceProperty> _resourceProperties;
        private readonly INotifyInProgress _notify;

        public PartialResultLoader(string serviceUrl, string resourceSetName, IEnumerable<ResourceProperty> resourceProperties, INotifyInProgress notify)
        {
            _serviceUrl = serviceUrl;
            _resourceSetName = resourceSetName;
            _resourceProperties = resourceProperties;
            _notify = notify;

            this.HasMoreItems = true;
        }

        public virtual async Task<ObservableCollection<ResultRow>> LoadResults(int skipCount, int maxCount)
        {
            var odataService = new ODataService();

            var result = await odataService.LoadResultsAsync(
                _serviceUrl,
                _resourceSetName,
                skipCount,
                Math.Max(maxCount, PageSize),
                _notify);

            var resultRows = new ObservableCollection<ResultRow>();
            if (result != null)
            {
                this.HasMoreItems = result.Rows.Any() && !result.IsError;
                resultRows = new ObservableCollection<ResultRow>(result.Rows.Select(row => 
                    new ResultRow(row, _resourceProperties.Where(x => x.IsKey).Select(x => x.Name))));
            }
            else
            {
                this.HasMoreItems = false;
            }

            return resultRows;
        }
    }
}