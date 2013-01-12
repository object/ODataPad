using Windows.Foundation;
using Windows.UI.Core;

namespace ODataPad.WinRT
{
    public static class Utils
    {
        public static IAsyncAction ExecuteOnUIThread(DispatchedHandler action)
        {
            return Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                Windows.UI.Core.CoreDispatcherPriority.Normal, 
                action);
        }
    }
}