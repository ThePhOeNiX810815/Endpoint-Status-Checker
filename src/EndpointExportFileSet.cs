using System.IO;

namespace EndpointChecker
{
    internal sealed class EndpointExportFileSet
    {
        private EndpointExportFileSet(
            string directory,
            string xlsxFileName,
            string jsonFileName,
            string xmlFileName,
            string htmlInfoFileName,
            string htmlHttpFileName,
            string htmlFtpFileName)
        {
            Directory = directory;
            XlsxFileName = xlsxFileName;
            JsonFileName = jsonFileName;
            XmlFileName = xmlFileName;
            HtmlInfoFileName = htmlInfoFileName;
            HtmlHttpFileName = htmlHttpFileName;
            HtmlFtpFileName = htmlFtpFileName;
        }

        public string Directory { get; }

        public string XlsxFileName { get; }

        public string JsonFileName { get; }

        public string XmlFileName { get; }

        public string HtmlInfoFileName { get; }

        public string HtmlHttpFileName { get; }

        public string HtmlFtpFileName { get; }

        public string XlsxPath => Path.Combine(Directory, XlsxFileName);

        public string JsonPath => Path.Combine(Directory, JsonFileName);

        public string XmlPath => Path.Combine(Directory, XmlFileName);

        public string HtmlInfoPath => Path.Combine(Directory, HtmlInfoFileName);

        public string HtmlHttpPath => Path.Combine(Directory, HtmlHttpFileName);

        public string HtmlFtpPath => Path.Combine(Directory, HtmlFtpFileName);

        public static EndpointExportFileSet Create(
            string directory,
            string xlsxFileName,
            string jsonFileName,
            string xmlFileName,
            string htmlInfoFileName,
            string htmlHttpFileName,
            string htmlFtpFileName)
        {
            return new EndpointExportFileSet(
                directory,
                xlsxFileName,
                jsonFileName,
                xmlFileName,
                htmlInfoFileName,
                htmlHttpFileName,
                htmlFtpFileName);
        }
    }
}
