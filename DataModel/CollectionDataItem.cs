using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ODataPad.Core.Models;
using Simple.OData.Client;

namespace ODataPad.DataModel
{
    public class CollectionDataItem : DataItem
    {
        public CollectionDataItem(ODataServiceInfo service, Table table)
            : base(GetUniqueId(service.Name, table.ActualName), table.ActualName, GetCollectionSummary(table), null, null)
        {
            this.Table = table;
        }

        public Table Table { get; private set; }

        private ObservableResultCollection _results;
        public ObservableResultCollection Results
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