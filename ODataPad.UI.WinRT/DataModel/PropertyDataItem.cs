using System;
using System.Collections.Generic;
using System.Linq;
using Simple.OData.Client;
using ODataPad.Core.Models;

namespace ODataPad.UI.WinRT.DataModel
{
    public class PropertyDataItem : DataItem
    {
        public PropertyDataItem(ServiceInfo service, Table table, Column column)
            : base(GetUniqueId(service.Name, table.ActualName, column.ActualName), column.ActualName, GetColumnSummary(column, table.GetKeyNames()), null, null)
        {
        }

        public PropertyDataItem(ServiceInfo service, Table table, Association association)
            : base(GetUniqueId(service.Name, table.ActualName, association.ActualName), association.ActualName, GetAssociationSummary(association), null, null)
        {
        }

        private static string GetColumnSummary(Column column, IEnumerable<string> keys)
        {
            var summary = column.PropertyType.ToString().Split('.').Last();
            var items = new List<string>();
            if (keys.Contains(column.ActualName))
                items.Add("key");
            if (column.IsNullable)
                items.Add("null");
            if (items.Any())
                summary += " (" + string.Join(",", items) + ")";
            return summary;
        }

        private static string GetAssociationSummary(Association association)
        {
            return association.Multiplicity;
        }
    }
}