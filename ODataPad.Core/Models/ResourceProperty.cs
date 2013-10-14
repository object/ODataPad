namespace ODataPad.Core.Models
{
    public class ResourceProperty
    {
        public ResourceProperty(string name, string type, bool isKey, bool isNullable)
        {
            this.Name = name;
            this.Type = type;
            this.IsKey = isKey;
            this.IsNullable = isNullable;
        }

        public string Name { get; private set; }
        public string Type { get; private set; }
        public bool IsKey { get; private set; }
        public bool IsNullable { get; private set; }
    }
}