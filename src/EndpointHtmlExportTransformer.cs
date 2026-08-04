using System;
using System.IO;
using System.Xml;

namespace EndpointChecker
{
    internal static class EndpointHtmlExportTransformer
    {
        public static string AddAutoRefreshMetaTag(string inputHtml, int refreshIntervalSeconds)
        {
            return inputHtml.Replace("<head>", "<head>" + Environment.NewLine + "<meta http-equiv=\"refresh\" content=\"" + refreshIntervalSeconds + "\">");
        }

        public static string CreateEndpointUrlHyperLinks(string inputHtml)
        {
            // Preserve the current HTML-to-XML conversion behavior used by the legacy export path.
            string inputXmlString = inputHtml
                .Replace("&nbsp;", " ")
                .Replace("&", "&amp;");

            XmlDocument inputHtmlDoc = new XmlDocument();
            inputHtmlDoc.LoadXml(inputXmlString);

            XmlNodeList trNodesList = inputHtmlDoc.GetElementsByTagName("tr");

            int trNodeIndex = 0;
            foreach (XmlNode trNode in trNodesList)
            {
                if (trNodeIndex > 0)
                {
                    XmlAttribute attr = inputHtmlDoc.CreateAttribute("onclick");
                    attr.Value = "location.href = '" +
                                 trNode.ChildNodes[3].ChildNodes[0].InnerXml +
                                 "'";

                    trNode.ChildNodes[3].ChildNodes[0].Attributes.Append(attr);

                    using (StringWriter stringWriter = new StringWriter())
                    using (XmlWriter xmlTextWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings { Indent = true, NewLineOnAttributes = false, OmitXmlDeclaration = true }))
                    {
                        inputHtmlDoc.WriteTo(xmlTextWriter);
                        xmlTextWriter.Flush();
                        inputHtml = stringWriter.GetStringBuilder().ToString();
                    }
                }

                trNodeIndex++;
            }

            foreach (string htmlLine in inputXmlString.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (htmlLine.StartsWith(".X") &&
                    htmlLine.Contains("text-decoration:underline"))
                {
                    inputHtml = inputHtml.Replace(
                        htmlLine.Split('{')[1],
                        "cursor:pointer;" + htmlLine.Split('{')[1]);
                }
            }

            return inputHtml;
        }

        public static string AddRefreshCssButton(string inputHtml)
        {
            return inputHtml.Replace(
                                    @"<html xmlns=""http://www.w3.org/1999/xhtml"">
  <head>
    <style type=""text/css"">table",
                                    @"<html xmlns=""http://www.w3.org/1999/xhtml"">
<INPUT TYPE=""button"" onClick=""window.location.reload()"" VALUE=""Refresh"" ID=""refreshBTN"">
  <head>
    <style type=""text/css"">
    body {
            background - color: #CCC;
            margin: 32px 0px 0px 0px;
                                }
                                INPUT#refreshBTN {
                                position: fixed;
                                top: 0px;
                                left: 0px;
                                width: 100%;
                                color: #7CFC00;
                                background: #333;
                                padding: 5px;
                                cursor:pointer;
                                }
                            table");
        }
    }
}