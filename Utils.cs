using System;
using System.Xml.Linq;

namespace ODataPad
{
    public class Utils
    {
        public static string TryGetStringValue(XElement parent, string elementName)
        {
            var element = parent.Element(elementName);
            return element == null ?
                null : 
                element.Value;
        }

        public static int TryGetIntValue(XElement parent, string elementName)
        {
            var element = parent.Element(elementName);
            return element == null ?
                0 : string.IsNullOrEmpty(element.Value) ?
                0 :
                int.Parse(element.Value);
        }

        public static DateTimeOffset? TryGetDateTimeValue(XElement parent, string elementName)
        {
            var element = parent.Element(elementName);
            return element == null ?
                null : string.IsNullOrEmpty(element.Value) ?
                null :
                new DateTimeOffset?(DateTimeOffset.Parse(element.Value));
        }
    }
}