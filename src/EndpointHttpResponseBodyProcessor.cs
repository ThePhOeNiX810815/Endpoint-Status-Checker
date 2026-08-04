using System;
using System.IO;
using System.Text;

namespace EndpointChecker
{
    internal static class EndpointHttpResponseBodyProcessor
    {
        public static bool ShouldReadResponseBody(bool saveResponse, bool resolvePageMetaInfo)
        {
            return saveResponse || resolvePageMetaInfo;
        }

        public static byte[] ReadResponseBytes(Stream responseStream, long maxBytes, int chunkSize = 1024)
        {
            if (responseStream == null)
            {
                return Array.Empty<byte>();
            }

            using (BinaryReader responseReader = new BinaryReader(responseStream))
            using (MemoryStream memoryStream = new MemoryStream())
            {
                byte[] buffer = responseReader.ReadBytes(chunkSize);
                while (buffer.Length > 0 && memoryStream.Length < maxBytes)
                {
                    memoryStream.Write(buffer, 0, buffer.Length);
                    buffer = responseReader.ReadBytes(chunkSize);
                }

                return memoryStream.ToArray();
            }
        }

        public static bool ShouldResolveHtmlMetaInfo(bool resolvePageMetaInfo, string httpContentType)
        {
            return resolvePageMetaInfo && string.Equals(httpContentType, "text/html", StringComparison.Ordinal);
        }

        public static bool ShouldResolveMetaWithHtmlEncodingFallback(Encoding httpEncoding, Encoding htmlEncoding)
        {
            return httpEncoding == null && htmlEncoding != null;
        }

        public static bool ShouldAssignDefaultHtmlEncoding(Encoding httpEncoding, Encoding htmlEncoding)
        {
            return httpEncoding == null && htmlEncoding == null;
        }
    }
}
