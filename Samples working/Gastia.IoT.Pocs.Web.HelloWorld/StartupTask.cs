using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Windows.ApplicationModel.Background;
using System.Threading.Tasks;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace Gastia.IoT.Pocs.Web.HelloWorld
{
    public sealed class StartupTask : IBackgroundTask
    {
        private bool _isClosing = false;
        private BackgroundTaskDeferral _deferral;
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            taskInstance.Canceled += TaskInstance_Canceled;
            _deferral = taskInstance.GetDeferral();

            MyWebserver ws = new MyWebserver();
            var tasks = new Task[1];
            tasks[0] = Task.Run(async () => { await ws.Start(); });

            Task.WaitAll(tasks);

            _deferral.Complete();
        }

        private void TaskInstance_Canceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            _isClosing = true;
        }
    }
}
