using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Windows.ApplicationModel.Background;
using System.Threading.Tasks;
using Gastia.IoT.POCs.Web.CmdBackgroundTask.Services;
using Gastia.IoT.POCs.Web.CmdBackgroundTask.Interfaces.HttpInterface;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace Gastia.IoT.POCs.Web.CmdBackgroundTask
{
    public sealed class StartupTask : IBackgroundTask
    {
        private const int PORT = 8081;
           
        private BackgroundTaskDeferral _deferral;
        private Task[] _tasks;
        private bool _isClosing = false;

        public bool IsClosing { get => _isClosing; }

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            // If you start any asynchronous methods here, prevent the task
            // from closing prematurely by using BackgroundTaskDeferral as
            // described in http://aka.ms/backgroundtaskdeferral

            taskInstance.Canceled += TaskInstance_Canceled;
            _deferral = taskInstance.GetDeferral();

            Webserver ws = new Webserver(PORT.ToString(),this);
            WebServer ws2 = new WebServer(PORT);
            _tasks = new Task[1];
            //_tasks[0] = Task.Run(async () => { await ws.Start(); });
            _tasks[0] = Task.Run(() => ws2.StartServer());
        }

        private void TaskInstance_Canceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            _isClosing = true;
            Task.WaitAll(_tasks);
            _deferral.Complete();
        }
    }
}
