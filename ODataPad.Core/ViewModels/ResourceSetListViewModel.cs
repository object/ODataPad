using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;
using ODataPad.Core.Models;

namespace ODataPad.Core.ViewModels
{
    public class ResourceSetListViewModel : MvxViewModel
    {
        private readonly string _serviceUrl;

        public ResourceSetListViewModel(string serviceUrl)
        {
            _serviceUrl = serviceUrl;
            _resourceSets = new ObservableCollection<ResourceSet>();
        }

        public AppState AppState { get { return AppState.Current; } }

        private ObservableCollection<ResourceSet> _resourceSets;
        public ObservableCollection<ResourceSet> Items
        {
            get { return _resourceSets; }
            set
            {
                _resourceSets = value; 
                RaisePropertyChanged(() => Items);
            }
        }

        private ResourceSet _selectedResourceSet;
        public ResourceSet SelectedItem
        {
            get { return _selectedResourceSet; }
            set
            {
                if (_selectedResourceSet != value)
                {
                    if (AppState.ActiveResourceSet != null)
                        AppState.ActiveResourceSet.Results.SelectedResult = null;
                    if (value == null)
                        AppState.ActiveResourceSet = null;
                }
                _selectedResourceSet = value;
                RaisePropertyChanged(() => SelectedItem);
            }
        }

        public ICommand SelectResourceSetCommand
        {
            get { return new MvxCommand(SelectResourceSet); }
        }

        public async void SelectResourceSet()
        {
            if (this.SelectedItem != null)
            {
                if (AppState.ViewModelsWithOwnViews.Contains(typeof(ResourceSetDetailsViewModel)))
                {
                    ShowViewModel<ResourceSetDetailsViewModel>(new ResourceSetDetailsViewModel.NavObject(
                        _serviceUrl, _selectedResourceSet.Name, 
                        _selectedResourceSet.Properties, _selectedResourceSet.Associations));
                }
                else
                {
                    AppState.ActiveResourceSet = new ResourceSetDetailsViewModel(_serviceUrl, _selectedResourceSet);
                }

                if (AppState.ActiveResourceSetMode == AppState.ResourceSetModes.Last())
                {
                    await AppState.ActiveResourceSet.Results.LoadResultsAsync();
                }
            }
            else
            {
                AppState.ActiveResourceSet = null;
            }
        }

        public void DesignModePopulate(IEnumerable<ResourceSet> details)
        {
            this.Items = new ObservableCollection<ResourceSet>(details);
        }

        public void DesignModeSetActiveResourceSet(ResourceSet resourceSet)
        {
            AppState.ActiveResourceSet = resourceSet == null ? null : new ResourceSetDetailsViewModel(_serviceUrl, resourceSet);
        }
    }
}