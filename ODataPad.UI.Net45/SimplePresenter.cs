using System.Windows;
using Cirrious.MvvmCross.Wpf.Views;

namespace ODataPad.UI.Net45
{
    public class SimplePresenter : MvxWpfViewPresenter
    {
        private readonly MainWindow _mainWindow;

        public SimplePresenter(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
        }

        public override void Present(FrameworkElement frameworkElement)
        {
            _mainWindow.Present(frameworkElement);
        }
    }
}