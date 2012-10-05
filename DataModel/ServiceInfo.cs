using System;

namespace ODataPad.DataModel
{
    public class ServiceInfo
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string Logo { get; set; }
        public string MetadataCache { get; set; }
        public DateTimeOffset? CacheUpdated { get; set; }
        public string MetadataCacheFilename { get { return this.Name + ".edmx"; } }
        public int Index { get; set; }
    }
}