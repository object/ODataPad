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

        public AppState AppState { get { return AppState.Current; } }

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
                _selectedService = value;
                RaisePropertyChanged(() => SelectedService);
                RaisePropertyChanged(() => IsServiceSelected);
            }
        }

        public ICommand SelectServiceCommand
        {
            get { return new MvxCommand(SelectService); }
        }

        public void SelectService()
        {
            this.IsServiceSelected = this.SelectedService != null;
            if (this.SelectedService != null)
            {
                if (AppState.ViewModelsWithOwnViews.Contains(typeof (ServiceDetailsViewModel)))
                {
                    ShowViewModel<ServiceDetailsViewModel>(this.SelectedService);
                }
                else
                {
                    AppState.UI.ActiveService = new ServiceDetailsViewModel(this.SelectedService);
                }
            }
            else
            {
                AppState.UI.ActiveService = null;
            }
        }

        private bool _isServiceSelected;
        public bool IsServiceSelected
        {
            get { return _isServiceSelected; }
            set { _isServiceSelected = value; RaisePropertyChanged(() => IsServiceSelected); }
        }

        public void Populate(IEnumerable<ServiceInfo> services)
        {
            this.Items = new ObservableCollection<ServiceInfo>(services);
        }
    }
}