using System;
using Microsoft.SPOT;
using System.IO;
using MFCommon.Network.Raw;

namespace MFCommon.Network
{
    public class NetworkUtils
    {
   
        public static StreamReader Get(string host, int port, string request)
        {
            return SocketUtils.GetBytes(host, port, request);
        }
    }
}
