using ODataPad.Core.Interfaces;

namespace ODataPad.Core.ViewModels
{
    class QueryInProgress : INotifyInProgress
    {
        private readonly HomeViewModel _viewModel;

        public QueryInProgress(HomeViewModel viewModel)
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