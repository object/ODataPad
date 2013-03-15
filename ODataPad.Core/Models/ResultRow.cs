using System.Collections.Generic;

namespace ODataPad.Core.Models
{
    public class ResultRow
    {
        public ResultRow(IDictionary<string, object> data, IEnumerable<string> keys)
        {
            this.Keys = keys;
            this.Properties = data;
        }

        public IEnumerable<string> Keys { get; private set; }
        public IDictionary<string, object> Properties { get; private set; }
    }
}