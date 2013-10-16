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
    }
}