using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Xml.Linq;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.ViewModels;
using Cirrious.MvvmCross.Views;
using ODataPad.Core.Models;
using ODataPad.Core.Services;

namespace ODataPad.Core.ViewModels
{
    public class ServiceListViewModel : MvxViewModel
    {
        public ServiceListViewModel(HomeViewModelBase home)
        {
            this.Home = home;

            _services = new ObservableCollection<ServiceDetailsViewModel>();

            this.SelectedService = null;
            this.IsServiceSelected = false;
        }

        public HomeViewModelBase Home { get; set; }

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
        public ObservableCollection<ServiceDetailsViewModel> Items
        {
            get { return _services; }
            set
            {
                _services = value; RaisePropertyChanged(() => Items);
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

        public void SelectService()
        {
            this.IsServiceSelected = this.SelectedService != null;
            if (this.SelectedService != null)
            {
                RefreshServiceMetadataFromCache(this.SelectedService);

                if (ODataPadApp.ViewModelsWithOwnViews.Contains(typeof (ServiceDetailsViewModel)))
                {
                    ShowViewModel<ServiceDetailsViewModel>(this.SelectedService.ServiceInfo);
                }
            }
        }

        private bool _isServiceSelected;
        public bool IsServiceSelected
        {
            get { return _isServiceSelected; }
            set { _isServiceSelected = value; RaisePropertyChanged(() => IsServiceSelected); }
        }

        public void Populate(IEnumerable<ServiceDetailsViewModel> details)
        {
            this.Items = new ObservableCollection<ServiceDetailsViewModel>(details);
        }

        public void SelectTopItem()
        {
            this.SelectedService = this.Items.First();
            this.IsServiceSelected = true;
        }

        private void RefreshServiceMetadataFromCache(ServiceDetailsViewModel item)
        {
            this.SelectedService.ResourceSets.Items.Clear();
            if (!string.IsNullOrEmpty(item.MetadataCache))
            {
                var resources = MetadataService.ParseServiceMetadata(item.MetadataCache);
                foreach (var resource in resources)
                {
                    this.SelectedService.ResourceSets.Items.Add(new ResourceSetDetailsViewModel(this.Home, resource));
                }
            }
        }

        private void RefreshServiceMetadataFromCache(ServiceDetailsViewModel item, ServiceInfo service)
        {
            //item.Resources.Clear();
            if (!string.IsNullOrEmpty(service.MetadataCache))
            {
                var element = XElement.Parse(service.MetadataCache);
                if (element.Name == "Error")
                {
                    //item.Resources.Add(new ServiceError(
                    //    service.Name, 
                    //    "Unable to load service metadata",
                    //    element.Element("Message").Value));
                }
                else
                {
                    var resources = MetadataService.ParseServiceMetadata(service.MetadataCache);
                    foreach (var resource in resources)
                    {
                        //item.Resources.Add(collection);
                    }
                }
            }
        }
    }
}