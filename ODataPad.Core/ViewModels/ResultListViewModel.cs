using System;
using System.Linq;
using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;
using ODataPad.Core.Models;

namespace ODataPad.Core.ViewModels
{
    public class ResultListViewModel : MvxViewModel
    {
        public ResultListViewModel(HomeViewModelBase home)
        {
            this.Home = home;
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

        public bool IsSingleResultSelected { get { return this.SelectedResult != null; } }

        private void ShowResultDetails()
        {
            this.SelectedResultDetails = string.Join(Environment.NewLine + Environment.NewLine,
                this.SelectedResult.Properties
                    .Select(y => y.Key + Environment.NewLine + (y.Value == null ? "(null)" : y.Value.ToString())));
        }
    }
}