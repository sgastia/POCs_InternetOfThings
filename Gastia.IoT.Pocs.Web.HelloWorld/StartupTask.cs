using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Windows.ApplicationModel.Background;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace Gastia.IoT.Pocs.Web.HelloWorld
{
    public sealed class StartupTask : IBackgroundTask
    {
        private BackgroundTaskDeferral _deferral;
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            // If you start any asynchronous methods here, prevent the task
            // from closing prematurely by using BackgroundTaskDeferral as
            // described in http://aka.ms/backgroundtaskdeferral

            _deferral = taskInstance.GetDeferral();

            /*
            var webserver = new MyWebserver();
            //no compila porque Run tiene que ser aync
            await ThreadPool.RunAsync(workItem =>
            {
                webserver.Start();
            });
            */

            var webserver = new MyWebserver();
            webserver.Start();
        }
    }
}
