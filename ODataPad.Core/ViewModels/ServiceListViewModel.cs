using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Xml.Linq;
using Cirrious.MvvmCross.ViewModels;
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
            if (this.SelectedService != null)
                RefreshServiceMetadataFromCache(this.SelectedService);
            this.IsServiceSelected = this.SelectedService != null;
        }

        private bool _isServiceSelected;
        public bool IsServiceSelected
        {
            get { return _isServiceSelected; }
            set { _isServiceSelected = value; RaisePropertyChanged(() => IsServiceSelected); }
        }

        private void RefreshServiceMetadataFromCache(ServiceDetailsViewModel item)
        {
            this.Home.ResourceSets.Items.Clear();
            if (!string.IsNullOrEmpty(item.MetadataCache))
            {
                var resources = MetadataService.ParseServiceMetadata(item.MetadataCache);
                foreach (var resource in resources)
                {
                    this.Home.ResourceSets.Items.Add(new ResourceSetDetailsViewModel(this.Home, resource));
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