using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ODataPad.Core.Models
{
    public class ServiceCollection
    {
        public ServiceCollection(string name, 
            IEnumerable<CollectionProperty> properties, 
            IEnumerable<CollectionAssociation> associations)
        {
            this.Name = name;
            this.Properties = new ObservableCollection<CollectionProperty>(properties);
            this.Associations = new ObservableCollection<CollectionAssociation>(associations);
        }

        public string Name { get; set; }
        public ObservableCollection<CollectionProperty> Properties { get; private set; }
        public ObservableCollection<CollectionAssociation> Associations { get; private set; }
    }
}