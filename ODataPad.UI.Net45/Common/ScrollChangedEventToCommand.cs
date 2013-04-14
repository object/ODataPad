using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Cirrious.MvvmCross.Wpf.Commands;
using ODataPad.Core.Models;
using ODataPad.Core.ViewModels;

namespace ODataPad.UI.Net45.Common
{
    public class ScrollChangedEventToCommand : MvxWithArgsEventToCommand
    {
        protected override object MapCommandParameter(object parameter)
        {
            var e = parameter as ScrollChangedEventArgs;

            if (e != null && (int) e.ExtentHeightChange == 0 && e.VerticalOffset > 0 &&
                (int) (e.ViewportHeight + e.VerticalOffset) == (int) e.ExtentHeight)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}