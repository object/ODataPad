using System.Collections.Generic;
using System.Linq;
using Cirrious.MvvmCross.ViewModels;
using ODataPad.Core.Models;

namespace ODataPad.Core.ViewModels
{
    public class ResourceSetDetailsViewModel : MvxViewModel
    {
        private readonly ResourceSet _resourceSet;
        private readonly SchemaViewModel _schema;
        private readonly ResultListViewModel _results;

        public ResourceSetDetailsViewModel(HomeViewModelBase home, ResourceSet resourceSet)
        {
            this.Home = home;

            _resourceSet = resourceSet;
            _schema = new SchemaViewModel(this);
            _results = new ResultListViewModel(this);
        }

        public HomeViewModelBase Home { get; set; }
        public string Name { get { return _resourceSet.Name; } }
        public string Summary { get { return GetResourceSetSummary(); } }
        public IList<ResourceProperty> Properties { get { return _resourceSet.Properties; } }
        public IList<ResourceAssociation> Associations { get { return _resourceSet.Associations; } }
        public SchemaViewModel Schema { get { return _schema; } }
        public ResultListViewModel Results { get { return _results; } }

        private string GetResourceSetSummary()
        {
            return string.Format("{0} properties, {1} relations", this.Properties.Count, this.Associations.Count);
        }
    }
}