using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
            _collections = new ObservableCollection<CollectionViewModel>();
            _collectionModes = new ObservableCollection<string>(new[]
                                                                    {
                                                                        "Show collection properties", 
                                                                        "Show collection data"
                                                                    });
            _collectionMode = _collectionModes[0];
        }

        public HomeViewModelBase Self { get { return this; } }

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

        private ObservableCollection<CollectionViewModel> _collections;
        public ObservableCollection<CollectionViewModel> Collections
        {
            get { return _collections; }
            set { _collections = value; RaisePropertyChanged(() => Collections); }
        }

        private CollectionViewModel _selectedCollection;
        public CollectionViewModel SelectedCollection
        {
            get { return _selectedCollection; }
            set { _selectedCollection = value; RaisePropertyChanged(() => SelectedCollection); }
        }

        public ICommand SelectCollectionCommand
        {
            get { return new MvxCommand(SelectCollection); }
        }

        public virtual void SelectCollection()
        {
        }

        private readonly ObservableCollection<string> _collectionModes; 
        public IEnumerable<string> CollectionModes
        {
            get { return _collectionModes; }
        }

        private string _collectionMode;
        public string CollectionMode
        {
            get { return _collectionMode; }
            set
            {
                _collectionMode = value;
                RaisePropertyChanged(() => CollectionMode);
                RaisePropertyChanged(() => IsPropertyViewSelected);
                RaisePropertyChanged(() => IsResultViewSelected);
                RaisePropertyChanged(() => IsSingleResultSelected);
            }
        }

        public ICommand SelectCollectionModeCommand
        {
            get { return new MvxCommand(SelectCollectionMode); }
        }

        public virtual void SelectCollectionMode()
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

        public bool IsPropertyViewSelected { get { return this.CollectionMode == this.CollectionModes.First(); } }
        public bool IsResultViewSelected { get { return this.CollectionMode != this.CollectionModes.First() && this.SelectedResult == null; } }
        public bool IsSingleResultSelected { get { return this.CollectionMode != this.CollectionModes.First() && this.SelectedResult != null; } }
    }
}