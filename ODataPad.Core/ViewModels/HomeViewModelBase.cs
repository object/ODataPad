using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Cirrious.MvvmCross.Commands;
using Cirrious.MvvmCross.ViewModels;

namespace ODataPad.Core.ViewModels
{
    public class HomeViewModelBase : MvxViewModel
    {
        protected HomeViewModelBase()
        {
            _services = new ObservableCollection<ServiceViewItem>();
            _collections = new ObservableCollection<CollectionViewItem>();
            _collectionModes = new ObservableCollection<string>(new[] { "Show collection properties", "Show collection data" });
            _collectionMode = _collectionModes[0];
        }

        public HomeViewModelBase Self { get { return this; } }

        private ObservableCollection<ServiceViewItem> _services;
        public ObservableCollection<ServiceViewItem> Services
        {
            get { return _services; }
            set { _services = value; RaisePropertyChanged("Services"); }
        }

        private ServiceViewItem _selectedService;
        public ServiceViewItem SelectedService
        {
            get { return _selectedService; }
            set { _selectedService = value; RaisePropertyChanged("SelectedService"); }
        }

        private bool _isServiceSelected;
        public bool IsServiceSelected
        {
            get { return _isServiceSelected; }
            set { _isServiceSelected = value; RaisePropertyChanged("IsServiceSelected"); }
        }

        public virtual ICommand SelectedServiceCommand
        {
            get { return null; }
        }

        private ObservableCollection<CollectionViewItem> _collections;
        public ObservableCollection<CollectionViewItem> Collections
        {
            get { return _collections; }
            set { _collections = value; RaisePropertyChanged("Collections"); }
        }

        private CollectionViewItem _selectedCollection;
        public CollectionViewItem SelectedCollection
        {
            get { return _selectedCollection; }
            set { _selectedCollection = value; RaisePropertyChanged("SelectedCollection"); }
        }

        public virtual ICommand SelectedCollectionCommand
        {
            get { return null; }
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
                RaisePropertyChanged("CollectionMode");
                RaisePropertyChanged("IsPropertyViewSelected");
                RaisePropertyChanged("IsResultViewSelected");
            }
        }

        public virtual ICommand CollectionModeCommand 
        {
            get { return null; }
        }

        private bool _isQueryInProgress;
        public bool IsQueryInProgress
        {
            get { return _isQueryInProgress; }
            set { _isQueryInProgress = value; RaisePropertyChanged("IsQueryInProgress"); }
        }

        public bool IsPropertyViewSelected { get { return this.CollectionMode == this.CollectionModes.First(); } }
        public bool IsResultViewSelected { get { return this.CollectionMode != this.CollectionModes.First(); } }
    }
}