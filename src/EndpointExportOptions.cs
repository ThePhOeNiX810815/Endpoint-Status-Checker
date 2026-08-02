namespace EndpointChecker
{
    internal sealed class EndpointExportOptions
    {
        private EndpointExportOptions(bool json, bool xml, bool xlsx, bool html)
        {
            Json = json;
            Xml = xml;
            Xlsx = xlsx;
            Html = html;
        }

        public bool Json { get; }

        public bool Xml { get; }

        public bool Xlsx { get; }

        public bool Html { get; }

        public bool StructuredText => Json || Xml;

        public static EndpointExportOptions Create(bool json, bool xml, bool xlsx, bool html)
        {
            return new EndpointExportOptions(json, xml, xlsx, html);
        }
    }
}
