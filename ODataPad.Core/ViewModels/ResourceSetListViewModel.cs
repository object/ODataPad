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
            _resourceSets = new ObservableCollection<ResourceSetDetailsViewModel>();
        }

        private ObservableCollection<ResourceSetDetailsViewModel> _resourceSets;
        public ObservableCollection<ResourceSetDetailsViewModel> Items
        {
            get { return _resourceSets; }
            set { _resourceSets = value; RaisePropertyChanged(() => Items); }
        }

        private ResourceSetDetailsViewModel _selectedResourceSet;
        public ResourceSetDetailsViewModel SelectedItem
        {
            get { return _selectedResourceSet; }
            set
            {
                _selectedResourceSet = value;
                AppState.Current.ActiveResourceSet = value;
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
                if (this.SelectedItem.IsResultViewSelected)
                {
                    await this.SelectedItem.Results.LoadResultsAsync();
                }

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
            }
        }

        public void Populate(IEnumerable<ResourceSetDetailsViewModel> details)
        {
            this.Items = new ObservableCollection<ResourceSetDetailsViewModel>(details);
        }

        public void SelectTopItem()
        {
            this.SelectedItem = this.Items.First();
        }
    }
}