using System.Collections.ObjectModel;
using Cirrious.MvvmCross.ViewModels;

namespace ODataPad.Core.ViewModels
{
    public class BaseHomeViewModel : MvxViewModel
    {
        public BaseHomeViewModel()
        {
            _services = new ObservableCollection<ServiceItem>();
        }

        public BaseHomeViewModel Self { get { return this; } }

        private ObservableCollection<ServiceItem> _services;
        public ObservableCollection<ServiceItem> Services
        {
            get { return _services; }
            set { _services = value; RaisePropertyChanged("Services"); }
        }
    }
}