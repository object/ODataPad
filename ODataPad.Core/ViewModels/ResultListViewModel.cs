using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.ViewModels;
using ODataPad.Core.Interfaces;
using ODataPad.Core.Models;

namespace ODataPad.Core.ViewModels
{
    public class ResultListViewModel : MvxViewModel
    {
        private IResultProvider _resultProvider;
        private string _serviceUrl;
        private string _resourceSetName;
        private List<ResourceProperty> _properties;

        public ResultListViewModel()
        {
        }

        public ResultListViewModel(string serviceUrl, string resourceSetName, IEnumerable<ResourceProperty> properties)
        {
            Init(new NavObject
            {
                ServiceUrl = serviceUrl,
                ResourceSetName = resourceSetName,
                Properties = new List<ResourceProperty>(properties),
            });
        }

        public void Init(NavObject navObject)
        {
            if (!HomeViewModelBase.IsDesignTime)
                _resultProvider = Mvx.Resolve<IResultProvider>();

            _serviceUrl = navObject.ServiceUrl;
            _resourceSetName = navObject.ResourceSetName;
            _properties = navObject.Properties;
        }

        public class NavObject
        {
            public string ServiceUrl { get; set; }
            public string ResourceSetName { get; set; }
            public List<ResourceProperty> Properties { get; set; }
        }

        public class SavedState
        {
            public string ServiceUrl { get; set; }
            public string ResourceSetName { get; set; }
            public List<ResourceProperty> Properties { get; set; }
        }

        public SavedState SaveState()
        {
            return new SavedState()
            {
                ServiceUrl = _serviceUrl,
                ResourceSetName = _resourceSetName,
                Properties = _properties,
            };
        }

        public void ReloadState(SavedState savedState)
        {
            Init(new NavObject
            {
                ServiceUrl = savedState.ServiceUrl,
                ResourceSetName = savedState.ResourceSetName,
                Properties = savedState.Properties
            });
        }

        public AppStateViewModel StateView { get { return AppState.Current.View; } }

        private ObservableResultCollection _queryResults;
        public ObservableResultCollection QueryResults
        {
            get { return _queryResults; }
            set { _queryResults = value; RaisePropertyChanged(() => QueryResults); }
        }

        private string _selectedResultDetails;
        public string SelectedResultDetails
        {
            get { return _selectedResultDetails; }
            set { _selectedResultDetails = value; RaisePropertyChanged(() => SelectedResultDetails); }
        }

        private ResultDetailsViewModel _selectedResult;
        public ResultDetailsViewModel SelectedResult
        {
            get { return _selectedResult; }
            set
            {
                if (_selectedResult != value)
                {
                    StateView.ActiveResult = null;
                }
                _selectedResult = value;
                RaisePropertyChanged(() => SelectedResult);
                RaisePropertyChanged(() => IsSingleResultSelected);
            }
        }

        public ICommand SelectResultCommand
        {
            get { return new MvxCommand(SelectResult); }
        }

        public void SelectResult()
        {
            if (this.SelectedResult != null)
            {
                ShowResultDetails();
            }
        }

        public ICommand UnselectResultCommand
        {
            get { return new MvxCommand(CollapseResult); }
        }

        public virtual void CollapseResult()
        {
            this.SelectedResult = null;
        }

        public bool IsQueryInProgress
        {
            get { return AppState.Current.View.IsQueryInProgress; }
            set
            {
                AppState.Current.View.IsQueryInProgress = value;
                RaisePropertyChanged(() => IsQueryInProgress);
            }
        }

        public ICommand LoadMoreResultsCommand
        {
            get { return new MvxCommand<bool>(x => { if (x) LoadMoreResults(); }); }
        }

        public void LoadMoreResults()
        {
            if (HomeViewModelBase.IsDesignTime)
                return;

            if (this.QueryResults != null
                && this.QueryResults.HasMoreItems
                && !this.IsQueryInProgress)
            {
                _resultProvider.AddResultsAsync(this.QueryResults);
            }
        }

        public async Task LoadResultsAsync()
        {
            this.QueryResults = _resultProvider.CreateResultCollection(
                _serviceUrl,
                _resourceSetName,
                _properties,
                new QueryInProgress(x => IsQueryInProgress = x));

            await _resultProvider.AddResultsAsync(this.QueryResults);
        }

        public bool IsSingleResultSelected { get { return this.SelectedResult != null; } }

        private void ShowResultDetails()
        {
            this.SelectedResultDetails = string.Join(Environment.NewLine + Environment.NewLine,
                this.SelectedResult.Properties
                    .Select(y => y.Key + Environment.NewLine + (y.Value == null ? "(null)" : y.Value.ToString())));
        }
    }
}