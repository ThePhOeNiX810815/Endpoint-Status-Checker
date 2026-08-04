namespace EndpointChecker
{
    internal sealed class EndpointWhoIsPresentationInput
    {
        public bool HasLookupPayload { get; set; }

        public bool TabCurrentlyPresent { get; set; }

        public string RegistrableDomain { get; set; }

        public string WhoIsServer { get; set; }

        public string RawResponse { get; set; }
    }

    internal sealed class EndpointWhoIsPresentationModel
    {
        public bool ShouldApplyPayload { get; set; }

        public bool ShouldEnsureTabPresent { get; set; }

        public string RegistrableDomain { get; set; }

        public string WhoIsServer { get; set; }

        public string RawResponse { get; set; }
    }

    internal static class EndpointWhoIsPresentationBuilder
    {
        public static EndpointWhoIsPresentationModel Build(EndpointWhoIsPresentationInput input)
        {
            EndpointWhoIsPresentationModel model = new EndpointWhoIsPresentationModel();

            if (input == null || !input.HasLookupPayload)
            {
                return model;
            }

            model.ShouldApplyPayload = true;
            model.ShouldEnsureTabPresent = !input.TabCurrentlyPresent;
            model.RegistrableDomain = input.RegistrableDomain;
            model.WhoIsServer = input.WhoIsServer;
            model.RawResponse = input.RawResponse;

            return model;
        }
    }
}