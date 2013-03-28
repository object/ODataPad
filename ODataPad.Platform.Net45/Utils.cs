
using System;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace ODataPad.Platform.Net45
{
    public static class Utils
    {
        public static Task ExecuteOnUIThread(Action action)
        {
            return Task.Factory.StartNew(() => Dispatcher.CurrentDispatcher.Invoke(action));
        }
    }
}