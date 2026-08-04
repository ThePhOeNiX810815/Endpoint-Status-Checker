using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;

namespace EndpointChecker
{
    internal sealed class EndpointHttpHtmlMetadataResolveInput
    {
        public string HtmlResponseDocumentString { get; set; }

        public Uri ResponseUri { get; set; }

        public bool ResolvePageLinks { get; set; }

        public string StatusNotAvailable { get; set; }

        public string CurrentHtmlContentLanguage { get; set; }

        public Encoding CurrentHtmlEncoding { get; set; }

        public Func<string, Encoding> ParseEncoding { get; set; }
    }

    internal sealed class EndpointHttpHtmlMetadataResolveOutput
    {
        public PropertyItems HtmlMetaInfo { get; set; }

        public PropertyItems HtmlPageLinks { get; set; }

        public string HtmlTitle { get; set; }

        public string HtmlDescription { get; set; }

        public string HtmlAuthor { get; set; }

        public string HtmlContentLanguage { get; set; }

        public Color HtmlThemeColor { get; set; }

        public bool HasHtmlThemeColor { get; set; }

        public Encoding HtmlEncoding { get; set; }

        public Encoding HtmlDefaultStreamEncoding { get; set; }
    }

    internal static class EndpointHttpHtmlMetadataResolver
    {
        public static EndpointHttpHtmlMetadataResolveOutput Resolve(EndpointHttpHtmlMetadataResolveInput input)
        {
            string htmlResponseDocumentString = (input.HtmlResponseDocumentString ?? string.Empty)
                .Replace(Environment.NewLine, string.Empty)
                .Replace('\'', '"')
                .Replace("&nbsp", " ");

            string statusNotAvailable = input.StatusNotAvailable;
            string htmlTitle = statusNotAvailable;
            string htmlDescription = statusNotAvailable;
            string htmlAuthor = statusNotAvailable;
            string htmlContentLanguage = input.CurrentHtmlContentLanguage;
            Encoding htmlEncoding = input.CurrentHtmlEncoding;

            PropertyItems htmlMetaInfo = new PropertyItems { PropertyItem = new List<Property>() };
            PropertyItems htmlPageLinks = null;

            HtmlDocument htmlResponseDocument = new HtmlDocument();
            htmlResponseDocument.LoadHtml(htmlResponseDocumentString);
            htmlResponseDocument.OptionFixNestedTags = true;

            if (input.ResolvePageLinks)
            {
                htmlPageLinks = GetDocumentLinks(input.ResponseUri, htmlResponseDocument, new[] { "href", "src" });
            }

            HtmlNode[] htmlRootNodeList = htmlResponseDocument.DocumentNode.Descendants()
                .Where(node => node.Name.ToLower() == "html")
                .ToArray();

            foreach (HtmlNode htmlRootNode in htmlRootNodeList)
            {
                if (htmlContentLanguage == statusNotAvailable)
                {
                    foreach (HtmlAttribute rootNodeAttribute in htmlRootNode.Attributes)
                    {
                        if (rootNodeAttribute.Name.ToLower().Contains("lang") &&
                            !string.IsNullOrEmpty(rootNodeAttribute.Value))
                        {
                            htmlContentLanguage = GetContentLanguage(rootNodeAttribute.Value, statusNotAvailable);
                        }
                    }
                }

                HtmlNode[] htmlHeadNodeList = htmlRootNode.Descendants()
                    .Where(node => node.Name.ToLower() == "head")
                    .ToArray();

                foreach (HtmlNode htmlHeadNode in htmlHeadNodeList)
                {
                    if (htmlTitle == statusNotAvailable)
                    {
                        foreach (HtmlNode htmlNodeChild in htmlHeadNode.ChildNodes)
                        {
                            if (htmlNodeChild.OriginalName.ToLower() == "title" &&
                                !string.IsNullOrEmpty(htmlNodeChild.InnerText.TrimStart().TrimEnd()))
                            {
                                htmlTitle = htmlNodeChild.InnerText.TrimStart().TrimEnd();
                                break;
                            }
                        }
                    }

                    foreach (HtmlNode htmlNodeChild in htmlHeadNode.ChildNodes)
                    {
                        if (htmlNodeChild.OriginalName.ToLower() == "meta")
                        {
                            ProcessMetaTag(htmlNodeChild, htmlMetaInfo, statusNotAvailable, ref htmlTitle, ref htmlEncoding, input.ParseEncoding);
                        }
                    }
                }
            }

            if (htmlContentLanguage == statusNotAvailable)
            {
                string contentLanguage = GetMetaInfoValueByKey(htmlMetaInfo, "content-language", statusNotAvailable);
                if (contentLanguage != statusNotAvailable)
                {
                    htmlContentLanguage = GetContentLanguage(contentLanguage, statusNotAvailable);
                }
            }

            htmlAuthor = GetMetaInfoValueByKey(htmlMetaInfo, "author", statusNotAvailable);
            if (htmlAuthor == statusNotAvailable)
            {
                htmlAuthor = GetMetaInfoValueByKey(htmlMetaInfo, "autor", statusNotAvailable);
            }

            if (htmlAuthor == statusNotAvailable)
            {
                htmlAuthor = GetMetaInfoValueByKey(htmlMetaInfo, "web_author", statusNotAvailable);
            }

            htmlDescription = GetMetaInfoValueByKey(htmlMetaInfo, "description", statusNotAvailable);

            Color htmlThemeColor = Color.Empty;
            bool hasThemeColor = false;
            string themeColorCode = GetMetaInfoValueByKey(htmlMetaInfo, "theme-color", statusNotAvailable);
            if (themeColorCode != statusNotAvailable)
            {
                try
                {
                    htmlThemeColor = ColorTranslator.FromHtml(themeColorCode);
                    hasThemeColor = true;
                }
                catch
                {
                }
            }

            return new EndpointHttpHtmlMetadataResolveOutput
            {
                HtmlMetaInfo = htmlMetaInfo,
                HtmlPageLinks = htmlPageLinks,
                HtmlTitle = htmlTitle,
                HtmlDescription = htmlDescription,
                HtmlAuthor = htmlAuthor,
                HtmlContentLanguage = htmlContentLanguage,
                HtmlThemeColor = htmlThemeColor,
                HasHtmlThemeColor = hasThemeColor,
                HtmlEncoding = htmlEncoding,
                HtmlDefaultStreamEncoding = htmlResponseDocument.StreamEncoding,
            };
        }

