using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ODataPad.Core.Interfaces;
using ODataPad.Core.Models;
using Simple.OData.Client;

namespace ODataPad.Core.Services
{
    public class ODataService
    {
        public async Task<QueryResult> LoadResultsAsync(
            string serviceUrl, string resourceSetName, int skipCount, int maxCount, INotifyInProgress notify)
        {
            var task = Task<QueryResult>.Factory.StartNew(() =>
            {
                var result = new QueryResult();
                try
                {
                    var odataClient = new ODataClient(serviceUrl);
                    result.Rows = odataClient
                        .For(resourceSetName)
                        .Skip(skipCount)
                        .Top(maxCount)
                        .FindEntries();
                }
                catch (Exception exception)
                {
                    result.IsError = true;
                    var error = new Dictionary<string, object>()
                                    { {
                                            "Error",
                                            exception.InnerException == null ?
                                            exception.Message :
                                            exception.InnerException.Message
                                    } };
                    result.Rows = new List<IDictionary<string, object>> { error };
                }
                return result;
            });

            notify.IsInProgress = true;
            var queryResult = await task;
            notify.IsInProgress = false;
            return queryResult;
        }
    }
}