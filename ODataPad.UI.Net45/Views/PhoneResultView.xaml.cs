using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Cirrious.MvvmCross.Wpf.Views;
using ODataPad.Core.ViewModels;

namespace ODataPad.UI.Net45.Views
{
    /// <summary>
    /// Interaction logic for PhoneResultView.xaml
    /// </summary>
    [FormFactor(FormFactor.Phone)]
    public partial class PhoneResultView : MvxWpfView
    {
        public PhoneResultView()
        {
            InitializeComponent();
        }

        public new ResultDetailsViewModel ViewModel
        {
            get { return base.ViewModel as ResultDetailsViewModel; }
            set { base.ViewModel = value; }
        }

        public void Connect(int connectionId, object target)
        {
            throw new NotImplementedException();
        }
    }
}
