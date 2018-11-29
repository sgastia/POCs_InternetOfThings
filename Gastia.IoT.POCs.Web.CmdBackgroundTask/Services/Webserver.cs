using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace Gastia.IoT.POCs.Web.CmdBackgroundTask.Services
{
    internal class Webserver:IDisposable
    {
        private const uint BUFFER_SIZE = 8192;
        private const uint MAX_TRIALS = 10;

        private StreamSocketListener _listener;
        private string _port;
        private Commander _commander;
        private StartupTask _startupTask;

        private static int calls = 0;

        public Webserver(string port, StartupTask startupTask)
        {
            this._port = port;
            _commander = new Commander();
            _startupTask = startupTask;
        }

        public async Task Start()
        {
            _listener = new StreamSocketListener();
            _listener.Control.KeepAlive = true;
            _listener.Control.NoDelay = true;
            _listener.ConnectionReceived += (sender, args) =>
            {
                try
                {
                    // Process incoming request
                    ProcessRequestAsync(sender,args);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Exception in StreamSocketListener.ConnectionReceived(): " + ex.Message);
                }
            };
            await _listener.BindServiceNameAsync(this._port);
        }

        private async void ProcessRequestAsync(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            DataReader reader = new DataReader(args.Socket.InputStream);
            while(true)
            {
                if (_startupTask == null || _startupTask.IsClosing)
                    return;
                try
                {
                    calls = calls +1;
                    //StringBuilder request = await ReadRequest(reader);//https://github.com/Microsoft/Windows-universal-samples/blob/master/Samples/StreamSocket/cs/Scenario1_Start.xaml.cs --> OnConnection(...)
                    StringBuilder request = await ReadRequest(args.Socket.InputStream);//https://sandervandevelde.wordpress.com/2016/04/08/building-a-windows-10-iot-core-background-webserver/
                    StringBuilder responseContent = new StringBuilder();
                    responseContent.Append("{'calls':" + calls + ",");
                    responseContent.Append($"'time':'{ DateTime.Now.ToString()}'");
                    responseContent.Append("response:");
                    responseContent.Append(_commander.Process(request));
                    responseContent.Append("}");
                    Respond(args, responseContent);
                }
                catch(Exception ex)
                {
                    throw ex;
                }
            }
        }

        private async Task<StringBuilder> ReadRequest(DataReader reader)
        {
            // Read first 4 bytes (length of the subsequent string).
            uint sizeFieldCount = await reader.LoadAsync(sizeof(uint));
            if (sizeFieldCount != sizeof(uint))
            {
                // 
                return new StringBuilder("{'error':'The underlying socket was closed before we were able to read the first 4 bytes of data.'}");
            }

            // Read the string.
            uint stringLength = reader.ReadUInt32();
            uint actualStringLength = await reader.LoadAsync(stringLength);
            if (stringLength != actualStringLength)
            {
                // The underlying socket was closed before we were able to read the whole data.
                return new StringBuilder("{'error':'The underlying socket was closed before we were able to read the whole data.'}");
            }

            string request = reader.ReadString(actualStringLength);
            return new StringBuilder(request);
        }

        private async Task<StringBuilder> ReadRequest(IInputStream inputStream)
        {
            StringBuilder request = new StringBuilder();

            var data = new byte[BUFFER_SIZE];
            IBuffer buffer = data.AsBuffer();
            var dataRead = BUFFER_SIZE;

            while (dataRead == BUFFER_SIZE)
            {
                await inputStream.ReadAsync(buffer, BUFFER_SIZE, InputStreamOptions.Partial);
                request.Append(Encoding.UTF8.GetString(data, 0, data.Length));
                dataRead = buffer.Length;
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

        private async void Respond(StreamSocketListenerConnectionReceivedEventArgs args, StringBuilder responseContent)
        {
            using (var output = args.Socket.OutputStream)
            {
                using (var response = output.AsStreamForWrite())
                {
                    byte[] buffer = Encoding.UTF8.GetBytes(responseContent.ToString());
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

        public void Dispose()
        {
            
        }
    }
}
