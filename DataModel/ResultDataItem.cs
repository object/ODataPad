using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Simple.OData.Client;

namespace ODataPad.DataModel
{
    public class ResultDataItem : DataItem
    {
        public ResultDataItem(IDictionary<string, object> results, Table table)
            : base(Guid.NewGuid().ToString(), GetKeySummary(results, table.GetKeyNames()), GetResultSummary(results, table.GetKeyNames()), null, null)
        {
        }

        private static string GetKeySummary(IDictionary<string, object> results, IEnumerable<string> keys)
        {
            if (IsError(results, keys))
            {
                return results.Keys.First();
            }
            else
            {
                var sb = new StringBuilder();
                foreach (var result in results)
                {
                    if (!keys.Contains(result.Key))
                        continue;

                    if (sb.Length > 0)
                        sb.Append(" | ");
                    var text = result.Value.ToString();
                    sb.Append(text);
                }
                return sb.ToString();
            }
        }

        private static string GetResultSummary(IDictionary<string, object> results, IEnumerable<string> keys)
        {
            if (IsError(results, keys))
            {
                return results.Values.First().ToString();
            }
            else
            {
                var sb = new StringBuilder();
                foreach (var result in results)
                {
                    if (keys.Contains(result.Key))
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

        private static bool IsError(IDictionary<string, object> results, IEnumerable<string> keys)
        {
            return results.Count == 1 && 
                results.Keys.First() == "Error" && 
                !keys.Contains(results.Keys.First());
        }
    }
}