        private static PropertyItems GetDocumentLinks(Uri responseUri, HtmlDocument htmlResponseDocument, string[] elements)
        {
            PropertyItems linksList = new PropertyItems { PropertyItem = new List<Property>() };

            foreach (string element in elements)
            {
                HtmlNodeCollection elementNodeList = htmlResponseDocument.DocumentNode.SelectNodes("//*/@" + element);
                if (elementNodeList == null)
                {
                    continue;
                }

                foreach (HtmlNode linkNode in elementNodeList)
                {
                    foreach (HtmlAttribute linkNodeAttribute in linkNode.Attributes)
                    {
                        if (linkNodeAttribute.Name.ToLower() == element &&
                            !string.IsNullOrEmpty(linkNodeAttribute.Value) &&
                            (linkNodeAttribute.Value.ToLower().StartsWith(Uri.UriSchemeHttp.ToLower()) ||
                             linkNodeAttribute.Value.ToLower().StartsWith(Uri.UriSchemeHttps.ToLower()) ||
                             linkNodeAttribute.Value.ToLower().StartsWith(Uri.UriSchemeFtp.ToLower())))
                        {
                            if (linksList.PropertyItem.Where(item => item.ItemValue.ToLower().TrimEnd('/') == linkNodeAttribute.Value.ToLower().TrimEnd('/')).Count() == 0 &&
                                linksList.PropertyItem.Where(item => item.ItemValue.ToLower() == linkNodeAttribute.Value.ToLower()).Count() == 0 &&
                                responseUri.AbsoluteUri.ToLower().TrimEnd('/') != linkNodeAttribute.Value.ToLower().TrimEnd('/'))
                            {
                                linksList.PropertyItem.Add(new Property
                                {
                                    ItemName = linkNodeAttribute.Name,
                                    ItemValue = linkNodeAttribute.Value.TrimEnd('/')
                                });
                            }
                        }
                    }
                }
            }

            return linksList;
        }

