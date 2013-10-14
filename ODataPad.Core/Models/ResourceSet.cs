using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ODataPad.Core.Models
{
    public class ResourceSet
    {
        public ResourceSet(string name, 
            IEnumerable<ResourceProperty> properties, 
            IEnumerable<ResourceAssociation> associations)
        {
            this.Name = name;
            this.Properties = new ObservableCollection<ResourceProperty>(properties);
            this.Associations = new ObservableCollection<ResourceAssociation>(associations);
        }

        public string Name { get; set; }
        public ObservableCollection<ResourceProperty> Properties { get; private set; }
        public ObservableCollection<ResourceAssociation> Associations { get; private set; }
    }
}