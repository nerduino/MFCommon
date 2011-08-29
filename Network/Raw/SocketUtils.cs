using System;
using System.Net;
using System.Net.Sockets;
using Microsoft.SPOT;
using System.IO;
using System.Text;

namespace MFCommon.Network.Raw
{
    public class SocketUtils
    {
        public static StreamReader GetBytes(string host, int port, string request)
        {
            // Get the server's IP address
            IPHostEntry hostEntry = Dns.GetHostEntry(host);

            // Create socket and connect
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                socket.SendTimeout = 30000;
                socket.ReceiveTimeout = 30000;
                socket.Connect(new IPEndPoint(hostEntry.AddressList[0], port));

                // Send the request
                if (request != null)
                {
                    socket.Send(Encoding.UTF8.GetBytes(request));
                }

                return new StreamReader(new NetworkStream(socket, true));               
            }
        }
    }
}
