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
                if (_data is ServiceCollection)
                    return (_data as ServiceCollection).Name;
                if (_data is CollectionProperty)
                    return (_data as CollectionProperty).Name;
                if (_data is CollectionAssociation)
                    return (_data as CollectionAssociation).Name;
                if (_data is ResultRow)
                    return (_data as ResultRow).KeySummary;
                if (_data is ViewableItem)
                    return (_data as ViewableItem).Title;

                return "Unknown";
            }
            //set { this.SetProperty(ref this._title, value); }
        }

        public string Subtitle
        {
            get
            {
                if (_data is ServiceCollection)
                    return (_data as ServiceCollection).Summary;
                if (_data is CollectionProperty)
                    return (_data as CollectionProperty).Summary;
                if (_data is CollectionAssociation)
                    return (_data as CollectionAssociation).Multiplicity;
                if (_data is ResultRow)
                    return (_data as ResultRow).ValueSummary;
                if (_data is ViewableItem)
                    return (_data as ViewableItem).Subtitle;

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

        private ObservableCollection<ViewableItem> _elements = new ObservableCollection<ViewableItem>();
        public ObservableCollection<ViewableItem> Elements
        {
            get { return this._elements; }
            set { this.SetProperty(ref this._elements, value); }
        }

        public override string ToString()
        {
            return this.Title;
        }
    }
}