using System;
using System.Collections.Generic;
using System.Xml;

namespace EndpointChecker
{
    internal static class EndpointStructuredExportGeneratorTests
    {
        private static int failed;

        private static void Main()
        {
            Run("JSON export remains indented", JsonExportRemainsIndented);
            Run("XML export preserves root shape", XmlExportPreservesRootShape);
            Run("XML export preserves Encoding plus workaround", XmlExportPreservesEncodingPlusWorkaround);

            if (failed > 0)
            {
                Environment.Exit(1);
            }
        }

        private static void JsonExportRemainsIndented()
        {
            string json = EndpointStructuredExportGenerator.CreateJson(new[]
            {
                new EndpointDefinition { Name = "Endpoint A", Address = "https://example.test" }
            });

            AssertContains(Environment.NewLine, json);
            AssertContains("  \"Name\": \"Endpoint A\"", json);
            AssertContains("  \"Address\": \"https://example.test\"", json);
        }

        private static void XmlExportPreservesRootShape()
        {
            string json = EndpointStructuredExportGenerator.CreateJson(new[]
            {
                new EndpointDefinition { Name = "Endpoint A", Address = "https://example.test" }
            });

            XmlDocument document = EndpointStructuredExportGenerator.CreateXmlDocument(json);

            AssertEqual("EndpointStatus", document.DocumentElement.Name);
            AssertEqual("Endpoint A", document.DocumentElement.SelectSingleNode("EndpointStatus/Name").InnerText);
            AssertEqual("https://example.test", document.DocumentElement.SelectSingleNode("EndpointStatus/Address").InnerText);
        }

        private static void XmlExportPreservesEncodingPlusWorkaround()
        {
            string json = "[{\"Encoding+Name\":\"utf-8\"}]";

            XmlDocument document = EndpointStructuredExportGenerator.CreateXmlDocument(json);

            AssertEqual("utf-8", document.DocumentElement.SelectSingleNode("EndpointStatus/Encoding_Name").InnerText);
        }

        private static void Run(string name, Action test)
        {
            try
            {
                test();
                Console.WriteLine("PASS " + name);
            }
            catch (Exception exception)
            {
                failed++;
                Console.WriteLine("FAIL " + name + ": " + exception.Message);
            }
        }

        private static void AssertContains(string expected, string actual)
        {
            if (actual == null || !actual.Contains(expected))
            {
                throw new InvalidOperationException("Expected text to contain <" + expected + "> but was <" + actual + ">.");
            }
        }

        private static void AssertEqual<T>(T expected, T actual)
        {
            if (!object.Equals(expected, actual))
            {
                throw new InvalidOperationException("Expected <" + expected + "> but was <" + actual + ">.");
            }
        }
    }

    public class EndpointDefinition
    {
        public string Name { get; set; }

        public string Address { get; set; }
    }
}
