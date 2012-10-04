using System;
using System.Linq;
using Simple.OData.Client;

namespace ODataPad.DataModel
{
    public class PropertyDataItem : DataItem
    {
        public PropertyDataItem(ServiceInfo service, Table table, Column column)
            : base(GetUniqueId(service, table, column), column.ActualName, GetColumnSummary(column), null, null)
        {
        }

        public PropertyDataItem(ServiceInfo service, Table table, Association association)
            : base(GetUniqueId(service, table, association), association.ActualName, GetAssociationSummary(association), null, null)
        {
        }

        private static string GetColumnSummary(Column column)
        {
            return column.PropertyType.ToString().Split('.').Last() + (column.IsNullable ? ("(null)") : null);
        }

        private static string GetAssociationSummary(Association association)
        {
            return association.Multiplicity;
        }
    }
}