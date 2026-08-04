using System.Collections.Generic;
using System.Net;

namespace EndpointChecker
{
    internal static class EndpointHttpHeaderCollector
    {
        public static List<Property> Collect(WebHeaderCollection headerCollection)
        {
            List<Property> headers = new List<Property>();

            if (headerCollection != null &&
                headerCollection.Count > 0)
            {
                foreach (string headerName in headerCollection.Keys)
                {
                    headers.Add(new Property
                    {
                        ItemName = headerName,
                        ItemValue = headerCollection[headerName],
                    });
                }
            }

            return headers;
        }
    }
}
