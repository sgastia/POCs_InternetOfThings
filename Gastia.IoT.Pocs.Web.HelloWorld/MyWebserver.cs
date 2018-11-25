using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace Gastia.IoT.Pocs.Web.HelloWorld
{
    internal class MyWebserver
    {
        //https://sandervandevelde.wordpress.com/2016/04/08/building-a-windows-10-iot-core-background-webserver/
        //ejemplo de llamado: http://pcplaymobil:8081/api/test?aaa=123

        private const uint BUFFER_SIZE = 8192;
        private const uint MAX_TRIALS = 10;
        private const string PORT = "8081";
        private StreamSocketListener _listener;
        public void Start()
        {
            _listener = new StreamSocketListener();
            _listener.ConnectionReceived += Listener_ConnectionReceived;
            IAsyncAction action = _listener.BindServiceNameAsync(PORT);
        }

        private async void Listener_ConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            StringBuilder request = await ReadRequest(args);
            Dictionary<string, string> queryStringValues = ParseQueryString(request);
            Respond(args, queryStringValues);
            
        }

        
        private async Task<StringBuilder> ReadRequest(StreamSocketListenerConnectionReceivedEventArgs args)
        {
            StringBuilder request = new StringBuilder();

            using (var input = args.Socket.InputStream)
            {
                var data = new byte[BUFFER_SIZE];
                IBuffer buffer = data.AsBuffer();
                var dataRead = BUFFER_SIZE;

                while (dataRead == BUFFER_SIZE)
                {
                    await input.ReadAsync(buffer, BUFFER_SIZE, InputStreamOptions.Partial);
                    request.Append(Encoding.UTF8.GetString(data, 0, data.Length));
                    dataRead = buffer.Length;
                }
            }
            return request;
        }

        private Dictionary<string, string> ParseQueryString(StringBuilder request)
        {
            var requestLines = request.ToString().Split(' ');

            var url = requestLines.Length > 1 ? requestLines[1] : string.Empty;

            var uri = new Uri("http://localhost" + url);
            var query = uri.Query;

            Dictionary<string, string> ret = new Dictionary<string, string>();
            string[] values = query.Substring(1).Split('&');//I'm assuming that the first char is always '?'
            foreach (string val in values)
            {
                string[] keyValue = val.Split('=');
                string key = keyValue[0];
                string value = keyValue[1];
                ret.Add(key, value);
            }
            return ret;
        }

        private async void Respond(StreamSocketListenerConnectionReceivedEventArgs args, Dictionary<string, string> queryStringValues)
        {
            using (var output = args.Socket.OutputStream)
            {
                using (var response = output.AsStreamForWrite())
                {
                    StringBuilder html = new StringBuilder($"<html><head><title>Background Message</title></head><body>Hello from the background process!<br/>");
                    foreach(var entry in queryStringValues)
                    {
                        html.Append($"The value for key {entry.Key} is: {entry.Value}<br>");
                    }
                    html.Append("</body></html>");
                    byte[] buffer = Encoding.UTF8.GetBytes(html.ToString());
                    

                    using (var bodyStream = new MemoryStream(buffer))
                    {
                        var header = $"HTTP/1.1 200 OK\r\nContent-Length: {bodyStream.Length}\r\nConnection: close\r\n\r\n";
                        var headerArray = Encoding.UTF8.GetBytes(header);
                        await response.WriteAsync(headerArray, 0, headerArray.Length);
                        await bodyStream.CopyToAsync(response);
                        await response.FlushAsync();
                    }
                }
            }
        }
    }
}
