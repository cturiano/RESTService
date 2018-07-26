using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Service.Tests
{
    [TestFixture]
    public class BackgroundTaskSchedulerTests
    {
        [Test]
        public void QueueBackgroundWorkItemExceptionTest()
        {
            Assert.ThrowsAsync<Exception>(() => BackgroundTaskScheduler.QueueBackgroundWorkItem(async ct => await Task.Run(() => throw new Exception(), ct)));
        }
    }
}