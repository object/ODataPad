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
            _services = new ObservableCollection<ServiceViewModel>();
            _resourceSets = new ObservableCollection<ResourceSetViewModel>();
            _resourceSetModes = new ObservableCollection<string>(new[]
                                                                    {
                                                                        "Show collection properties", 
                                                                        "Show collection data"
                                                                    });
            _resourceSetMode = _resourceSetModes[0];
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

        private ServiceViewModel _editedService;
        public ServiceViewModel EditedService
        {
            get { return _editedService; }
            set
            {
                _editedService = value;
                RaisePropertyChanged(() => EditedService);
            }
        }

        private ObservableCollection<ServiceViewModel> _services;
        public ObservableCollection<ServiceViewModel> Services
        {
            get { return _services; }
            set { _services = value; RaisePropertyChanged(() => Services);
            }
        }

        private ServiceViewModel _selectedService;
        public ServiceViewModel SelectedService
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

        private ObservableCollection<ResourceSetViewModel> _resourceSets;
        public ObservableCollection<ResourceSetViewModel> ResourceSets
        {
            get { return _resourceSets; }
            set { _resourceSets = value; RaisePropertyChanged(() => ResourceSets); }
        }

        private ResourceSetViewModel _selectedResourceSet;
        public ResourceSetViewModel SelectedResourceSet
        {
            get { return _selectedResourceSet; }
            set { _selectedResourceSet = value; RaisePropertyChanged(() => SelectedResourceSet); }
        }

        public ICommand SelectResourceCommand
        {
            get { return new MvxCommand(SelectResourceSet); }
        }

        public virtual void SelectResourceSet()
        {
        }

        private readonly ObservableCollection<string> _resourceSetModes; 
        public IEnumerable<string> ResourceSetModes
        {
            get { return _resourceSetModes; }
        }

        private string _resourceSetMode;
        public string ResourceSetMode
        {
            get { return _resourceSetMode; }
            set
            {
                _resourceSetMode = value;
                RaisePropertyChanged(() => ResourceSetMode);
                RaisePropertyChanged(() => IsPropertyViewSelected);
                RaisePropertyChanged(() => IsResultViewSelected);
            }
        }

        public ICommand SelectResourceModeCommand
        {
            get { return new MvxCommand(SelectResourceMode); }
        }

        public virtual void SelectResourceMode()
        {
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

        public bool IsPropertyViewSelected { get { return this.ResourceSetMode == this.ResourceSetModes.First(); } }
        public bool IsResultViewSelected { get { return this.ResourceSetMode != this.ResourceSetModes.First(); } }
    }
}