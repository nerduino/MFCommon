

using System;
using Microsoft.SPOT;
using System.Net.Sockets;
using System.Net;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;


namespace MFCommon.Network
{
    public class NTP
    {

        static string[] servers = new string[] { 
            "10.3.64.80" //local ntp server
            //"time-a.nist.gov", "time-b.nist.gov", "nist1-la.ustiming.org", "nist1-chi.ustiming.org", "nist1-ny.ustiming.org", "time-nw.nist.gov" 
        };

        public static DateTime NTPTime(int UTC_Offset)
        {
            foreach(string server in servers)
            {
                try
                {
                    return NTPTime(server, UTC_Offset);
                }
                catch { }

            }
            throw new SocketException(SocketError.HostUnreachable);
        }
        /// <summary>
        /// Get DateTime from NTP Server  
        /// Based on:
        /// http://weblogs.asp.net/mschwarz/archive/2008/03/09/wrong-datetime-on-net-micro-framework-devices.aspx
        /// </summary>
        /// <param name="TimeServer">Timeserver</param>
        /// <returns>Local NTP Time</returns>
        public static DateTime NTPTime(String TimeServer, int UTC_offset)
        {
            // Find endpoint for timeserver
            IPEndPoint ep = new IPEndPoint(Dns.GetHostEntry(TimeServer).AddressList[0], 123);

            // Connect to timeserver
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            
            // Make send/receive buffer
            byte[] ntpData = new byte[48];
            Array.Clear(ntpData, 0, 48);

            // Set protocol version
            ntpData[0] = 0x1B;

            // Send Request
            s.SendTo(ntpData, ep);

            // Receive Time
            s.Receive(ntpData);

            byte offsetTransmitTime = 40;

            ulong intpart = 0;
            ulong fractpart = 0;

            for (int i = 0; i <= 3; i++)
                intpart = (intpart <<  8 ) | ntpData[offsetTransmitTime + i];

            for (int i = 4; i <= 7; i++)
                fractpart = (fractpart <<  8 ) | ntpData[offsetTransmitTime + i];

            ulong milliseconds = (intpart * 1000 + (fractpart * 1000) / 0x100000000L);

            s.Close();

            TimeSpan timeSpan = TimeSpan.FromTicks((long)milliseconds * TimeSpan.TicksPerMillisecond);
            DateTime dateTime = new DateTime(1900, 1, 1);
            dateTime += timeSpan;

            TimeSpan offsetAmount = new TimeSpan(0, UTC_offset, 0, 0, 0);
            DateTime networkDateTime = (dateTime + offsetAmount);

            return networkDateTime;
        }

    }
}