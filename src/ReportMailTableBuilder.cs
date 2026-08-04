using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace EndpointChecker
{
    internal static class ReportMailTableBuilder
    {
        public static string Build(
            IEnumerable<KeyValuePair<string, string>> rows,
            string borderColor,
            string backgroundColor)
        {
            if (rows == null)
            {
                throw new ArgumentNullException(nameof(rows));
            }

            StringBuilder mailMessageString = new StringBuilder();
            mailMessageString.AppendFormat(
                "<table border=\"3\" bordercolor=\"{0}\" bgcolor=\"{1}\" cellpadding=\"5\" cellspacing=\"10\">",
                borderColor,
                backgroundColor);

            foreach (KeyValuePair<string, string> row in rows)
            {
                mailMessageString.Append("<tr>");
                mailMessageString.AppendFormat(
                    "<td>{0}</td><td>{1}</td>",
                    WebUtility.HtmlEncode(row.Key),
                    WebUtility.HtmlEncode(row.Value));
                mailMessageString.Append("</tr>");
            }

            mailMessageString.Append("</table>");
            return mailMessageString.ToString();
        }
    }
}
