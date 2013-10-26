using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cirrious.MvvmCross.ViewModels;

namespace ODataPad.Core.ViewModels
{
    public class HomeViewModelBase : MvxViewModel
    {
        private readonly ServiceListViewModel _services;

        protected HomeViewModelBase()
        {
            _services = new ServiceListViewModel();
        }

        public static bool IsDesignTime { get; protected set; }

        public ServiceListViewModel Services { get { return _services; } }

        public virtual Task Init()
        {
            return InitAsync();
        }

        public virtual Task InitAsync()
        {
            return new Task(() => { });
        }

        public AppState AppState { get { return AppState.Current; } }
    }
}