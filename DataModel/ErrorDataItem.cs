using System.Xml.Linq;

namespace ODataPad.DataModel
{
    public class ErrorDataItem : DataItem
    {
        public ErrorDataItem(ServiceInfo service, XElement element)
            : base(GetUniqueId(service.Name, element.Name), element.Name.ToString(), "Unable to load service metadata", null, GetErrorDescription(element))
        {
        }

        private static string GetErrorDescription(XElement element)
        {
            return element.Element("Message").Value;
        }
    }
}