using System;

namespace EndpointChecker
{
    internal sealed class EndpointGeoLocationPresentationInput
    {
        public bool IsLookupSuccessful { get; set; }

        public bool HasMapTile { get; set; }

        public bool HasCountryFlag { get; set; }

        public bool TabCurrentlyPresent { get; set; }

        public string StatusNotAvailable { get; set; }

        public string SelectedIpAddress { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }

        public string Isp { get; set; }

        public string IspAs { get; set; }

        public string RegionName { get; set; }

        public string RegionCode { get; set; }

        public string TimeZone { get; set; }

        public string ZipCode { get; set; }

        public string City { get; set; }

        public string Organization { get; set; }

        public string CountryName { get; set; }

        public string CountryCode { get; set; }
    }

    internal sealed class EndpointGeoLocationPresentationModel
    {
        public bool HasDisplayPayload { get; set; }

        public bool ShouldShowCountryFlag { get; set; }

        public bool ShouldEnsureTabPresent { get; set; }

        public bool ShouldEnsureTabRemoved { get; set; }

        public string IpAddress { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }

        public string Isp { get; set; }

        public string AsDescription { get; set; }

        public string RegionDescription { get; set; }

        public string TimeZone { get; set; }

        public string ZipCode { get; set; }

        public string City { get; set; }

        public string Organization { get; set; }

        public string CountryDescription { get; set; }
    }

    internal static class EndpointGeoLocationPresentationBuilder
    {
        public static EndpointGeoLocationPresentationModel Build(EndpointGeoLocationPresentationInput input)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }

            EndpointGeoLocationPresentationModel model = new EndpointGeoLocationPresentationModel();

            bool hasPayload = input.IsLookupSuccessful && input.HasMapTile;
            model.HasDisplayPayload = hasPayload;

            if (!hasPayload)
            {
                model.ShouldEnsureTabRemoved = input.TabCurrentlyPresent;
                return model;
            }

            model.ShouldShowCountryFlag = input.HasCountryFlag;
            model.ShouldEnsureTabPresent = true;

            model.IpAddress = input.SelectedIpAddress;
            model.Latitude = input.Latitude;
            model.Longitude = input.Longitude;
            model.Isp = NotAvailableIfNullOrEmpty(input.Isp, input.StatusNotAvailable);
            model.AsDescription = NotAvailableIfNullOrEmpty(
                (input.IspAs ?? string.Empty).Replace(input.Isp ?? string.Empty, string.Empty).TrimEnd(),
                input.StatusNotAvailable);
            model.RegionDescription = NotAvailableIfNullOrEmpty(
                (input.RegionName ?? string.Empty) + " (" + (input.RegionCode ?? string.Empty) + ")",
                input.StatusNotAvailable);
            model.TimeZone = NotAvailableIfNullOrEmpty(input.TimeZone, input.StatusNotAvailable);
            model.ZipCode = NotAvailableIfNullOrEmpty(input.ZipCode, input.StatusNotAvailable);
            model.City = NotAvailableIfNullOrEmpty(input.City, input.StatusNotAvailable);
            model.Organization = NotAvailableIfNullOrEmpty(input.Organization, input.StatusNotAvailable);
            model.CountryDescription = NotAvailableIfNullOrEmpty(
                (input.CountryName ?? string.Empty) + " (" + (input.CountryCode ?? string.Empty) + ")",
                input.StatusNotAvailable);

            return model;
        }

        private static string NotAvailableIfNullOrEmpty(string value, string statusNotAvailable)
        {
            return !string.IsNullOrEmpty(value)
                ? value
                : statusNotAvailable;
        }
    }
}
