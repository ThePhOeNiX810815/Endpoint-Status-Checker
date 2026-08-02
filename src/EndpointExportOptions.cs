namespace EndpointChecker
{
    /// <summary>
    /// Captures selected endpoint-status export formats before export generation begins.
    /// </summary>
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

        /// <summary>
        /// Creates a snapshot of the selected export formats.
        /// </summary>
        public static EndpointExportOptions Create(bool json, bool xml, bool xlsx, bool html)
        {
            return new EndpointExportOptions(json, xml, xlsx, html);
        }
    }
}
