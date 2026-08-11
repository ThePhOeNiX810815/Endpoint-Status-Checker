using System;
using System.Collections.Generic;
using System.Linq;
using EndpointChecker.Properties;

namespace EndpointChecker
{
    /// <summary>
    /// Persists endpoint names manually flagged as local/internal addresses, for cases where
    /// <see cref="EndpointLocalAddressClassifier"/> cannot tell automatically whether VirusTotal
    /// scanning is applicable (e.g. an unresolved hostname that still looks public).
    /// </summary>
    internal static class EndpointVirusTotalLocalFlagStore
    {
        public static bool IsFlagged(string endpointName)
        {
            if (string.IsNullOrEmpty(endpointName))
            {
                return false;
            }

            return ReadFlaggedNames().Contains(endpointName);
        }

        public static void SetFlagged(string endpointName, bool flagged)
        {
            if (string.IsNullOrEmpty(endpointName))
            {
                return;
            }

            List<string> flaggedNames = ReadFlaggedNames();

            if (flagged && !flaggedNames.Contains(endpointName))
            {
                flaggedNames.Add(endpointName);
            }
            else if (!flagged)
            {
                flaggedNames.Remove(endpointName);
            }

            Settings.Default.VirusTotal_LocalOverrideList = string.Join("|", flaggedNames);
            Settings.Default.Save();
        }

        private static List<string> ReadFlaggedNames()
        {
            string raw = Settings.Default.VirusTotal_LocalOverrideList;

            return string.IsNullOrEmpty(raw)
                ? new List<string>()
                : raw.Split('|').Where(name => !string.IsNullOrEmpty(name)).ToList();
        }
    }
}
