using System;
using Microsoft.SPOT;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Komodex.NETMF.MicroTweet.HTTP
{
    public class HttpRequest : IDisposable
    {
        HttpResponse response = null;

        public HttpRequest(string uri)
            : this(new HttpUri(uri))
        { }

        public HttpRequest(HttpUri uri)
        {
            Uri = uri;
            Method = "GET";
            Headers = new HeaderCollection();
            Timeout = 30;
        }

        #region Properties

        public HttpUri Uri { get; set; }

        public string Method { get; set; }

        public HeaderCollection Headers { get; set; }

        public int Timeout { get; set; }
        
        public string ContentType { get; set; }
        
        public string PostContent { get; set; }
        
        public int ContentLength
        {
            get
            {
                if (PostContent == null)
                    return 0;
                return Encoding.UTF8.GetBytes(PostContent).Length;
            }
        }

        #endregion

        #region Public Methods

        public HttpResponse GetResponse()
        {
            // If the request has already been submitted, return the existing response
            if (response != null)
                return response;

            // Build the request
            string request = Method + " " + Uri.Path + " HTTP/1.1\r\n";
            request += "Host: " + Uri.Hostname + "\r\n";

            // Headers
            request += "Connection: Close\r\n";
            if (ContentType != null && ContentType != string.Empty)
                request += "Content-Type: " + ContentType + "\r\n";
            request += "Content-Length: " + ContentLength + "\r\n";
            request += Headers.ToString() + "\r\n\r\n";

            // POST content
            if (PostContent != null && PostContent != string.Empty)
                request += PostContent + "\r\n\r\n";

            // Get the server's IP address
            IPHostEntry hostEntry = Dns.GetHostEntry(Uri.Hostname);

            // Create socket and connect
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.SendTimeout = socket.ReceiveTimeout = Timeout * 1000;
            socket.Connect(new IPEndPoint(hostEntry.AddressList[0], Uri.Port));

            // Convert the HTTP request to a byte array
            byte[] requestBytes = Encoding.UTF8.GetBytes(request);
            request = null;

            // Send the request
            socket.Send(requestBytes);
            requestBytes = null;

            response = new HttpResponse(socket);
            return response;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (response != null)
                response.Dispose();
        }

        #endregion
    }
}
