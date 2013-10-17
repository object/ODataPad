﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using Cirrious.MvvmCross.ViewModels;
using ODataPad.Core.Models;

namespace ODataPad.Core.ViewModels
{
    public class SchemaViewModel : MvxViewModel
    {
        private readonly List<SchemaElementViewModel> _schemaElements;

        public SchemaViewModel(ResourceSetDetailsViewModel parent)
        {
            this.Home = parent.Home;

            _schemaElements = PopulateSchemaElements(parent.Properties, parent.Associations);
        }

        public HomeViewModelBase Home { get; set; }
        public List<SchemaElementViewModel> Elements { get { return _schemaElements; } }

        private List<SchemaElementViewModel> PopulateSchemaElements(
            IEnumerable<ResourceProperty> properties,
            IEnumerable<ResourceAssociation> associations)
        {
            var elements = new List<SchemaElementViewModel>();
            foreach (var property in properties)
            {
                elements.Add(new SchemaElementViewModel(property));
            }
            foreach (var association in associations)
            {
                elements.Add(new SchemaElementViewModel(association));
            }
            return elements;
        }
    }
}