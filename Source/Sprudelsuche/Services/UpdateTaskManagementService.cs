using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace Sprudelsuche.Services
{
    public static class UpdateTaskManagementService
    {
        private const string UpdateTaskEntryPoint = "Sprudelsuche.Tasks.UpdateTask";
        private const string UpdateTaskName = "SprudelUpdateTask";
        private const int UpdateTaskTimeTriggerInterval = 60;

        public static async Task<BackgroundTaskRegistration> Register()
        {
            // If the user denies this once, it cannot be re-enabled: uninstall / install from the Start screen...
            var status = await BackgroundExecutionManager.RequestAccessAsync();

            if (status == BackgroundAccessStatus.Denied || status == BackgroundAccessStatus.Unspecified)
            {
                return null;
            }

            var builder = new BackgroundTaskBuilder();

            builder.Name = UpdateTaskName;
            builder.TaskEntryPoint = UpdateTaskEntryPoint;
            builder.SetTrigger(new TimeTrigger(UpdateTaskTimeTriggerInterval, false));

            IBackgroundCondition condition = new SystemCondition(SystemConditionType.InternetAvailable);
            builder.AddCondition(condition);

            BackgroundTaskRegistration task = builder.Register();
            return task;
        }

        public static void Unregister()
        {
            var task = GetTaskRegistration();

            if (null != task)
            {
                task.Unregister(true);
            }
        }

        public static IBackgroundTaskRegistration GetTaskRegistration()
        {
            return BackgroundTaskRegistration.AllTasks
                .Where(t => t.Value.Name == UpdateTaskName)
                .Select(t => t.Value)
                .FirstOrDefault();
        }

        public static bool IsTaskRegistered()
        {
            return BackgroundTaskRegistration.AllTasks
                .Any(t => t.Value.Name == UpdateTaskName);
        }
    }
}
