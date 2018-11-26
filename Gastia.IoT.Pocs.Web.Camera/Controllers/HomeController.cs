using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Gastia.IoT.Pocs.Web.Camera.Models;
using Gastia.IoT.Pocs.Web.Camera.Services;

namespace Gastia.IoT.Pocs.Web.Camera.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            /*
            CameraService mgr = new CameraService();
            JsonResult devices = await mgr.EnumerateCameras();
            return Ok(devices);
            */

            return View("~/Views/Home/Index.cshtml");
        }


        

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
