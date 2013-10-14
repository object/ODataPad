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
            _resources = new ObservableCollection<ResourceSetViewModel>();
            _resourceSetModes = new ObservableCollection<string>(new[]
                                                                    {
                                                                        "Show collection properties", 
                                                                        "Show collection data"
                                                                    });
            _resourceSetMode = _resourceSetModes[0];
        }

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

        private ObservableCollection<ResourceSetViewModel> _resources;
        public ObservableCollection<ResourceSetViewModel> Resources
        {
            get { return _resources; }
            set { _resources = value; RaisePropertyChanged(() => Resources); }
        }

        private ResourceSetViewModel _selectedResource;
        public ResourceSetViewModel SelectedResource
        {
            get { return _selectedResource; }
            set { _selectedResource = value; RaisePropertyChanged(() => SelectedResource); }
        }

        public ICommand SelectResourceCommand
        {
            get { return new MvxCommand(SelectResource); }
        }

        public virtual void SelectResource()
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
                RaisePropertyChanged(() => IsSingleResultSelected);
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
            set { _isQueryInProgress = value; RaisePropertyChanged(() => IsQueryInProgress); }
        }

        public ICommand LoadMoreResultsCommand
        {
            get { return new MvxCommand<bool>(x => { if (x) LoadMoreResults(); }); }
        }

        public virtual void LoadMoreResults()
        {
        }

        private ResultViewModel _selectedResult;
        public ResultViewModel SelectedResult
        {
            get { return _selectedResult; }
            set
            {
                _selectedResult = value;
                RaisePropertyChanged(() => SelectedResult);
                RaisePropertyChanged(() => IsResultViewSelected);
                RaisePropertyChanged(() => IsSingleResultSelected);
            }
        }

        public ICommand SelectResultCommand
        {
            get { return new MvxCommand(SelectResult); }
        }

        public virtual void SelectResult()
        {
        }

        public ICommand UnselectResultCommand
        {
             get { return new MvxCommand(CollapseResult); } 
        }

        public virtual void CollapseResult()
        {
            this.SelectedResult = null;
        }

        private string _selectedResultDetails;
        public string SelectedResultDetails
        {
            get { return _selectedResultDetails; }
            set { _selectedResultDetails = value; RaisePropertyChanged(() => SelectedResultDetails); }
        }

        public bool IsPropertyViewSelected { get { return this.ResourceSetMode == this.ResourceSetModes.First(); } }
        public bool IsResultViewSelected { get { return this.ResourceSetMode != this.ResourceSetModes.First() && this.SelectedResult == null; } }
        public bool IsSingleResultSelected { get { return this.ResourceSetMode != this.ResourceSetModes.First() && this.SelectedResult != null; } }
    }
}