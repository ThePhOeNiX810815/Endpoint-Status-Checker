using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace EndpointChecker
{
    internal sealed class EndpointNetworkShareNativeItem
    {
        public string Name { get; set; }

        public uint TypeCode { get; set; }

        public string Remark { get; set; }
    }

    internal static class EndpointNetworkShareEnumerator
    {
        [DllImport("Netapi32.dll", CharSet = CharSet.Unicode)]
        private static extern int NetShareEnum(
            StringBuilder serverName,
            int level,
            ref IntPtr bufferPointer,
            uint preferredMaximumLength,
            ref int entriesRead,
            ref int totalEntries,
            ref int resumeHandle);

        [DllImport("Netapi32.dll", SetLastError = true)]
        private static extern int NetApiBufferFree(IntPtr buffer);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct SHARE_INFO_1
        {
            public string shi1_netname;
            public uint shi1_type;
            public string shi1_remark;
        }

        public static List<string> Enumerate(string hostName, string statusNotAvailable)
        {
            List<EndpointNetworkShareNativeItem> nativeItems = new List<EndpointNetworkShareNativeItem>();

            int entriesRead = 0;
            int totalEntries = 0;
            int resumeHandle = 0;
            int structSize = Marshal.SizeOf(typeof(SHARE_INFO_1));
            IntPtr bufferPointer = IntPtr.Zero;
            StringBuilder host = new StringBuilder(hostName);

            int result = NetShareEnum(host, 1, ref bufferPointer, 0xFFFFFFFF, ref entriesRead, ref totalEntries, ref resumeHandle);
            if (result == 0)
            {
                IntPtr currentPointer = bufferPointer;
                for (int i = 0; i < entriesRead; i++)
                {
                    SHARE_INFO_1 shareInfo = (SHARE_INFO_1)Marshal.PtrToStructure(currentPointer, typeof(SHARE_INFO_1));
                    nativeItems.Add(new EndpointNetworkShareNativeItem
                    {
                        Name = shareInfo.shi1_netname,
                        TypeCode = shareInfo.shi1_type,
                        Remark = shareInfo.shi1_remark,
                    });

                    currentPointer = new IntPtr(currentPointer.ToInt32() + structSize);
                }

                NetApiBufferFree(bufferPointer);
                return BuildShareItems(nativeItems);
            }

            return new List<string>
            {
                statusNotAvailable + " (" + GetErrorMessage(result) + ")"
            };
        }

        public static List<string> BuildShareItems(IReadOnlyList<EndpointNetworkShareNativeItem> nativeItems)
        {
            List<string> netShares = new List<string>();

            if (nativeItems == null)
            {
                return netShares;
            }

            for (int i = 0; i < nativeItems.Count; i++)
            {
                EndpointNetworkShareNativeItem item = nativeItems[i];
                string share = "[" + GetTypeName(item.TypeCode) + "] " + item.Name;
                if (!string.IsNullOrEmpty(item.Remark))
                {
                    share += " (" + item.Remark + ")";
                }

                netShares.Add(share);
            }

            return netShares;
        }

        public static string GetErrorMessage(int code)
        {
            Dictionary<int, string> codeList = new Dictionary<int, string>
            {
                { 0, "OK" },
                { 5, "The user has insufficient privilege for this operation" },
                { 8, "Not enough memory" },
                { 65, "Network access is denied" },
                { 87, "Invalid parameter specified" },
                { 53, "The network path was not found" },
                { 123, "Invalid name" },
                { 124, "Invalid level parameter" },
                { 234, "More data available, buffer too small" },
                { 2102, "Device driver not installed" },
                { 2106, "This operation can be performed only on a server" },
                { 2114, "Server service not installed" },
                { 2123, "Buffer too small for fixed-length data" },
                { 2127, "Error encountered while executing function remotely" },
                { 2138, "The Workstation service is not started" },
                { 2141, "The server is not configured for this transaction (IPC$ is not shared)" },
                { 2351, "Invalid computername specified" }
            };

            return codeList.ContainsKey(code) ? codeList[code] : "Result Code: " + code;
        }

        public static string GetTypeName(uint code)
        {
            Dictionary<uint, string> codeList = new Dictionary<uint, string>
            {
                { 0, "Folder" },
                { 1, "Printer" },
                { 2, "Device" },
                { 3, "IPC" },
                { 2147483648, "Admin/Folder" },
                { 2147483649, "Admin/Printer" },
                { 2147483650, "Admin/Device" },
                { 2147483651, "Admin/IPC" }
            };

            return codeList.ContainsKey(code) ? codeList[code] : "Type Code: " + code;
        }
    }
}
