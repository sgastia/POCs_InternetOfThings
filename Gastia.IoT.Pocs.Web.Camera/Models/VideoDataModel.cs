using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gastia.IoT.Pocs.Web.Aspnetcore2webapp.Models
{
    public class VideoDataModel
    {
        public byte[] File { get; internal set; }
        public string FileType { get; internal set; }
    }
}
