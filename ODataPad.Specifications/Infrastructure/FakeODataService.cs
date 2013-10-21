using System.Collections.Generic;
using System.Threading.Tasks;
using ODataPad.Core.Models;

namespace ODataPad.Specifications.Infrastructure
{
    public class FakeODataService
    {
        public Task<QueryResult> GetQueryResultAsync()
        {
            var queryResult = new QueryResult
            {
                Rows = new[]
                {
                    new Dictionary<string, object>()
                    {
                        {"Id", 1},
                        {"Name", "SharePoint 2010"},
                        {"Description", "(...)"},
                        {"ApplicationUrl", "(...)"}
                    },
                    new Dictionary<string, object>()
                    {
                        {"Id", 2},
                        {"Name", "IBM WebSphere"},
                        {"Description", "(...)"},
                        {"ApplicationUrl", "(...)"}
                    },
                    new Dictionary<string, object>()
                    {
                        {"Id", 4},
                        {"Name", "Windows Azure Table Storage"},
                        {"Description", "Windows Azure Table provides (...)"},
                        {"ApplicationUrl", "http://msdn.microsoft.com/en-us/azure/cc994380.aspx"}
                    },
                }
            };

            return Task.FromResult(queryResult);
        }
    }
}
