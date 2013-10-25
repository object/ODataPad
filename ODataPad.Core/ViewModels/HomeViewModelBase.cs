using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cirrious.MvvmCross.ViewModels;
using ODataPad.Core.Models;

namespace ODataPad.Core.ViewModels
{
    public class HomeViewModelBase : MvxViewModel
    {
        private readonly ServiceListViewModel _services;

        protected HomeViewModelBase()
        {
            _services = new ServiceListViewModel();
        }

        public static bool IsDesignTime { get; protected set; }

        public ServiceListViewModel Services { get { return _services; } }

        public virtual Task Init()
        {
            return InitAsync();
        }

        public virtual Task InitAsync()
        {
            return new Task(() => { });
        }

        public AppState AppState { get { return AppState.Current; } }

        public IEnumerable<string> ResourceSetModes
        {
            get { return ResourceSetDetailsViewModel.ResourceSetModes; }
        }

        public string SelectedResourceSetMode
        {
            get { return ResourceSetDetailsViewModel.ResourceSetMode; }
            set
            {
                if (AppState.ActiveService.ResourceSets.SelectedItem != null)
                {
                    AppState.ActiveService.ResourceSets.SelectedItem.SelectedResourceSetMode = value;
                }
                else
                {
                    ResourceSetDetailsViewModel.ResourceSetMode = value;
                    RaisePropertyChanged(() => SelectedResourceSetMode);
                }
            }
        }

        public bool IsQueryInProgress
        {
            get { return AppState.Current.IsQueryInProgress; }
            set
            {
                if (AppState.ActiveService != null &&
                    AppState.ActiveResourceSet != null &&
                    AppState.ActiveResourceSet.Results != null)
                {
                    AppState.IsQueryInProgress = value;
                }
                else
                {
                    AppState.Current.IsQueryInProgress = value;
                    RaisePropertyChanged(() => IsQueryInProgress);
                }
            }
        }
    }
}