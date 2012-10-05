using System;
using System.Collections.Generic;
using System.Linq;
using Simple.OData.Client;

namespace ODataPad.DataModel
{
    public class CollectionDataItem : DataItem
    {
        public CollectionDataItem(ServiceInfo service, Table table)
            : base(GetUniqueId(service.Name, table.ActualName), table.ActualName, GetCollectionSummary(table), null, null)
        {
        }

        private IEnumerable<IDictionary<string, object>> _results;
        public IEnumerable<IDictionary<string, object>> Results
        {
            get { return this._results; }
            set { this.SetProperty(ref this._results, value); }
        }

        private static string GetCollectionSummary(Table table)
        {
            return string.Format("{0} properties, {1} relations", table.Columns.Count(), table.Associations.Count());
        }
    }
}