using System;
using System.Text;

namespace Gastia.IoT.POCs.Web.CmdBackgroundTask.Services
{
    internal class Commander
    {
        public Commander()
        {
            
        }
            
        internal StringBuilder Process(StringBuilder request)
        {

            StringBuilder response = new StringBuilder();
            response.Append("{");

            string content =(new CameraService()).GetAllCameras().Result;
            response.Append(content);

            response.Append("}");
            return response;
        }
    }
}