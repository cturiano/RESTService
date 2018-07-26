using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Hosting;

namespace Service
{
    /// <summary>
    ///     Schedules work items on the HostingEnvironment if possible, or simply invokes the work item if not possible
    /// </summary>
    public static class BackgroundTaskScheduler
    {
        #region Public Methods

        /// <summary>
        ///     Queues the work item to complete on a background task
        /// </summary>
        /// <param name="workItem">work item to enqueue</param>
        public static async Task QueueBackgroundWorkItem(Func<CancellationToken, Task> workItem)
        {
            if (HostingEnvironment.IsHosted)
            {
                HostingEnvironment.QueueBackgroundWorkItem(workItem);
            }
            else
            {
                await workItem.Invoke(new CancellationToken());
            }
        }

        #endregion
    }
}