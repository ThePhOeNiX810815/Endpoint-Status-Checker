using System;
using System.Text.RegularExpressions;

namespace EndpointChecker
{
    internal sealed class EndpointMacVendorPresentationInput
    {
        public string StatusNotAvailable { get; set; }

        public string Company { get; set; }

        public string Address { get; set; }

        public string Domain { get; set; }

        public bool HasVendorImage { get; set; }
    }

    internal sealed class EndpointMacVendorPresentationModel
    {
        public string VendorText { get; set; }

        public bool ShouldApplyVendorImage { get; set; }

        public bool ShouldEnableVendorLink { get; set; }

        public string VendorWebPage { get; set; }

        public string VendorTooltipText { get; set; }
    }

    internal static class EndpointMacVendorPresentationBuilder
    {
        public static EndpointMacVendorPresentationModel Build(EndpointMacVendorPresentationInput input)
        {
            EndpointMacVendorPresentationModel model = new EndpointMacVendorPresentationModel();

            if (input == null)
            {
                return model;
            }

            string vendorText = input.StatusNotAvailable;

            if (!string.IsNullOrEmpty(input.Company))
            {
                vendorText = input.Company;

                if (!string.IsNullOrEmpty(input.Address))
                {
                    Regex regex = new Regex("[ ]{2,}", RegexOptions.None);
                    vendorText += " (" + regex.Replace(input.Address, " ") + ")";
                }
            }

            model.VendorText = vendorText;
            model.ShouldApplyVendorImage = input.HasVendorImage;
            model.ShouldEnableVendorLink = input.HasVendorImage && !string.IsNullOrEmpty(input.Domain);

            if (model.ShouldEnableVendorLink)
            {
                model.VendorWebPage = "http://" + input.Domain;
                model.VendorTooltipText = "Click to open \"" +
                    input.Company +
                    "\" web page (" +
                    input.Domain +
                    ")";
            }

            return model;
        }
    }
}
