using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ODataPad.DataModel
{
    /// <summary>
    /// Generic item data model.
    /// </summary>
    public class ServiceDataItem : DataItem
    {
        public ServiceDataItem(ServiceInfo service)
            : base(GetUniqueId(service.Name), service.Name, service.Url, GetImagePath(service), service.Description)
        {
            _metadataCache = service.MetadataCache;
        }

        private string _metadataCache = string.Empty;
        public string MetadataCache
        {
            get { return this._metadataCache; }
            set { this.SetProperty(ref this._metadataCache, value); }
        }

        private static string GetImagePath(ServiceInfo service)
        {
            return "Samples/" + (string.IsNullOrEmpty(service.Logo) ? service.Name : service.Logo) + ".png";
        }
    }
}