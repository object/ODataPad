namespace ODataPad.Core.Models
{
    public class CollectionAssociation : CollectionElement
    {
        public CollectionAssociation(string name, string multiplicity)
        {
            this.Name = name;
            this.Multiplicity = multiplicity;
        }

        public string Name { get; private set; }
        public string Multiplicity { get; private set; }

        // Aliases for XAML templates
        public string Summary { get { return Multiplicity; } }
    }
}