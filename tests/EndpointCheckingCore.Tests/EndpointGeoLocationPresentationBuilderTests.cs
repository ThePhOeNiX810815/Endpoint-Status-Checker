using System;

namespace EndpointChecker
{
    internal static class EndpointGeoLocationPresentationBuilderTests
    {
        public static void Register()
        {
            EndpointCheckingCoreTestRunner.Run("Geo builder returns display payload and show-tab transition for successful lookup", GeoBuilderReturnsDisplayPayloadAndShowTabTransitionForSuccessfulLookup);
            EndpointCheckingCoreTestRunner.Run("Geo builder hides country flag when no flag image exists", GeoBuilderHidesCountryFlagWhenNoFlagImageExists);
            EndpointCheckingCoreTestRunner.Run("Geo builder requests tab removal when payload is missing and tab exists", GeoBuilderRequestsTabRemovalWhenPayloadMissingAndTabExists);
            EndpointCheckingCoreTestRunner.Run("Geo builder keeps tab untouched when payload is missing and tab is absent", GeoBuilderKeepsTabUntouchedWhenPayloadMissingAndTabIsAbsent);
            EndpointCheckingCoreTestRunner.Run("Geo builder preserves legacy AS and region formatting", GeoBuilderPreservesLegacyAsAndRegionFormatting);
        }

        private static void GeoBuilderReturnsDisplayPayloadAndShowTabTransitionForSuccessfulLookup()
        {
            EndpointGeoLocationPresentationModel model = EndpointGeoLocationPresentationBuilder.Build(new EndpointGeoLocationPresentationInput
            {
                IsLookupSuccessful = true,
                HasMapTile = true,
                HasCountryFlag = true,
                TabCurrentlyPresent = false,
                StatusNotAvailable = "N/A",
                SelectedIpAddress = "203.0.113.15",
                Latitude = "48.123",
                Longitude = "16.456",
                Isp = "ISP Ltd",
                IspAs = "ISP Ltd AS123",
                RegionName = "Vienna",
                RegionCode = "9",
                TimeZone = "Europe/Vienna",
                ZipCode = "1010",
                City = "Vienna",
                Organization = "Org",
                CountryName = "Austria",
                CountryCode = "AT",
            });

            EndpointCheckingCoreTestRunner.AssertEqual(true, model.HasDisplayPayload);
            EndpointCheckingCoreTestRunner.AssertEqual(true, model.ShouldEnsureTabPresent);
            EndpointCheckingCoreTestRunner.AssertEqual(false, model.ShouldEnsureTabRemoved);
            EndpointCheckingCoreTestRunner.AssertEqual(true, model.ShouldShowCountryFlag);
            EndpointCheckingCoreTestRunner.AssertEqual("203.0.113.15", model.IpAddress);
        }

        private static void GeoBuilderHidesCountryFlagWhenNoFlagImageExists()
        {
            EndpointGeoLocationPresentationModel model = EndpointGeoLocationPresentationBuilder.Build(new EndpointGeoLocationPresentationInput
            {
                IsLookupSuccessful = true,
                HasMapTile = true,
                HasCountryFlag = false,
                TabCurrentlyPresent = false,
                StatusNotAvailable = "N/A",
                Isp = "ISP",
                IspAs = "ISP AS42",
                RegionName = "X",
                RegionCode = "Y",
                CountryName = "Country",
                CountryCode = "CC",
            });

            EndpointCheckingCoreTestRunner.AssertEqual(true, model.HasDisplayPayload);
            EndpointCheckingCoreTestRunner.AssertEqual(false, model.ShouldShowCountryFlag);
        }

        private static void GeoBuilderRequestsTabRemovalWhenPayloadMissingAndTabExists()
        {
            EndpointGeoLocationPresentationModel model = EndpointGeoLocationPresentationBuilder.Build(new EndpointGeoLocationPresentationInput
            {
                IsLookupSuccessful = false,
                HasMapTile = false,
                HasCountryFlag = false,
                TabCurrentlyPresent = true,
                StatusNotAvailable = "N/A",
            });

            EndpointCheckingCoreTestRunner.AssertEqual(false, model.HasDisplayPayload);
            EndpointCheckingCoreTestRunner.AssertEqual(false, model.ShouldEnsureTabPresent);
            EndpointCheckingCoreTestRunner.AssertEqual(true, model.ShouldEnsureTabRemoved);
        }

        private static void GeoBuilderKeepsTabUntouchedWhenPayloadMissingAndTabIsAbsent()
        {
            EndpointGeoLocationPresentationModel model = EndpointGeoLocationPresentationBuilder.Build(new EndpointGeoLocationPresentationInput
            {
                IsLookupSuccessful = false,
                HasMapTile = false,
                HasCountryFlag = false,
                TabCurrentlyPresent = false,
                StatusNotAvailable = "N/A",
            });

            EndpointCheckingCoreTestRunner.AssertEqual(false, model.HasDisplayPayload);
            EndpointCheckingCoreTestRunner.AssertEqual(false, model.ShouldEnsureTabPresent);
            EndpointCheckingCoreTestRunner.AssertEqual(false, model.ShouldEnsureTabRemoved);
        }

        private static void GeoBuilderPreservesLegacyAsAndRegionFormatting()
        {
            EndpointGeoLocationPresentationModel model = EndpointGeoLocationPresentationBuilder.Build(new EndpointGeoLocationPresentationInput
            {
                IsLookupSuccessful = true,
                HasMapTile = true,
                HasCountryFlag = true,
                TabCurrentlyPresent = true,
                StatusNotAvailable = "N/A",
                Isp = "MegaNet",
                IspAs = "MegaNet AS9000",
                RegionName = "North",
                RegionCode = "N1",
                CountryName = "Freedonia",
                CountryCode = "FD",
            });

            EndpointCheckingCoreTestRunner.AssertEqual(" AS9000", model.AsDescription);
            EndpointCheckingCoreTestRunner.AssertEqual("North (N1)", model.RegionDescription);
            EndpointCheckingCoreTestRunner.AssertEqual("Freedonia (FD)", model.CountryDescription);
        }
    }
}
