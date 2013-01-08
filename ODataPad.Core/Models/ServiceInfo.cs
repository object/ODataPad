using System;
using System.Xml.Linq;

namespace ODataPad.Core.Models
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

        public static ServiceInfo Parse(string xml)
        {
            return Parse(XElement.Parse(xml));
        }

        public static ServiceInfo Parse(XElement element)
        {
            return new ServiceInfo()
            {
                Name = Utils.TryGetStringValue(element, "Name"),
                Url = Utils.TryGetStringValue(element, "Url"),
                Description = Utils.TryGetStringValue(element, "Description"),
                Logo = Utils.TryGetStringValue(element, "Logo"),
                CacheUpdated = Utils.TryGetDateTimeValue(element, "CacheUpdated"),
                Index = Utils.TryGetIntValue(element, "Index"),
            };
        }

        public string Format()
        {
            var element = new XElement("Service");
            element.Add(new XElement("Name", this.Name));
            element.Add(new XElement("Url", this.Url));
            element.Add(new XElement("Description", this.Description));
            element.Add(new XElement("Logo", this.Logo));
            element.Add(new XElement("CacheUpdated", this.CacheUpdated));
            element.Add(new XElement("Index", this.Index));
            return element.ToString();
        }
    }
}