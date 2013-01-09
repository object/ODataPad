using System;
using System.Collections.ObjectModel;
using ODataPad.Core.Models;
using ODataPad.UI.WinRT.Common;
using ODataPad.WinRT;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace ODataPad.UI.WinRT
{
    [Windows.Foundation.Metadata.WebHostHidden]
    public class ViewableItem : BindableBase
    {
        private object _data;

        public ViewableItem(object data)
        {
            _data = data;
        }

        public object Data
        {
            get { return _data; }
        }

        public string Title
        {
            get
            {
                if (_data is CollectionProperty)
                    return (_data as CollectionProperty).Name;
                if (_data is CollectionAssociation)
                    return (_data as CollectionAssociation).Name;
                if (_data is ResultRow)
                    return (_data as ResultRow).KeySummary;

                return "Unknown";
            }
            //set { this.SetProperty(ref this._title, value); }
        }

        public string Subtitle
        {
            get
            {
                if (_data is CollectionProperty)
                    return (_data as CollectionProperty).Summary;
                if (_data is CollectionAssociation)
                    return (_data as CollectionAssociation).Multiplicity;
                if (_data is ResultRow)
                    return (_data as ResultRow).PropertySummary;

                return "Unknown";
            }
            //set { this.SetProperty(ref this._subtitle, value); }
        }

        public string Description
        {
            get
            {
                return "d";
                //set { this.SetProperty(ref this._description, value); }
            }
        }

        public string ImagePath
        {
            get
            {
                return "i";
            }
            //set { this.SetProperty(ref this._imagePath, value); }
        }

        public override string ToString()
        {
            return this.Title;
        }
    }
}