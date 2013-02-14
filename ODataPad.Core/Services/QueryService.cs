using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Simple.OData.Client;
using ODataPad.Core.Models;

namespace ODataPad.Core.Services
{
    public class QueryService
    {
        public async Task<QueryResult> LoadResultsAsync(
            string serviceUrl, string collectionName, int skipCount, int maxCount)
        {
            var task = Task<QueryResult>.Factory.StartNew(() =>
            {
                var result = new QueryResult();
                try
                {
                    var odataClient = new ODataClient(serviceUrl);
                    result.Rows = odataClient
                        .For(collectionName)
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

            return await task;
        }
    }
}