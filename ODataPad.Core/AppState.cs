using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Cirrious.MvvmCross.ViewModels;
using ODataPad.Core.ViewModels;

namespace ODataPad.Core
{
    public class AppState : MvxNotifyPropertyChanged
    {
        private static readonly AppState Instance = new AppState();

        static AppState()
        {
            ViewModelsWithOwnViews = new List<Type>(new[] { typeof(HomeViewModel) });
        }

        public AppState()
        {
            _resourceSetModes = new ObservableCollection<string>(new[]
                                                                    {
                                                                        "Show collection properties", 
                                                                        "Show collection data"
                                                                    });
            this.ActiveResourceSetMode = ResourceSetModes[0];
        }

        public static AppState Current { get { return Instance; } }
        public static List<Type> ViewModelsWithOwnViews { get; private set; }

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