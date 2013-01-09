using System.Collections.Generic;
using System.Linq;

namespace ODataPad.Core.Models
{
    public class CollectionProperty : CollectionElement
    {
        public CollectionProperty(string name, string type, bool isKey, bool isNullable)
        {
            this.Name = name;
            this.Summary = GetPropertySummary(type, isKey, isNullable);
        }

        public string Name { get; private set; }
        public string Summary { get; private set; }

        private string GetPropertySummary(string type, bool isKey, bool isNullable)
        {
            var summary = type;
            var items = new List<string>();
            if (isKey)
                items.Add("key");
            if (isNullable)
                items.Add("null");
            if (items.Any())
                summary += " (" + string.Join(",", items) + ")";
            return summary;
        }
    }
}