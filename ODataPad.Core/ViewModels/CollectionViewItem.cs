using System.Linq;
using System.Collections.ObjectModel;
using ODataPad.Core.Models;

namespace ODataPad.Core.ViewModels
{
    public class CollectionViewItem : BindableBase
    {
        private readonly ServiceCollection _serviceCollection;

        public CollectionViewItem(HomeViewModelBase viewModel, ServiceCollection serviceCollection)
        {
            this.ViewModel = viewModel;
            _serviceCollection = serviceCollection;
        }

        public HomeViewModelBase ViewModel { get; set; }
        public string Name { get { return _serviceCollection.Name; } }
        public string Summary { get { return GetCollectionSummary(); } }
        public ObservableCollection<CollectionProperty> Properties { get { return _serviceCollection.Properties; } }
        public ObservableCollection<CollectionAssociation> Associations { get { return _serviceCollection.Associations; } }
        public ObservableCollection<SchemaElementViewItem> SchemaElements { get { return PopulateSchemaElements(); } }

        private ObservableCollection<ResultViewItem> _queryResults;
        public ObservableCollection<ResultViewItem> QueryResults
        {
            get { return _queryResults; }
            set { this.SetProperty(ref _queryResults, value); }
        }

        private string GetCollectionSummary()
        {
            return string.Format("{0} properties, {1} relations", this.Properties.Count, this.Associations.Count);
        }

        private ObservableCollection<SchemaElementViewItem> PopulateSchemaElements()
        {
            var elements = new ObservableCollection<SchemaElementViewItem>();
            foreach (var property in this.Properties)
            {
                elements.Add(new SchemaElementViewItem(property));
            }
            foreach (var association in this.Associations)
            {
                elements.Add(new SchemaElementViewItem(association));
            }
            return elements;
        }
    }
}