using System.Collections.Generic;
using System.Net;

namespace EndpointChecker
{
    internal static class EndpointPageLinkValidationWorkflowTests
    {
        public static void Register()
        {
            EndpointCheckingCoreTestRunner.Run("Page-link workflow resolves successful requests as VALID", PageLinkWorkflowResolvesSuccessfulRequestsAsValid);
            EndpointCheckingCoreTestRunner.Run("Page-link workflow preserves legacy invalid HTTP status set", PageLinkWorkflowPreservesLegacyInvalidHttpStatusSet);
            EndpointCheckingCoreTestRunner.Run("Page-link workflow marks non-excluded HTTP errors as VALID", PageLinkWorkflowMarksNonExcludedHttpErrorsAsValid);
            EndpointCheckingCoreTestRunner.Run("Page-link workflow computes invalid summary transition", PageLinkWorkflowComputesInvalidSummaryTransition);
        }

        private static void PageLinkWorkflowResolvesSuccessfulRequestsAsValid()
        {
            EndpointPageLinkValidationItemModel model = EndpointPageLinkValidationWorkflow.BuildItem(new EndpointPageLinkValidationItemInput
            {
                RequestSucceeded = true,
                ResponseStatusCode = null,
                LinkType = "a",
                LinkAddress = "http://example.com",
            });

            EndpointCheckingCoreTestRunner.AssertEqual("VALID", model.Status);
            EndpointCheckingCoreTestRunner.AssertEqual("a", model.LinkType);
            EndpointCheckingCoreTestRunner.AssertEqual("http://example.com", model.LinkAddress);
        }

        private static void PageLinkWorkflowPreservesLegacyInvalidHttpStatusSet()
        {
            HttpStatusCode[] invalidStatuses =
            {
                HttpStatusCode.NotFound,
                HttpStatusCode.BadGateway,
                HttpStatusCode.GatewayTimeout,
                HttpStatusCode.InternalServerError,
                HttpStatusCode.NotImplemented,
                HttpStatusCode.RequestTimeout,
                HttpStatusCode.Conflict,
                HttpStatusCode.Gone,
                HttpStatusCode.ServiceUnavailable,
            };

            foreach (HttpStatusCode status in invalidStatuses)
            {
                string resolved = EndpointPageLinkValidationWorkflow.ResolveStatus(false, status);
                EndpointCheckingCoreTestRunner.AssertEqual("INVALID", resolved);
            }
        }

        private static void PageLinkWorkflowMarksNonExcludedHttpErrorsAsValid()
        {
            string resolved = EndpointPageLinkValidationWorkflow.ResolveStatus(false, HttpStatusCode.Forbidden);
            EndpointCheckingCoreTestRunner.AssertEqual("VALID", resolved);
        }

        private static void PageLinkWorkflowComputesInvalidSummaryTransition()
        {
            bool hasInvalid = EndpointPageLinkValidationWorkflow.HasAnyInvalid(new List<EndpointPageLinkValidationItemModel>
            {
                new EndpointPageLinkValidationItemModel { Status = "VALID" },
                new EndpointPageLinkValidationItemModel { Status = "INVALID" },
            });

            bool noInvalid = EndpointPageLinkValidationWorkflow.HasAnyInvalid(new List<EndpointPageLinkValidationItemModel>
            {
                new EndpointPageLinkValidationItemModel { Status = "VALID" },
            });

            EndpointCheckingCoreTestRunner.AssertEqual(true, hasInvalid);
            EndpointCheckingCoreTestRunner.AssertEqual(false, noInvalid);
        }
    }
}
