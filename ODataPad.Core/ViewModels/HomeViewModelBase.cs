using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;
using ODataPad.Core.Models;

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
        public IList<ServiceDetailsViewModel> ServiceDetails { get { return Services.Items; } }
        public ServiceDetailsViewModel SelectedServiceDetails { get { return Services.SelectedService; } }
        public ResourceSetListViewModel ResourceSets { get { return SelectedServiceDetails.ResourceSets; } }
        public IList<ResourceSetDetailsViewModel> ResourceSetDetails { get { return ResourceSets.Items; } }
        public ResourceSetDetailsViewModel SelectedResourceSetDetails { get { return ResourceSets.SelectedItem; } }
        public string SelectedSchemaSummary { get { return SelectedResourceSetDetails.Summary; } }
        public ResultListViewModel Results { get { return SelectedResourceSetDetails.Results; } }
        public ObservableResultCollection ResultDetails { get { return Results.QueryResults; } }
        public ResultDetailsViewModel SelectedResultDetails { get { return Results.SelectedResult; } set { Results.SelectedResult = value; } }
        public string SelectedResultSummary { get { return Results.SelectedResultDetails; } }
        public bool IsSingleResultSelected { get { return Results.IsSingleResultSelected; } }

        public virtual Task Init(HomeViewModel.NavigationParameters parameters)
        {
            return InitAsync(parameters);
        }

        public virtual Task InitAsync(HomeViewModel.NavigationParameters parameters)
        {
            return new Task(() => { });
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