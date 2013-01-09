using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        public string KeySummary { get { return GetKeySummary(); } }
        public string ValueSummary { get { return GetPropertySummary(); } }

        private string GetKeySummary()
        {
            if (IsError())
            {
                return this.Properties.Keys.First();
            }
            else
            {
                var sb = new StringBuilder();
                foreach (var result in this.Properties)
                {
                    if (!this.Keys.Contains(result.Key))
                        continue;

                    if (sb.Length > 0)
                        sb.Append(" | ");
                    var text = result.Value.ToString();
                    sb.Append(text);
                }
                return sb.ToString();
            }
        }

        private string GetPropertySummary()
        {
            if (IsError())
            {
                return this.Properties.Values.First().ToString();
            }
            else
            {
                var sb = new StringBuilder();
                foreach (var result in this.Properties)
                {
                    if (this.Keys.Contains(result.Key))
                        continue;

                    if (sb.Length > 0)
                        sb.Append(" | ");
                    var text = result.Value == null ? "(null)" : result.Value.ToString();
                    if (text.Length > 30)
                        text = text.Substring(0, 30) + "...";
                    sb.Append(text);
                }
                return sb.ToString();
            }
        }

        private bool IsError()
        {
            return this.Properties.Count == 1 &&
                this.Properties.Keys.First() == "Error" &&
                !this.Keys.Contains(this.Properties.Keys.First());
        }
    }
}