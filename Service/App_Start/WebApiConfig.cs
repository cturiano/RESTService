using System.Web.Http;

namespace Service
{
    public static class WebApiConfig
    {
        #region Public Methods

        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Register Types

            // Web API routes
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute("IdApi", "api/{controller}/{id}", new {id = RouteParameter.Optional}, new {id = @"\d+"});
            config.Routes.MapHttpRoute("NameApi", "api/{controller}/{name}", new {name = RouteParameter.Optional}, new {name = @"\w+"});
            config.Routes.MapHttpRoute("AllApi", "api/{controller}");
        }

        #endregion
    }
}