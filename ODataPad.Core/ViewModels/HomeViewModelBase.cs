using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;

namespace ODataPad.Core.ViewModels
{
    public class HomeViewModelBase : MvxViewModel
    {
        protected HomeViewModelBase()
        {
            _services = new ObservableCollection<ServiceDetailsViewModel>();
            _resourceSets = new ObservableCollection<ResourceSetDetailsViewModel>();
        }

        public virtual bool IsDesignTime { get { return true; } }

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

        private ObservableCollection<ResourceSetDetailsViewModel> _resourceSets;
        public ObservableCollection<ResourceSetDetailsViewModel> ResourceSets
        {
            get { return _resourceSets; }
            set { _resourceSets = value; RaisePropertyChanged(() => ResourceSets); }
        }

        private ResourceSetDetailsViewModel _selectedResourceSet;
        public ResourceSetDetailsViewModel SelectedResourceSet
        {
            get { return _selectedResourceSet; }
            set { _selectedResourceSet = value; RaisePropertyChanged(() => SelectedResourceSet); }
        }

        public ICommand SelectResourceSetCommand
        {
            get { return new MvxCommand(SelectResourceSet); }
        }

        public virtual void SelectResourceSet()
        {
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
                if (this.SelectedResourceSet != null)
                {
                    this.SelectedResourceSet.SelectedResourceSetMode = value;
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