using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Devices.Enumeration;

namespace Gastia.IoT.POCs.Web.CmdBackgroundTask.Services
{
    internal class Devices
    {
        private async Task<string> GetAllCameras()
        {
            var devices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
            List<DeviceInformation> deviceList = new List<DeviceInformation>();
            StringBuilder response = new StringBuilder();
            response.Append("{\"sequence\":[");
            if (devices.Count > 0)
            {
                for (var i = 0; i < devices.Count; i++)
                {
                    response.Append ("{\"cameraid\":\"" + devices[i].Name + "\"}");
                    if (i < deviceList.Count - 1)
                        response.Append(",");
                    deviceList.Add(devices[i]);
                }

            }
            response.Append("]]}");
            return response.ToString();
        }

        
    }
}