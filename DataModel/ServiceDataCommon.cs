using System;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace ODataPad.DataModel
{
    /// <summary>
    /// Base class for <see cref="ServiceDataItem"/> and <see cref="ServiceDataGroup"/> that
    /// defines properties common to both.
    /// </summary>
    [Windows.Foundation.Metadata.WebHostHidden]
    public abstract class ServiceDataCommon : ODataPad.Common.BindableBase
    {
        private static Uri _baseUri = new Uri("ms-appx:///");

        public ServiceDataCommon(String uniqueId, String name, String uri, String imagePath, String description)
        {
            this._uniqueId = uniqueId;
            this._name = name;
            this._uri = uri;
            this._description = description;
            this._imagePath = imagePath;
        }

        private string _uniqueId = string.Empty;
        public string UniqueId
        {
            get { return this._uniqueId; }
            set { this.SetProperty(ref this._uniqueId, value); }
        }

        private string _name = string.Empty;
        public string Name
        {
            get { return this._name; }
            set { this.SetProperty(ref this._name, value); }
        }

        private string _uri = string.Empty;
        public string Uri
        {
            get { return this._uri; }
            set { this.SetProperty(ref this._uri, value); }
        }

        private string _description = string.Empty;
        public string Description
        {
            get { return this._description; }
            set { this.SetProperty(ref this._description, value); }
        }

        private ImageSource _image = null;
        private String _imagePath = null;
        public ImageSource Image
        {
            get
            {
                if (this._image == null && this._imagePath != null)
                {
                    this._image = new BitmapImage(new Uri(ServiceDataCommon._baseUri, this._imagePath));
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

        public override string ToString()
        {
            return this.Name;
        }
    }
}