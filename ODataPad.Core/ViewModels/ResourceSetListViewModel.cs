﻿using System.Collections.ObjectModel;
using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;

namespace ODataPad.Core.ViewModels
{
    public class ResourceSetListViewModel : MvxViewModel
    {
        public ResourceSetListViewModel(HomeViewModelBase home)
        {
            this.Home = home;

            _resourceSets = new ObservableCollection<ResourceSetDetailsViewModel>();
        }

        public HomeViewModelBase Home { get; set; }

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
            set { _selectedResourceSet = value; RaisePropertyChanged(() => SelectedItem); }
        }

        public ICommand SelectResourceSetCommand
        {
            get { return new MvxCommand(SelectResourceSet); }
        }

        public async void SelectResourceSet()
        {
            if (this.SelectedItem != null && this.SelectedItem.IsResultViewSelected)
            {
                await this.SelectedItem.Results.RequestResourceData();
            }
        }
    }
}