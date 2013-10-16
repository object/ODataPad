using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using Cirrious.MvvmCross.ViewModels;
using ODataPad.Core.Models;

namespace ODataPad.Core.ViewModels
{
    public class ResourceSetViewModel : MvxViewModel
    {
        private readonly ResourceSet _resourceSet;
        private readonly List<SchemaElementViewModel> _schemaElements;
        private readonly ResultListViewModel _results;

        public ResourceSetViewModel(HomeViewModelBase home, ResourceSet resourceSet)
        {
            this.Home = home;
            _resourceSet = resourceSet;
            _schemaElements = PopulateSchemaElements();
            _results = new ResultListViewModel(home);
        }

        public HomeViewModelBase Home { get; set; }
        public string Name { get { return _resourceSet.Name; } }
        public string Summary { get { return GetResourceSetSummary(); } }
        public ObservableCollection<ResourceProperty> Properties { get { return _resourceSet.Properties; } }
        public ObservableCollection<ResourceAssociation> Associations { get { return _resourceSet.Associations; } }
        public List<SchemaElementViewModel> SchemaElements { get { return _schemaElements; } }
        public ResultListViewModel Results { get { return _results; } }

        private string GetResourceSetSummary()
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