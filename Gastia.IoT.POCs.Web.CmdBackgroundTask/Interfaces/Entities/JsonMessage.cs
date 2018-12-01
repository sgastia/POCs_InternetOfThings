using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;

namespace Gastia.IoT.POCs.Web.CmdBackgroundTask.Interfaces.Entities
{
    internal class JsonMessage<T>
    {
        public bool Ok { get; set; }
        public string ErrorMessage { get; set; }

        public T Content { get; set; }

        public string Stringify()
        {
            JsonObject jo = new JsonObject();
            jo["ok"] = JsonValue.CreateBooleanValue(Ok);
            if(!string.IsNullOrEmpty(ErrorMessage))
                jo["ErrorMessage"] = JsonValue.CreateStringValue(ErrorMessage);
            if(Content is string)
            {
                jo["content"] = JsonValue.CreateStringValue(Content as string);
            }
            //TODO: It's pending to add the content

            return jo.Stringify();
        }
    }
}
