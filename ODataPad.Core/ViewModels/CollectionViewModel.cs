using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using ODataPad.Core.Models;

namespace ODataPad.Core.ViewModels
{
    public class CollectionViewModel : BindableBase
    {
        private readonly ServiceCollection _serviceCollection;
        private readonly List<SchemaElementViewModel> _schemaElements;

        public CollectionViewModel(HomeViewModelBase viewModel, ServiceCollection serviceCollection)
        {
            this.ViewModel = viewModel;
            _serviceCollection = serviceCollection;
            _schemaElements = PopulateSchemaElements();
        }

        public HomeViewModelBase ViewModel { get; set; }
        public string Name { get { return _serviceCollection.Name; } }
        public string Summary { get { return GetCollectionSummary(); } }
        public ObservableCollection<CollectionProperty> Properties { get { return _serviceCollection.Properties; } }
        public ObservableCollection<CollectionAssociation> Associations { get { return _serviceCollection.Associations; } }
        public List<SchemaElementViewModel> SchemaElements { get { return _schemaElements; } }

        private ObservableResultCollection _queryResults;
        public ObservableResultCollection QueryResults
        {
            get { return _queryResults; }
            set { this.SetProperty(ref _queryResults, value); }
        }

        private string GetCollectionSummary()
        {
            return string.Format("{0} properties, {1} relations", this.Properties.Count, this.Associations.Count);
        }

        private List<SchemaElementViewModel> PopulateSchemaElements()
        {
            var elements = new List<SchemaElementViewModel>();
            foreach (var property in this.Properties)
            {
                elements.Add(new SchemaElementViewModel(property));
            }
            foreach (var association in this.Associations)
            {
                elements.Add(new SchemaElementViewModel(association));
            }
            return elements;
        }
    }
}