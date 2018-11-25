using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Windows.ApplicationModel.Background;
using System.Threading.Tasks;
using System.Net.WebSockets;
using System.Threading;
using System.Diagnostics;
using System.IO;
using Windows.Devices.Enumeration;
using Windows.Data.Json;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace Gastia.IoT.Pocs.Web.CameraBackgroundTask
{
    public sealed class StartupTask : IBackgroundTask
    {
        private const string WebInterfaceUrl = "ws://localhost:5000/";

        private bool _isClosing = false;
        private BackgroundTaskDeferral _deferral;

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            taskInstance.Canceled += TaskInstance_Canceled;
            _deferral = taskInstance.GetDeferral();

            var tasks = new Task[2];
            tasks[0] = Task.Run(async () => { await Report(); });
            tasks[1] = Task.Run(async () => { await Read(); });

            Task.WaitAll(tasks);

            _deferral.Complete();
        }

        private void TaskInstance_Canceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            _isClosing = true;
        }

        /// <summary>
        /// It sends random numbers to web application using WebSocket every 10 seconds
        /// </summary>
        /// <returns></returns>
        private async Task Report()
        {
            var rnd = new Random();

            using (var client = new ClientWebSocket())
            {
                var ct = new CancellationToken();

                await client.ConnectAsync(new Uri(WebInterfaceUrl), ct);

                while (true)
                {
                    if (_isClosing)
                    {
                        break;
                    }

                    var message = "Report " + Math.Round(rnd.NextDouble() * 100, 2);
                    await SendStringAsync(client, message);
                    await Task.Delay(10000);
                }

                await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", ct);
            }
        }

        /// <summary>
        /// It reads commands inserted through browser (displayed in debug window).
        /// </summary>
        /// <returns></returns>
        private async Task Read()
        {
            using (var client = new ClientWebSocket())
            {
                var ct = new CancellationToken();

                await client.ConnectAsync(new Uri(WebInterfaceUrl), ct);

                while (true)
                {
                    if (_isClosing)
                    {
                        break;
                    }

                    var fromSocket = await ReceiveStringAsync(client, ct);
                    Debug.WriteLine(fromSocket);

                    if (fromSocket != null)
                    {
                        if (fromSocket.Trim().ToLower() == "cameras")
                        {
                            var devices = await Windows.Devices.Enumeration.DeviceInformation.FindAllAsync(Windows.Devices.Enumeration.DeviceClass.VideoCapture);
                            //List<DeviceInformation> deviceList = new List<Windows.Devices.Enumeration.DeviceInformation>();
                            JsonArray deviceList = new JsonArray();
                            if (devices.Count > 0)
                            {
                                for (var i = 0; i < devices.Count; i++)
                                {
                                    IJsonValue jv = JsonValue.CreateStringValue(devices[i].Name);
                                    deviceList.Add(jv);
                                }

                                //InitCaptureSettings();
                                //InitMediaCapture();
                            }
                            string json = deviceList.Stringify();
                            await SendStringAsync(client,json , ct);
                        }
                    }
                }

                await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", ct);
            }
        }

        private static async Task<string> ReceiveStringAsync(ClientWebSocket socket, CancellationToken ct = default(CancellationToken))
        {
            var buffer = new ArraySegment<byte>(new byte[8192]);
            using (var ms = new MemoryStream())
            {
                WebSocketReceiveResult result;
                do
                {
                    ct.ThrowIfCancellationRequested();

                    result = await socket.ReceiveAsync(buffer, ct);
                    ms.Write(buffer.Array, buffer.Offset, result.Count);
                }
                while (!result.EndOfMessage);

                ms.Seek(0, SeekOrigin.Begin);
                if (result.MessageType != WebSocketMessageType.Text)
                {
                    return null;
                }

                using (var reader = new StreamReader(ms, Encoding.UTF8))
                {
                    return await reader.ReadToEndAsync();
                }
            }
        }

        private static async Task SendStringAsync(ClientWebSocket client, string message, CancellationToken ct = default(CancellationToken))
        {
            var segment = new ArraySegment<byte>(Encoding.UTF8.GetBytes(message));
            await client.SendAsync(segment, WebSocketMessageType.Text, true, ct);
        }
    }
}
