using System;

namespace EndpointChecker
{
    internal static class EndpointExportOptionsTests
    {
        private static int failed;

        private static void Main()
        {
            Run("Snapshot preserves selected export switches", SnapshotPreservesSelectedExportSwitches);
            Run("Structured text is enabled for JSON", StructuredTextIsEnabledForJson);
            Run("Structured text is enabled for XML", StructuredTextIsEnabledForXml);
            Run("Structured text is disabled without JSON or XML", StructuredTextIsDisabledWithoutJsonOrXml);

            if (failed > 0)
            {
                Environment.Exit(1);
            }
        }

        private static void SnapshotPreservesSelectedExportSwitches()
        {
            EndpointExportOptions options = EndpointExportOptions.Create(true, false, true, false);

            AssertEqual(true, options.Json);
            AssertEqual(false, options.Xml);
            AssertEqual(true, options.Xlsx);
            AssertEqual(false, options.Html);
        }

        private static void StructuredTextIsEnabledForJson()
        {
            EndpointExportOptions options = EndpointExportOptions.Create(true, false, false, false);

            AssertEqual(true, options.StructuredText);
        }

        private static void StructuredTextIsEnabledForXml()
        {
            EndpointExportOptions options = EndpointExportOptions.Create(false, true, false, false);

            AssertEqual(true, options.StructuredText);
        }

        private static void StructuredTextIsDisabledWithoutJsonOrXml()
        {
            EndpointExportOptions options = EndpointExportOptions.Create(false, false, true, true);

            AssertEqual(false, options.StructuredText);
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

        private static void AssertEqual<T>(T expected, T actual)
        {
            if (!object.Equals(expected, actual))
            {
                throw new InvalidOperationException("Expected <" + expected + "> but was <" + actual + ">.");
            }
        }
    }
}
