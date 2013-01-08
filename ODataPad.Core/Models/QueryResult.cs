using System.Collections.Generic;

namespace ODataPad.Core.Models
{
    public class QueryResult
    {
        public bool IsError { get; set; }
        public IEnumerable<IDictionary<string, object>> Rows { get; set; } 
    }
}