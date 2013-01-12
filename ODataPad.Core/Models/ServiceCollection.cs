using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ODataPad.Core.Models
{
    public class ServiceCollection : BindableBase
    {
        public ServiceCollection(string name, 
            IEnumerable<CollectionProperty> properties, 
            IEnumerable<CollectionAssociation> associations)
        {
            this.Name = name;
            this.Properties = new ObservableCollection<CollectionProperty>(properties);
            this.Associations = new ObservableCollection<CollectionAssociation>(associations);
        }

        public string Name { get; set; }
        public string Summary { get { return GetCollectionSummary(); } }
        public ObservableCollection<CollectionProperty> Properties { get; private set; }
        public ObservableCollection<CollectionAssociation> Associations { get; private set; }
        
        private ObservableCollection<ResultRow> _queryResults;
        public ObservableCollection<ResultRow> QueryResults
        {
            get { return _queryResults; }
            set { this.SetProperty(ref _queryResults, value); }
        }

        public ObservableCollection<CollectionElement> SchemaElements
        {
            get
            {
                var elements = new ObservableCollection<CollectionElement>();
                foreach (var property in this.Properties)
                {
                    elements.Add(property);
                }
                foreach (var association in this.Associations)
                {
                    elements.Add(association);
                }
                return elements;
            }
        }

        private string GetCollectionSummary()
        {
            return string.Format("{0} properties, {1} relations", this.Properties.Count, this.Associations.Count);
        }
    }
}