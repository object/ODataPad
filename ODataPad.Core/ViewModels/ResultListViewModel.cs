using System;
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
        private readonly IResultProvider _resultProvider;

        public ResultListViewModel(HomeViewModelBase home)
        {
            if (!home.IsDesignTime)
                _resultProvider = Mvx.Resolve<IResultProvider>();

            this.Home = home;
            this.IsQueryInProgress = false;
        }

        public HomeViewModelBase Home { get; set; }

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

        private ResultViewModel _selectedResult;
        public ResultViewModel SelectedResult
        {
            get { return _selectedResult; }
            set
            {
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
            get { return this.Home.IsQueryInProgress; }
            set { this.Home.IsQueryInProgress = value; }
        }

        public ICommand LoadMoreResultsCommand
        {
            get { return new MvxCommand<bool>(x => { if (x) LoadMoreResults(); }); }
        }

        public void LoadMoreResults()
        {
            if (this.Home.IsDesignTime)
                return;

            if (this.QueryResults != null
                && this.QueryResults.HasMoreItems
                && !this.IsQueryInProgress)
            {
                _resultProvider.AddResultsAsync(this.QueryResults);
            }
        }

        public async Task RequestResourceData()
        {
            this.QueryResults = _resultProvider.CreateResultCollection(
                this.Home.SelectedService.Url,
                this.Home.SelectedResourceSet.Name,
                this.Home.SelectedResourceSet.Properties,
                new QueryInProgress(this));

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