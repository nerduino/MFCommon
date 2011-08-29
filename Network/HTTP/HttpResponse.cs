using System;
using Microsoft.SPOT;
using System.Net.Sockets;
using System.IO;

namespace Komodex.NETMF.MicroTweet.HTTP
{
    public class HttpResponse : IDisposable
    {
        internal HttpResponse(Socket socket)
        {
            // Set up the network stream and stream reader
            NetworkStream netStream = new NetworkStream(socket, true);
            StreamReader streamReader = new StreamReader(netStream);
            
            // Read the HTTP headers
            do
            {
                // Get the next line
                string headerLine = streamReader.ReadLine();
                if (headerLine == null || headerLine == string.Empty)
                    break; // End of headers

                // Split the header line (example: "Status: 200 OK")
                string[] values = headerLine.Split(new char[] { ':' }, 2);
                if (values.Length < 2)
                    continue;

                // Trim the header and value
                string header = values[0].Trim();
                string value = values[1].Trim();

                // Process each header
                switch (header.ToLower())
                {
                    // "Status: 200 OK"
                    case "status":
                        try
                        {
                            ResponseCode = int.Parse(value.Split(' ')[0]);
                        }
                        catch { }
                        break;

                    // "Content-Length: 725"
                    case "content-length":
                        try
                        {
                            ContentLength = long.Parse(value);
                        }
                        catch { }
                        break;

                    default:
                        break;
                }
            } while (!streamReader.EndOfStream);

            ResponseStreamReader = streamReader;
        }

        #region Properties

        public StreamReader ResponseStreamReader { get; protected set; }

        public int ResponseCode { get; protected set; }

        public long ContentLength { get; protected set; }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (ResponseStreamReader != null)
                ResponseStreamReader.Dispose();
        }

        #endregion
    }
}
