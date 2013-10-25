using System;
using System.Collections.Generic;
using Cirrious.MvvmCross.ViewModels;
using ODataPad.Core.ViewModels;

namespace ODataPad.Core
{
    public class AppStateViewModel : MvxViewModel
    {
        private ServiceDetailsViewModel _activeService;
        public ServiceDetailsViewModel ActiveService
        {
            get { return _activeService; }
            set
            {
                _activeService = value;
                RaisePropertyChanged(() => ActiveService);
            }
        }

        private ResourceSetDetailsViewModel _activeResourceSet;
        public ResourceSetDetailsViewModel ActiveResourceSet
        {
            get { return _activeResourceSet; }
            set
            {
                _activeResourceSet = value;
                RaisePropertyChanged(() => ActiveResourceSet);
            }
        }

        private ResultDetailsViewModel _activeResult;
        public ResultDetailsViewModel ActiveResult
        {
            get { return _activeResult; }
            set
            {
                _activeResult = value;
                RaisePropertyChanged(() => _activeResult);
            }
        }
    }

    public class AppState
    {
        private static readonly AppState Instance = new AppState();

        static AppState()
        {
            ViewModelsWithOwnViews = new List<Type>(new[] { typeof(HomeViewModel) });
        }

        public AppState()
        {
            this.UI = new AppStateViewModel();
        }

        public static AppState Current { get { return Instance; } }
        public static List<Type> ViewModelsWithOwnViews { get; private set; }

        public AppStateViewModel UI { get; private set; }

        public bool IsQueryInProgress { get; set; }
    }
}