using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Service.DAL;
using Unity;

namespace Service
{
    public class WebApiApplication : HttpApplication
    {
        #region Static Fields and Constants

        protected static readonly UnityContainer Container = new UnityContainer();

        #endregion

        #region Protected Methods

        /// <summary>
        ///     This is the top level unhandled exception handler
        ///     Add code here to save state, log error messages, etc.
        /// </summary>
        protected void Application_Error()
        {
            // log the error
            throw new Exception("Fatal exception occurred.");
        }

        protected async Task Application_OnStart(object sender, EventArgs e)
        {
            UnityConfig.RegisterComponents(Container);
            GlobalConfiguration.Configure(WebApiConfig.Register);
            await BackgroundTaskScheduler.QueueBackgroundWorkItem(ct => DatabaseHelpers.InitializeDatabase(Container));
        }

        #endregion
    }
}