namespace ODataPad.Core.Models
{
    public class CollectionAssociation
    {
        public CollectionAssociation(string name, string multiplicity)
        {
            this.Name = name;
            this.Multiplicity = multiplicity;
        }

        public string Name { get; private set; }
        public string Multiplicity { get; private set; }
    }
}