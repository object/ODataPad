using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;
using ODataPad.Core.Models;

namespace ODataPad.Core.ViewModels
{
    public class ResourceSetDetailsViewModel : MvxViewModel
    {
        private readonly ResourceSet _resourceSet;
        private readonly SchemaViewModel _schema;
        private readonly ResultListViewModel _results;

        static ResourceSetDetailsViewModel()
        {
            ResourceSetModes = new ObservableCollection<string>(new[]
                                                                    {
                                                                        "Show collection properties", 
                                                                        "Show collection data"
                                                                    });
            ResourceSetMode = ResourceSetModes[0];
        }

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

        public static IList<string> ResourceSetModes { get; private set; }
        public static string ResourceSetMode { get; set; }

        public string SelectedResourceSetMode
        {
            get { return ResourceSetMode; }
            set
            {
                ResourceSetMode = value;
                RaisePropertyChanged(() => SelectedResourceSetMode);
                RaisePropertyChanged(() => IsPropertyViewSelected);
                RaisePropertyChanged(() => IsResultViewSelected);
            }
        }

        public ICommand SelectResourceSetModeCommand
        {
            get { return new MvxCommand(SelectResourceSetMode); }
        }

        public async void SelectResourceSetMode()
        {
            if (this.IsResultViewSelected)
            {
                await this.Results.LoadResultsAsync();
            }
        }

        public bool IsPropertyViewSelected { get { return this.SelectedResourceSetMode == ResourceSetModes.First(); } }
        public bool IsResultViewSelected { get { return this.SelectedResourceSetMode == ResourceSetModes.Last(); } }

        private string GetResourceSetSummary()
        {
            return string.Format("{0} properties, {1} relations", this.Properties.Count, this.Associations.Count);
        }
    }
}