using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Gastia.IoT.POCs.Web.CmdBackgroundTask.Services
{
    internal class CameraService
    {
        

        internal async Task<string> GetAllCameras()
        {
            var devices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
            List<DeviceInformation> deviceList = new List<DeviceInformation>();
            string ret = "\"sequence\":[";
            if (devices.Count > 0)
            {
                for (var i = 0; i < devices.Count; i++)
                {
                    ret += "{\"cameraid\":\""+devices[i].Name+"\"}";
                    if (i < deviceList.Count - 1)
                        ret += ",";
                    deviceList.Add(devices[i]);
                }

            }
            ret += "]";
            return ret;
        }


        
    }
}
