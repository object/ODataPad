using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODataPad.UI.Net45
{
    public class Program
    {
        [STAThread]
        public static void Main()
        {
            var app = new App();
            var ourWindow = new MainWindow();
            var presenter = new SimplePresenter(ourWindow);
            var setup = new Setup(app.Dispatcher, presenter);
            setup.Initialize();
            app.MainWindow.Show();
            app.Run();
        }
    }
}
