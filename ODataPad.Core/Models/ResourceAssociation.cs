namespace ODataPad.Core.Models
{
    public class ResourceAssociation
    {
        public ResourceAssociation(string name, string multiplicity)
        {
            this.Name = name;
            this.Multiplicity = multiplicity;
        }

        public string Name { get; private set; }
        public string Multiplicity { get; private set; }
    }
}