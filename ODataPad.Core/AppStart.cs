using Cirrious.CrossCore.IoC;
using Cirrious.MvvmCross.ViewModels;
using ODataPad.Core.ViewModels;

namespace ODataPad.Core
{
    public class AppStart
        : MvxNavigatingObject
        , IMvxAppStart
    {
        public void Start(object hint = null)
        {
            ShowViewModel<HomeViewModel>();
        }
    }
}