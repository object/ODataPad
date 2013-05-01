using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Cirrious.MvvmCross.WindowsPhone.Views;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using ODataPad.Core.ViewModels;
using ODataPad.UI.WP8.Resources;

namespace ODataPad.UI.WP8
{
    public partial class MainPage : MvxPhonePage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        public new HomeViewModel ViewModel
        {
            get { return (HomeViewModel)base.ViewModel; }
            set { base.ViewModel = value; }
        }
    }
}