using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace ArpLookup
{
    internal static class WindowsLookupService
    {
        public static string Lookup(IPAddress ip)
        {
            _ = ip ?? throw new ArgumentNullException(nameof(ip));

            var destIp = BitConverter.ToUInt32(ip.GetAddressBytes(), 0);

            var addr = new byte[6];
            var len = addr.Length;

            var res = NativeMethods.SendARP(destIp, 0, addr, ref len);

            if (res == 0)
            {
                return string.Join(":", (from z in addr select z.ToString("X2")).ToArray());
            }
            else if (res == NativeMethods.ERROR_BAD_NET_NAME)
            {
                return null;
            }

            throw new Win32Exception(res);
        }

        private static class NativeMethods
        {
            public const int ERROR_BAD_NET_NAME = 67;

            [DllImport("IPHlpApi", ExactSpelling = true)]
            [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
            [SecurityCritical]
            internal static extern int SendARP(uint destinationIp, uint sourceIp, byte[] macAddress, ref int physicalAddrLength);
        }
    }
}
