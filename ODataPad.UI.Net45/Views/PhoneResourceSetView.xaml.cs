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
    /// Interaction logic for PhoneResourceSetView.xaml
    /// </summary>
    [FormFactor(FormFactor.Phone)]
    public partial class PhoneResourceSetView : MvxWpfView
    {
        public PhoneResourceSetView()
        {
            InitializeComponent();
        }

        public new ResourceSetDetailsViewModel ViewModel
        {
            get { return base.ViewModel as ResourceSetDetailsViewModel; }
            set { base.ViewModel = value; }
        }

        public void Connect(int connectionId, object target)
        {
            throw new NotImplementedException();
        }
    }
}
