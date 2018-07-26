using System.Web.Http;
using Service.Abstract;
using Service.Controllers;
using Service.DAL;
using Service.Interfaces;
using Service.IoC;
using Service.Models;
using Service.Properties;
using Unity;
using Unity.Injection;
using Unity.Lifetime;

namespace Service
{
    public static class UnityConfig
    {
        #region Public Methods

        public static void RegisterComponents(IUnityContainer container)
        {
            var dataAccess = new DataAccess(Resources.DatabaseName);

            // register all your components with the container here
            container.RegisterInstance<IDataAccess>(Resources.DataAccessObjectName, dataAccess, new ContainerControlledLifetimeManager());

            container.RegisterType<AlbumController>(new InjectionFactory(s => new AlbumController(dataAccess)));
            container.RegisterType<ArtistController>(new InjectionFactory(s => new ArtistController(dataAccess)));
            container.RegisterType<GenreController>(new InjectionFactory(s => new GenreController(dataAccess)));
            container.RegisterType<StatisticsController>(new InjectionFactory(s => new StatisticsController(dataAccess)));

            GlobalConfiguration.Configuration.DependencyResolver = new UnityResolver(container);
        }

        #endregion
    }
}