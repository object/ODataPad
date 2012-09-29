using System;

namespace ODataPad.DataModel
{
    public class ServiceInfo
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Uri { get; set; }
        public string MetadataCache { get; set; }
        public DateTimeOffset CacheUpdated { get; set; }
    }
}