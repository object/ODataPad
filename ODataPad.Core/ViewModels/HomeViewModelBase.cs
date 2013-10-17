using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;

namespace ODataPad.Core.ViewModels
{
    public class HomeViewModelBase : MvxViewModel
    {
        private readonly ServiceListViewModel _services;

        protected HomeViewModelBase()
        {
            _services = new ServiceListViewModel(this);
        }

        public virtual bool IsDesignTime { get { return true; } }
        public ServiceListViewModel Services { get { return _services; } }

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

        public IEnumerable<string> ResourceSetModes
        {
            get { return ResourceSetDetailsViewModel.ResourceSetModes; }
        }

        public string SelectedResourceSetMode
        {
            get { return ResourceSetDetailsViewModel.ResourceSetMode; }
            set
            {
                if (this.Services.SelectedService.ResourceSets.SelectedItem != null)
                {
                    this.Services.SelectedService.ResourceSets.SelectedItem.SelectedResourceSetMode = value;
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