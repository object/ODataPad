using System.Collections;
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
    public class ResourceSetListViewModel : MvxViewModel
    {
        private readonly string _serviceUrl;

        public ResourceSetListViewModel(string serviceUrl)
        {
            _serviceUrl = serviceUrl;
            _resourceSets = new ObservableCollection<ResourceSet>();
        }

        public AppStateViewModel StateView { get { return AppState.Current.View; } }

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
                    if (StateView.ActiveResourceSet != null)
                        StateView.ActiveResourceSet.Results.SelectedResult = null;
                    if (value == null)
                        StateView.ActiveResourceSet = null;
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
                    var converter = Mvx.Resolve<IMvxJsonConverter>();

                    ShowViewModel<ResourceSetDetailsViewModel>(new ResourceSetDetailsViewModel.NavObject
                    {
                        ServiceUrl = _serviceUrl,
                        ResourceSetName = _selectedResourceSet.Name,
                        SerializedProperties = converter.SerializeObject(_selectedResourceSet.Properties),
                        SerializedAssociations = converter.SerializeObject(_selectedResourceSet.Associations),
                    });
                }
                else
                {
                    StateView.ActiveResourceSet = new ResourceSetDetailsViewModel(_serviceUrl, _selectedResourceSet);
                }

                if (StateView.ActiveResourceSetMode == StateView.ResourceSetModes.Last())
                {
                    await StateView.ActiveResourceSet.Results.LoadResultsAsync();
                }
            }
            else
            {
                StateView.ActiveResourceSet = null;
            }
        }

        public void DesignModePopulate(IEnumerable<ResourceSet> details)
        {
            this.Items = new ObservableCollection<ResourceSet>(details);
        }

        public void DesignModeSetActiveResourceSet(ResourceSet resourceSet)
        {
            StateView.ActiveResourceSet = resourceSet == null ? null : new ResourceSetDetailsViewModel(_serviceUrl, resourceSet);
        }
    }
}