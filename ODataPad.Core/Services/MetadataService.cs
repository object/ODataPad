using System.Collections.Generic;
using System.Linq;
using Simple.OData.Client;
using ODataPad.Core.Models;

namespace ODataPad.Core.Services
{
    public class MetadataService
    {
        public static string GetSchemaAsString(string serviceUrl)
        {
            return ODataClient.GetSchemaAsString(serviceUrl);
        }

        public static IEnumerable<ServiceCollection> ParseServiceMetadata(string schemaString)
        {
            var collections = new List<ServiceCollection>();
            var schema = ODataClient.ParseSchemaString(schemaString);

            foreach (var table in schema.Tables)
            {
                var properties = table.Columns.Select(x =>
                    new CollectionProperty(
                        x.ActualName,
                        x.PropertyType.Name.Split('.').Last(),
                        table.GetKeyNames().Contains(x.ActualName),
                        x.IsNullable)).ToList();

                var associations = table.Associations.Select(x =>
                    new CollectionAssociation(
                        x.ActualName,
                        x.Multiplicity)).ToList();

                collections.Add(new ServiceCollection(table.ActualName, properties, associations));
            }
            return collections;
        }
    }
}