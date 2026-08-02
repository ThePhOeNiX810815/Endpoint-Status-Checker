using Newtonsoft.Json;
using System.Collections.Generic;
using System.Xml;

namespace EndpointChecker
{
    /// <summary>
    /// Creates structured endpoint export payloads while preserving legacy JSON and XML shape.
    /// </summary>
    internal static class EndpointStructuredExportGenerator
    {
        /// <summary>
        /// Serializes endpoints using the legacy indented JSON formatting.
        /// </summary>
        public static string CreateJson(IEnumerable<EndpointDefinition> endpoints)
        {
            return JsonConvert.SerializeObject(endpoints, Newtonsoft.Json.Formatting.Indented);
        }

        /// <summary>
        /// Creates the legacy XML export document from an endpoint JSON payload.
        /// </summary>
        /// <remarks>
        /// The Encoding+ replacement is retained because plus signs are not valid XML element names.
        /// </remarks>
        public static XmlDocument CreateXmlDocument(string json)
        {
            return JsonConvert.DeserializeXmlNode("{\"EndpointStatus\":" + json.Replace("Encoding+", "Encoding_") + "}", "EndpointStatus");
        }
    }
}
