using System.Collections.Generic;
using System.Net;

namespace EndpointChecker
{
    internal sealed class EndpointPageLinkValidationItemInput
    {
        public bool RequestSucceeded { get; set; }

        public HttpStatusCode? ResponseStatusCode { get; set; }

        public string LinkType { get; set; }

        public string LinkAddress { get; set; }
    }

    internal sealed class EndpointPageLinkValidationItemModel
    {
        public string Status { get; set; }

        public string LinkType { get; set; }

        public string LinkAddress { get; set; }
    }

    internal static class EndpointPageLinkValidationWorkflow
    {
        public static EndpointPageLinkValidationItemModel BuildItem(EndpointPageLinkValidationItemInput input)
        {
            EndpointPageLinkValidationItemModel model = new EndpointPageLinkValidationItemModel();

            if (input == null)
            {
                model.Status = "INVALID";
                return model;
            }

            model.Status = ResolveStatus(input.RequestSucceeded, input.ResponseStatusCode);
            model.LinkType = input.LinkType;
            model.LinkAddress = input.LinkAddress;

            return model;
        }

        public static bool HasAnyInvalid(IReadOnlyList<EndpointPageLinkValidationItemModel> items)
        {
            if (items == null)
            {
                return false;
            }

            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] != null && items[i].Status == "INVALID")
                {
                    return true;
                }
            }

            return false;
        }

        public static string ResolveStatus(bool requestSucceeded, HttpStatusCode? responseStatusCode)
        {
            if (requestSucceeded)
            {
                return "VALID";
            }

            if (!responseStatusCode.HasValue)
            {
                return "INVALID";
            }

            switch (responseStatusCode.Value)
            {
                case HttpStatusCode.NotFound:
                case HttpStatusCode.BadGateway:
                case HttpStatusCode.GatewayTimeout:
                case HttpStatusCode.InternalServerError:
                case HttpStatusCode.NotImplemented:
                case HttpStatusCode.RequestTimeout:
                case HttpStatusCode.Conflict:
                case HttpStatusCode.Gone:
                case HttpStatusCode.ServiceUnavailable:
                    return "INVALID";

                default:
                    return "VALID";
            }
        }
    }
}
