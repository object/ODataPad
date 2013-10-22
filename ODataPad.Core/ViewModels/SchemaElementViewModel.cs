using System.Collections.Generic;
using System.Linq;
using Cirrious.MvvmCross.ViewModels;
using ODataPad.Core.Models;

namespace ODataPad.Core.ViewModels
{
    public class SchemaElementViewModel : MvxViewModel
    {
        public SchemaElementViewModel(ResourceProperty property)
        {
            this.Name = property.Name;
            this.Summary = GetPropertySummary(property);
        }

        public SchemaElementViewModel(ResourceAssociation association)
        {
            this.Name = association.Name;
            this.Summary = GetAssociationSummary(association);
        }

        public string Name { get; private set; }
        public string Summary { get; private set; }

        private string GetPropertySummary(ResourceProperty property)
        {
            var summary = property.Type;
            var items = new List<string>();
            if (property.IsKey)
                items.Add("key");
            if (property.IsNullable)
                items.Add("null");
            if (items.Any())
                summary += " (" + string.Join(",", items) + ")";
            return summary;
        }

        private string GetAssociationSummary(ResourceAssociation association)
        {
            return association.Multiplicity;
        }
    }
}