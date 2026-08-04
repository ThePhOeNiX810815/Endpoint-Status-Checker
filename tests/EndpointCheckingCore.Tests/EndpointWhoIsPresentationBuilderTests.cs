namespace EndpointChecker
{
    internal static class EndpointWhoIsPresentationBuilderTests
    {
        public static void Register()
        {
            EndpointCheckingCoreTestRunner.Run("WhoIs builder applies payload and requests tab addition when tab is absent", WhoIsBuilderAppliesPayloadAndRequestsTabAdditionWhenTabIsAbsent);
            EndpointCheckingCoreTestRunner.Run("WhoIs builder applies payload without forcing tab add when tab already exists", WhoIsBuilderAppliesPayloadWithoutForcingTabAddWhenTabAlreadyExists);
            EndpointCheckingCoreTestRunner.Run("WhoIs builder returns empty transition when lookup payload is missing", WhoIsBuilderReturnsEmptyTransitionWhenLookupPayloadIsMissing);
        }

        private static void WhoIsBuilderAppliesPayloadAndRequestsTabAdditionWhenTabIsAbsent()
        {
            EndpointWhoIsPresentationModel model = EndpointWhoIsPresentationBuilder.Build(new EndpointWhoIsPresentationInput
            {
                HasLookupPayload = true,
                TabCurrentlyPresent = false,
                RegistrableDomain = "example.com",
                WhoIsServer = "whois.verisign-grs.com",
                RawResponse = "Domain Name: EXAMPLE.COM",
            });

            EndpointCheckingCoreTestRunner.AssertEqual(true, model.ShouldApplyPayload);
            EndpointCheckingCoreTestRunner.AssertEqual(true, model.ShouldEnsureTabPresent);
            EndpointCheckingCoreTestRunner.AssertEqual("example.com", model.RegistrableDomain);
            EndpointCheckingCoreTestRunner.AssertEqual("whois.verisign-grs.com", model.WhoIsServer);
            EndpointCheckingCoreTestRunner.AssertEqual("Domain Name: EXAMPLE.COM", model.RawResponse);
        }

        private static void WhoIsBuilderAppliesPayloadWithoutForcingTabAddWhenTabAlreadyExists()
        {
            EndpointWhoIsPresentationModel model = EndpointWhoIsPresentationBuilder.Build(new EndpointWhoIsPresentationInput
            {
                HasLookupPayload = true,
                TabCurrentlyPresent = true,
                RegistrableDomain = "example.net",
                WhoIsServer = "whois.verisign-grs.com",
                RawResponse = "Domain Name: EXAMPLE.NET",
            });

            EndpointCheckingCoreTestRunner.AssertEqual(true, model.ShouldApplyPayload);
            EndpointCheckingCoreTestRunner.AssertEqual(false, model.ShouldEnsureTabPresent);
        }

        private static void WhoIsBuilderReturnsEmptyTransitionWhenLookupPayloadIsMissing()
        {
            EndpointWhoIsPresentationModel model = EndpointWhoIsPresentationBuilder.Build(new EndpointWhoIsPresentationInput
            {
                HasLookupPayload = false,
                TabCurrentlyPresent = false,
                RegistrableDomain = "example.edu",
                WhoIsServer = "whois.educause.edu",
                RawResponse = string.Empty,
            });

            EndpointCheckingCoreTestRunner.AssertEqual(false, model.ShouldApplyPayload);
            EndpointCheckingCoreTestRunner.AssertEqual(false, model.ShouldEnsureTabPresent);
            EndpointCheckingCoreTestRunner.AssertEqual(null, model.RegistrableDomain);
            EndpointCheckingCoreTestRunner.AssertEqual(null, model.WhoIsServer);
            EndpointCheckingCoreTestRunner.AssertEqual(null, model.RawResponse);
        }
    }
}
