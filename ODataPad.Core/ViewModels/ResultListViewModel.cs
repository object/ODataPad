using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;
using ODataPad.Core.Interfaces;
using ODataPad.Core.Models;

namespace ODataPad.Core.ViewModels
{
    public partial class ResultListViewModel : MvxViewModel
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

        public AppState AppState { get { return AppState.Current; } }

        private ObservableResultCollection _queryResults;
        public ObservableResultCollection QueryResults
        {
            get { return _queryResults; }
            set { _queryResults = value; RaisePropertyChanged(() => QueryResults); }
        }

        private ResultInfo _selectedResult;
        public ResultInfo SelectedResult
        {
            get { return _selectedResult; }
            set
            {
                if (_selectedResult != value)
                {
                    AppState.ActiveResult = null;
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
                if (AppState.ViewModelsWithOwnViews.Contains(typeof (ResultDetailsViewModel)))
                {
                    ShowViewModel<ResultDetailsViewModel>(new ResultDetailsViewModel.NavObject(_selectedResult));
                }
                else
                {
                    AppState.ActiveResult = new ResultDetailsViewModel(this.SelectedResult);
                }
            }
            else
            {
                AppState.ActiveResult = null;
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
            get { return AppState.IsQueryInProgress; }
            set
            {
                AppState.IsQueryInProgress = value;
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
    }
}