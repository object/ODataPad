﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using Simple.OData.Client;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace ODataPad.DataModel
{
    [Windows.Foundation.Metadata.WebHostHidden]
    public abstract class DataItem : ODataPad.Common.BindableBase
    {
        private static Uri _baseUri = new Uri("ms-appx:///");

        public DataItem(String uniqueId, String title, String subtitle, String imagePath, String description)
        {
            this._uniqueId = uniqueId;
            this._title = title;
            this._subtitle = subtitle;
            this._description = description;
            this._imagePath = imagePath;
        }

        private string _uniqueId = string.Empty;
        public string UniqueId
        {
            get { return this._uniqueId; }
            set { this.SetProperty(ref this._uniqueId, value); }
        }

        private string _title = string.Empty;
        public string Title
        {
            get { return this._title; }
            set { this.SetProperty(ref this._title, value); }
        }

        private string _subtitle = string.Empty;
        public string Subtitle
        {
            get { return this._subtitle; }
            set { this.SetProperty(ref this._subtitle, value); }
        }

        private string _description = string.Empty;
        public string Description
        {
            get { return this._description; }
            set { this.SetProperty(ref this._description, value); }
        }

        private String _imagePath = null;
        public string ImagePath
        {
            get { return this._imagePath; }
            set { this.SetProperty(ref this._imagePath, value); }
        }

        private ImageSource _image = null;
        public ImageSource Image
        {
            get
            {
                if (this._image == null && this._imagePath != null)
                {
                    this._image = new BitmapImage(new Uri(DataItem._baseUri, this._imagePath));
                }
                return this._image;
            }

            set
            {
                this._imagePath = null;
                this.SetProperty(ref this._image, value);
            }
        }

        public void SetImage(String path)
        {
            this._image = null;
            this._imagePath = path;
            this.OnPropertyChanged("Image");
        }

        private ObservableCollection<DataItem> _elements = new ObservableCollection<DataItem>();
        public ObservableCollection<DataItem> Elements
        {
            get { return this._elements; }
            set { this.SetProperty(ref this._elements, value); }
        }

        public override string ToString()
        {
            return this.Title;
        }

        protected static string GetUniqueId(params object[] items)
        {
            return string.Join("/", items);
        }
    }
}