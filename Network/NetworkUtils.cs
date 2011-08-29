using System;
using Microsoft.SPOT;
using Komodex.NETMF.MicroTweet.HTTP;
using System.IO;
using MFCommon.Network.Raw;

namespace MFCommon.Network
{
    public class NetworkUtils
    {
        public static string HttpGet(string url)
        {
            string result = "";
            using (HttpRequest httpRequest = new HttpRequest(new HttpUri(url)))
            using (HttpResponse response = httpRequest.GetResponse())
            {
                StreamReader reader = response.ResponseStreamReader;
                result = reader.ReadToEnd();
            }
            return result;
        }

        public static StreamReader Get(string host, int port, string request)
        {
            return SocketUtils.GetBytes(host, port, request);
        }
    }
}
