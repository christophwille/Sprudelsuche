using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sprudelsuche.Portable;
using Windows.ApplicationModel.Background;

namespace Sprudelsuche.Tasks
{
    //
    // http://msdn.microsoft.com/en-us/library/windows/apps/hh974425%28v=vs.110%29.aspx#BKMK_Trigger_a_background_task_event_from_a_standard_debug_session
    //
    public sealed class UpdateTask : IBackgroundTask
    {
        private Func<ISprudelRepository> CreateSprudelRepository { get; set; }

        public UpdateTask()
        {
            CreateSprudelRepository = () => new Sprudelsuche.WinRT.SprudelRepository();
        }

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            Debug.WriteLine("Sprudelsuche: Background Task invoked @ " + DateTime.Now.ToString());

            taskInstance.Canceled += new BackgroundTaskCanceledEventHandler(OnCanceled);
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();

            var repo = CreateSprudelRepository();
            var refreshResult = await repo.RefreshAsync();

            deferral.Complete();
        }

        private void OnCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            Debug.WriteLine("Background " + sender.Task.Name + " Cancel Requested...");
        }
    }
}
