using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;

namespace ODataPad.Core.ViewModels
{
    public class HomeViewModelBase : MvxViewModel
    {
        private readonly ResourceSetListViewModel _resourceSets;

        protected HomeViewModelBase()
        {
            _services = new ObservableCollection<ServiceDetailsViewModel>();
            _resourceSets = new ResourceSetListViewModel(this);
        }

        public virtual bool IsDesignTime { get { return true; } }
        public ResourceSetListViewModel ResourceSets { get { return _resourceSets; } }

        public ICommand AddServiceCommand
        {
            get { return new MvxCommand(AddService); }
        }

        public virtual void AddService()
        {
        }

        public ICommand EditServiceCommand
        {
            get { return new MvxCommand(AddService); }
        }

        public virtual void EditService()
        {
        }

        public ICommand RemoveServiceCommand
        {
            get { return new MvxCommand(AddService); }
        }

        public virtual void RemoveService()
        {
        }

        private bool _isServiceEditInProgress;
        public bool IsServiceEditInProgress
        {
            get { return _isServiceEditInProgress; }
            set { _isServiceEditInProgress = value; RaisePropertyChanged(() => IsServiceEditInProgress); }
        }

        private ServiceDetailsViewModel _editedService;
        public ServiceDetailsViewModel EditedService
        {
            get { return _editedService; }
            set
            {
                _editedService = value;
                RaisePropertyChanged(() => EditedService);
            }
        }

        private ObservableCollection<ServiceDetailsViewModel> _services;
        public ObservableCollection<ServiceDetailsViewModel> Services
        {
            get { return _services; }
            set { _services = value; RaisePropertyChanged(() => Services);
            }
        }

        private ServiceDetailsViewModel _selectedService;
        public ServiceDetailsViewModel SelectedService
        {
            get { return _selectedService; }
            set 
            { 
                _selectedService = value; 
                RaisePropertyChanged(() => SelectedService);
                RaisePropertyChanged(() => IsServiceSelected);
            }
        }

        public ICommand SelectServiceCommand
        {
            get { return new MvxCommand(SelectService); }
        }

        public virtual void SelectService()
        {
        }

        private bool _isServiceSelected;
        public bool IsServiceSelected
        {
            get { return _isServiceSelected; }
            set { _isServiceSelected = value; RaisePropertyChanged(() => IsServiceSelected); }
        }

        public IEnumerable<string> ResourceSetModes
        {
            get { return ResourceSetDetailsViewModel.ResourceSetModes; }
        }

        public string SelectedResourceSetMode
        {
            get { return ResourceSetDetailsViewModel.ResourceSetMode; }
            set
            {
                if (this.ResourceSets.SelectedItem != null)
                {
                    this.ResourceSets.SelectedItem.SelectedResourceSetMode = value;
                }
                else
                {
                    ResourceSetDetailsViewModel.ResourceSetMode = value;
                    RaisePropertyChanged(() => SelectedResourceSetMode);
                }
            }
        }

        private bool _isQueryInProgress;
        public bool IsQueryInProgress
        {
            get { return _isQueryInProgress; }
            set
            {
                _isQueryInProgress = value;
                RaisePropertyChanged(() => IsQueryInProgress);
            }
        }
    }
}