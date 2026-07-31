using System;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;

namespace ArpLookup
{
    internal static class WindowsLookupService
    {
        // Serializes concurrent SendARP calls — parallel calls to the Windows IP Helper API
        // from multiple threads can trigger a kernel-level fault on certain NIC drivers,
        // causing an immediate machine restart (GitHub issue #36).
        private static readonly SemaphoreSlim _arpThrottle = new SemaphoreSlim(1, 1);

        public static string Lookup(IPAddress ip)
        {
            _ = ip ?? throw new ArgumentNullException(nameof(ip));

            _arpThrottle.Wait();
            try
            {
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
            finally
            {
                _arpThrottle.Release();
            }
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