        private static string GetContentLanguage(string contentLanguage, string statusNotAvailable)
        {
            if (contentLanguage.ToLower() == "mul")
            {
                return "Multi-Language (mul)";
            }

            try
            {
                return new CultureInfo(contentLanguage).NativeName;
            }
            catch
            {
                return statusNotAvailable;
            }
        }

        private static void ProcessMetaTag(
            HtmlNode subNode,
            PropertyItems htmlMetaInfo,
            string statusNotAvailable,
            ref string htmlTitle,
            ref Encoding htmlEncoding,
            Func<string, Encoding> parseEncoding)
        {
            string metaName = string.Empty;
            string metaValue = string.Empty;

            foreach (HtmlAttribute subNodeAttribute in subNode.Attributes)
            {
                if (subNodeAttribute.OriginalName.ToLower() == "charset")
                {
                    ApplyEncodingFromMetaTag("charset=" + subNodeAttribute.Value, parseEncoding, ref htmlEncoding);
                }
                else if (subNodeAttribute.OriginalName.ToLower() == "http-equiv" ||
                         subNodeAttribute.OriginalName.ToLower() == "name" ||
                         subNodeAttribute.OriginalName.ToLower() == "property")
                {
                    metaName = subNodeAttribute.Value.TrimStart().TrimEnd();
                }
                else if (subNodeAttribute.OriginalName.ToLower() == "content")
                {
                    metaValue = subNodeAttribute.Value.Replace("<br>", string.Empty).Replace("\n", " ").TrimStart().TrimEnd();
                }
            }

            if (string.IsNullOrEmpty(metaName))
            {
                return;
            }

            htmlMetaInfo.PropertyItem.Add(new Property { ItemName = metaName, ItemValue = metaValue });

            if (htmlTitle == statusNotAvailable &&
                (metaName.ToLower() == "title" ||
                 (metaName.ToLower().Split(':').Length > 1 &&
                  metaName.ToLower().Split(':')[1] == "title")) &&
                !string.IsNullOrEmpty(metaValue))
            {
                htmlTitle = metaValue;
            }

            if (metaName.ToLower() == "content-type")
            {
                ApplyEncodingFromMetaTag(metaValue, parseEncoding, ref htmlEncoding);
            }
        }

        private static void ApplyEncodingFromMetaTag(string encodingValue, Func<string, Encoding> parseEncoding, ref Encoding htmlEncoding)
        {
            if (htmlEncoding != null || parseEncoding == null)
            {
                return;
            }

            Encoding parsedEncoding = parseEncoding(encodingValue);
            if (parsedEncoding != null)
            {
                htmlEncoding = parsedEncoding;
            }
        }

        private static string GetMetaInfoValueByKey(PropertyItems htmlMetaInfo, string key, string statusNotAvailable)
        {
            string metaValue = string.Empty;

            if (htmlMetaInfo.PropertyItem != null)
            {
                if (htmlMetaInfo.PropertyItem.Where(metaInfo => metaInfo.ItemName.ToLower() == key.ToLower()).Count() > 0)
                {
                    metaValue = htmlMetaInfo.PropertyItem.Where(metaInfo => metaInfo.ItemName.ToLower() == key.ToLower())
                        .FirstOrDefault().ItemValue.TrimStart().TrimEnd();
                }

                if (string.IsNullOrEmpty(metaValue.TrimStart().TrimEnd()))
                {
                    if (htmlMetaInfo.PropertyItem.Where(metaInfo =>
                        metaInfo.ItemName.ToLower().Split(':').Length > 1 &&
                        metaInfo.ItemName.ToLower().Split(':')[1] == key.ToLower()).Count() > 0)
                    {
                        foreach (Property metaInfo in htmlMetaInfo.PropertyItem)
                        {
                            if (metaInfo.ItemName.Split(':').Length > 1 &&
                                metaInfo.ItemName.Split(':')[1].ToLower() == key.ToLower())
                            {
                                metaValue = metaInfo.ItemValue.TrimStart().TrimEnd();
                            }
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(metaValue))
            {
                metaValue = statusNotAvailable;
            }

            return metaValue;
        }
    }
}
