using System.Collections.Generic;
using System.Linq;

namespace ODataPad.Core.Models
{
    public class CollectionProperty : CollectionElement
    {
        public CollectionProperty(string name, string type, bool isKey, bool isNullable)
        {
            this.Name = name;
            this.Type = type;
            this.IsKey = isKey;
            this.IsNullable = isNullable;
        }

        public string Name { get; private set; }
        public string Type { get; private set; }
        public bool IsKey { get; private set; }
        public bool IsNullable { get; private set; }
        public string Summary { get { return GetSummary(); } }

        private string GetSummary()
        {
            var summary = this.Type;
            var items = new List<string>();
            if (this.IsKey)
                items.Add("key");
            if (this.IsNullable)
                items.Add("null");
            if (items.Any())
                summary += " (" + string.Join(",", items) + ")";
            return summary;
        }
    }
}