using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Cirrious.CrossCore;
using Cirrious.CrossCore.Platform;
using Cirrious.MvvmCross.ViewModels;
using ODataPad.Core.Models;

namespace ODataPad.Core.ViewModels
{
    public partial class ResourceSetDetailsViewModel : MvxViewModel
    {
        private string _serviceUrl;
        private ResourceSet _resourceSet;
        private SchemaViewModel _schema;
        private ResultListViewModel _results;

        public ResourceSetDetailsViewModel()
        {
        }

        public ResourceSetDetailsViewModel(string serviceUrl, ResourceSet resourceSet)
        {
            _serviceUrl = serviceUrl;
            _resourceSet = resourceSet;

            _schema = new SchemaViewModel(_resourceSet.Properties, _resourceSet.Associations);
            _results = new ResultListViewModel(_serviceUrl, _resourceSet.Name, _resourceSet.Properties);
        }

        public AppState AppState { get { return AppState.Current; } }

        public string Name { get { return _resourceSet.Name; } }
        public string Summary { get { return _resourceSet.Summary; } }
        public IList<ResourceProperty> Properties { get { return _resourceSet.Properties; } }
        public IList<ResourceAssociation> Associations { get { return _resourceSet.Associations; } }
        public SchemaViewModel Schema { get { return _schema; } }
        public ResultListViewModel Results { get { return _results; } }

        public string SelectedResourceSetMode
        {
            get { return AppState.ActiveResourceSetMode; }
            set
            {
                AppState.ActiveResourceSetMode = value;
                RaisePropertyChanged(() => SelectedResourceSetMode);
            }
        }

        public ICommand SelectResourceSetModeCommand
        {
            get { return new MvxCommand(SelectResourceSetMode); }
        }

        public async void SelectResourceSetMode()
        {
            if (AppState.IsResultViewSelected)
            {
                await this.Results.LoadResultsAsync();
            }
        }
    }
}