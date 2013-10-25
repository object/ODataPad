using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Cirrious.MvvmCross.ViewModels;

namespace ODataPad.Core.ViewModels
{
    public class AppStateViewModel : MvxViewModel
    {
        public AppStateViewModel()
        {
            _resourceSetModes = new ObservableCollection<string>(new[]
                                                                    {
                                                                        "Show collection properties", 
                                                                        "Show collection data"
                                                                    });
            this.ActiveResourceSetMode = ResourceSetModes[0];
        }

        private ServiceDetailsViewModel _activeService;
        public ServiceDetailsViewModel ActiveService
        {
            get { return _activeService; }
            internal set
            {
                _activeService = value;
                RaisePropertyChanged(() => ActiveService);
            }
        }

        private ResourceSetDetailsViewModel _activeResourceSet;
        public ResourceSetDetailsViewModel ActiveResourceSet
        {
            get { return _activeResourceSet; }
            internal set
            {
                _activeResourceSet = value;
                RaisePropertyChanged(() => ActiveResourceSet);
            }
        }

        private ResultDetailsViewModel _activeResult;
        public ResultDetailsViewModel ActiveResult
        {
            get { return _activeResult; }
            internal set
            {
                _activeResult = value;
                RaisePropertyChanged(() => ActiveResult);
            }
        }

        private IList<string> _resourceSetModes;
        public IList<string> ResourceSetModes
        {
            get { return _resourceSetModes; }
            set
            {
                _resourceSetModes = value;
                RaisePropertyChanged(() => ResourceSetModes);
            }
        }

        private string _activeResourceSetMode;
        public string ActiveResourceSetMode
        {
            get { return _activeResourceSetMode; }
            set
            {
                _activeResourceSetMode = value;
                RaisePropertyChanged(() => ActiveResourceSetMode);
                RaisePropertyChanged(() => IsPropertyViewSelected);
                RaisePropertyChanged(() => IsResultViewSelected);
            }
        }

        public bool IsPropertyViewSelected { get { return this.ActiveResourceSetMode == this.ResourceSetModes.First(); } }
        public bool IsResultViewSelected { get { return this.ActiveResourceSetMode == this.ResourceSetModes.Last(); } }

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