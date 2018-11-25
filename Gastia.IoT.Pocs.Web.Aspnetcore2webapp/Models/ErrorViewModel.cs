using System;

namespace Gastia.IoT.Pocs.Web.Aspnetcore2webapp.Models
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}