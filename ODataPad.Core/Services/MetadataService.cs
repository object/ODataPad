using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
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

        public static async Task<string> LoadServiceMetadataAsync(ServiceInfo service)
        {
            var task = Task<string>.Factory.StartNew(() =>
            {
                try
                {
                    return GetSchemaAsString(service.Url);
                }
                catch (Exception exception)
                {
                    var element = new XElement("Error");
                    element.Add(new XElement("Message", exception.Message));
                    return element.ToString();
                }
            });
            return await task;
        }
    }
}