using System;
using System.Net;
using System.Net.Sockets;

namespace Knom.Helpers.Net
{
    public static class WakeOnLan
    {
        public static void WakeUp(string macAddress, string ipAddress, string subnetMask)
        {
            UdpClient client = new UdpClient();

            Byte[] datagram = new byte[102];

            for (int i = 0; i <= 5; i++)
            {
                datagram[i] = 0xff;
            }

            string[] macDigits = null;
            if (macAddress.Contains("-"))
            {
                macDigits = macAddress.Split('-');
            }
            else
            {
                macDigits = macAddress.Split(':');
            }

            if (macDigits.Length != 6)
            {
                throw new ArgumentException("Incorrect MAC address supplied!");
            }

            int start = 6;
            for (int i = 0; i < 16; i++)
            {
                for (int x = 0; x < 6; x++)
                {
                    datagram[start + i * 6 + x] = (byte)Convert.ToInt32(macDigits[x], 16);
                }
            }

            IPAddress address = IPAddress.Parse(ipAddress);
            IPAddress mask = IPAddress.Parse(subnetMask);
            IPAddress broadcastAddress = address.GetBroadcastAddress(mask);

            client.Send(datagram, datagram.Length, broadcastAddress.ToString(), 3);
        }
    }
}
