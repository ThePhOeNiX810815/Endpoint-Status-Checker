using System.Collections.Generic;
using EndpointChecker;
using Xunit;

namespace EndpointCheckingCore.Tests
{
    public class ReportMailTableBuilderTests
    {
        [Fact]
        public void Build_UsesExpectedTableWrapper()
        {
            string html = ReportMailTableBuilder.Build(
                new[]
                {
                    new KeyValuePair<string, string>("Name", "Value")
                },
                "#ABCDEF",
                "#123456");

            Assert.StartsWith("<table border=\"3\" bordercolor=\"#ABCDEF\" bgcolor=\"#123456\" cellpadding=\"5\" cellspacing=\"10\">", html);
            Assert.EndsWith("</table>", html);
        }

        [Fact]
        public void Build_HtmlEncodesKeyAndValue()
        {
            string html = ReportMailTableBuilder.Build(
                new[]
                {
                    new KeyValuePair<string, string>("A < B", "X & Y > Z")
                },
                "#000000",
                "#FFFFFF");

            Assert.Contains("<td>A &lt; B</td><td>X &amp; Y &gt; Z</td>", html);
        }

        [Fact]
        public void Build_ProducesRowPerInputItem()
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
            Assert.Equal(3, rows);
        }
    }
}
