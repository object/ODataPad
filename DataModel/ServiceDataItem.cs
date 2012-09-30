using System;
using System.Collections.Generic;

namespace ODataPad.DataModel
{
    /// <summary>
    /// Generic item data model.
    /// </summary>
    public class ServiceDataItem : ServiceDataCommon
    {
        public ServiceDataItem(String uniqueId, String name, String uri, String imagePath, String description, String content, ServiceDataGroup group)
            : base(uniqueId, name, uri, imagePath, description)
        {
            this._content = content;
            this._group = group;
        }

        private string _content = string.Empty;
        public string Content
        {
            get { return this._content; }
            set { this.SetProperty(ref this._content, value); }
        }

        private IList<string> _collections = new List<string>();
        public IList<string> Collections
        {
            get { return this._collections; }
            set { this.SetProperty(ref this._collections, value); }
        }

        private ServiceDataGroup _group;
        public ServiceDataGroup Group
        {
            get { return this._group; }
            set { this.SetProperty(ref this._group, value); }
        }
    }
}