using System;
using System.Collections.Generic;

namespace EndpointChecker
{
    internal static class ReportMailTableBuilderTests
    {
        public static void Register()
        {
            EndpointCheckingCoreTestRunner.Run("Report mail table builder preserves wrapper template", BuildUsesExpectedTableWrapper);
            EndpointCheckingCoreTestRunner.Run("Report mail table builder HTML-encodes keys and values", BuildHtmlEncodesKeyAndValue);
            EndpointCheckingCoreTestRunner.Run("Report mail table builder outputs one row per item", BuildProducesRowPerInputItem);
        }

        private static void BuildUsesExpectedTableWrapper()
        {
            string html = ReportMailTableBuilder.Build(
                new[]
                {
                    new KeyValuePair<string, string>("Name", "Value")
                },
                "#ABCDEF",
                "#123456");

            if (!html.StartsWith("<table border=\"3\" bordercolor=\"#ABCDEF\" bgcolor=\"#123456\" cellpadding=\"5\" cellspacing=\"10\">"))
            {
                throw new InvalidOperationException("Unexpected table wrapper start.");
            }

            if (!html.EndsWith("</table>"))
            {
                throw new InvalidOperationException("Unexpected table wrapper end.");
            }
        }

        private static void BuildHtmlEncodesKeyAndValue()
        {
            string html = ReportMailTableBuilder.Build(
                new[]
                {
                    new KeyValuePair<string, string>("A < B", "X & Y > Z")
                },
                "#000000",
                "#FFFFFF");

            if (!html.Contains("<td>A &lt; B</td><td>X &amp; Y &gt; Z</td>"))
            {
                throw new InvalidOperationException("Expected encoded key/value pair is missing.");
            }
        }

        private static void BuildProducesRowPerInputItem()
        {
            string html = ReportMailTableBuilder.Build(
                new[]
                {
                    new KeyValuePair<string, string>("One", "1"),
                    new KeyValuePair<string, string>("Two", "2"),
                    new KeyValuePair<string, string>("Three", "3")
                },
                "#000000",
                "#FFFFFF");

            int rows = html.Split(new[] { "<tr>" }, System.StringSplitOptions.None).Length - 1;
            EndpointCheckingCoreTestRunner.AssertEqual(3, rows);
        }
    }
}
