using Newtonsoft.Json;
using System.Collections.Generic;
using System.Xml;

namespace EndpointChecker
{
    internal static class EndpointStructuredExportGenerator
    {
        public static string CreateJson(IEnumerable<EndpointDefinition> endpoints)
        {
            return JsonConvert.SerializeObject(endpoints, Newtonsoft.Json.Formatting.Indented);
        }

        public static XmlDocument CreateXmlDocument(string json)
        {
            return JsonConvert.DeserializeXmlNode("{\"EndpointStatus\":" + json.Replace("Encoding+", "Encoding_") + "}", "EndpointStatus");
        }
    }
}
