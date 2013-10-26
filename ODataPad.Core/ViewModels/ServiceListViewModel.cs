using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Xml.Linq;
using Cirrious.MvvmCross.ViewModels;
using ODataPad.Core.Models;
using ODataPad.Core.Services;

namespace ODataPad.Core.ViewModels
{
    public class ServiceListViewModel : MvxViewModel
    {
        public ServiceListViewModel()
        {
            _services = new ObservableCollection<ServiceInfo>();

            this.SelectedService = null;
            this.IsServiceSelected = false;
        }

        public AppStateViewModel StateView { get { return AppState.Current.View; } }

        public ICommand AddServiceCommand
        {
            get { return new MvxCommand(AddService); }
        }

        public void AddService()
        {
            this.IsServiceEditInProgress = true;
            this.EditedService = null;
        }

        public ICommand EditServiceCommand
        {
            get { return new MvxCommand(AddService); }
        }

        public void EditService()
        {
            this.IsServiceEditInProgress = true;
            this.EditedService = this.SelectedService;
        }

        public ICommand RemoveServiceCommand
        {
            get { return new MvxCommand(AddService); }
        }

        public void RemoveService()
        {
            this.IsServiceEditInProgress = true;
            this.EditedService = this.SelectedService;
        }

        private bool _isServiceEditInProgress;
        public bool IsServiceEditInProgress
        {
            get { return _isServiceEditInProgress; }
            set { _isServiceEditInProgress = value; RaisePropertyChanged(() => IsServiceEditInProgress); }
        }

        private ServiceInfo _editedService;
        public ServiceInfo EditedService
        {
            get { return _editedService; }
            set
            {
                _editedService = value;
                RaisePropertyChanged(() => EditedService);
            }
        }

        private ObservableCollection<ServiceInfo> _services;
        public ObservableCollection<ServiceInfo> Items
        {
            get { return _services; }
            set
            {
                _services = value; RaisePropertyChanged(() => Items);
            }
        }

        private ServiceInfo _selectedService;
        public ServiceInfo SelectedService
        {
            get { return _selectedService; }
            set
            {
                if (_selectedService != value)
                {
                    if (StateView.ActiveService != null)
                        StateView.ActiveService.ResourceSets.SelectedItem = null;
                    if (value == null)
                        StateView.ActiveService = null;
                }
                _selectedService = value;
                RaisePropertyChanged(() => SelectedService);
                this.IsServiceSelected = this.SelectedService != null;
                RaisePropertyChanged(() => IsServiceSelected);
            }
        }

        public ICommand SelectServiceCommand
        {
            get
            {
                return new MvxCommand(SelectService);
            }
        }

        public void SelectService()
        {
            if (this.SelectedService != null)
            {
                if (AppState.ViewModelsWithOwnViews.Contains(typeof(ServiceDetailsViewModel)))
                {
                    ShowViewModel<ServiceDetailsViewModel>(this.SelectedService);
                }
                else
                {
                    StateView.ActiveService = new ServiceDetailsViewModel(this.SelectedService);
                }
            }
            else
            {
                StateView.ActiveService = null;
            }
        }

        private bool _isServiceSelected;
        public bool IsServiceSelected
        {
            get { return _isServiceSelected; }
            set
            {
                _isServiceSelected = value;
                RaisePropertyChanged(() => IsServiceSelected);
            }
        }

        public void DesignModePopulate(IEnumerable<ServiceInfo> services)
        {
            this.Items = new ObservableCollection<ServiceInfo>(services);
        }

        public void DesignModeSetActiveService(ServiceInfo service)
        {
            StateView.ActiveService = service == null ? null : new ServiceDetailsViewModel(this.SelectedService);
        }
    }
}