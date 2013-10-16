using ODataPad.Core.Interfaces;

namespace ODataPad.Core.ViewModels
{
    class QueryInProgress : INotifyInProgress
    {
        private readonly ResultListViewModel _viewModel;

        public QueryInProgress(ResultListViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public bool IsInProgress
        {
            get { return _viewModel.IsQueryInProgress; }
            set { _viewModel.IsQueryInProgress = value; }
        }
    }
}