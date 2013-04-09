using System;
using System.Windows;
using System.Windows.Controls;
using Cirrious.MvvmCross.Wpf.Commands;

namespace ODataPad.UI.Net45.Common
{
    public class ScrollChangedEventToCommand : MvxWithArgsEventToCommand
    {
        protected override object MapCommandParameter(object parameter)
        {
            var e = parameter as ScrollChangedEventArgs;

            if (e != null && e.VerticalOffset > 0 && e.ViewportHeight + e.VerticalOffset == e.ExtentHeight)
                return true;
            else
                return false;
        }
    }
}