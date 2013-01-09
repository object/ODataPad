using System;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using ODataPad.Core.Models;
using ODataPad.WinRT;
using ODataPad.UI.WinRT.Common;

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
                if (_data is ServiceInfo)
                    return (_data as ServiceInfo).Name;
                if (_data is ServiceCollection)
                    return (_data as ServiceCollection).Name;
                if (_data is CollectionProperty)
                    return (_data as CollectionProperty).Name;
                if (_data is CollectionAssociation)
                    return (_data as CollectionAssociation).Name;
                if (_data is ResultRow)
                    return (_data as ResultRow).KeySummary;
                if (_data is ServiceError)
                    return (_data as ServiceError).ErrorMessage;
                if (_data is ViewableItem)
                    return (_data as ViewableItem).Title;

                return string.Empty;
            }
        }

        public string Subtitle
        {
            get
            {
                if (_data is ServiceInfo)
                    return (_data as ServiceInfo).Url;
                if (_data is ServiceCollection)
                    return (_data as ServiceCollection).Summary;
                if (_data is CollectionProperty)
                    return (_data as CollectionProperty).Summary;
                if (_data is CollectionAssociation)
                    return (_data as CollectionAssociation).Multiplicity;
                if (_data is ResultRow)
                    return (_data as ResultRow).ValueSummary;
                if (_data is ServiceError)
                    return (_data as ServiceError).ErrorDescription;
                if (_data is ViewableItem)
                    return (_data as ViewableItem).Subtitle;

                return string.Empty;
            }
        }

        public string Description
        {
            get
            {
                if (_data is ServiceInfo)
                    return (_data as ServiceInfo).Description;

                return string.Empty;
            }
        }

        public string ImagePath
        {
            get
            {
                if (_data is ServiceInfo)
                    return (_data as ServiceInfo).GetImagePath();

                return string.Empty;
            }
        }

        private ImageSource _image = null;
        public ImageSource Image
        {
            get
            {
                if (_image == null && this.ImagePath != null)
                {
                    var resourcePath = new ResourceManager().GetImageResourcePath("Samples", this.ImagePath);
                    this._image = new BitmapImage(new Uri(resourcePath));
                }
                return this._image;
            }
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