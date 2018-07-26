using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using NUnit.Framework;
using Service.Abstract;
using Service.Controllers;
using Service.DAL;
using Service.Interfaces;
using Service.IoC;
using Service.Models;
using Service.Properties;
using Unity;

namespace Service.Tests
{
    [TestFixture]
    public class WebApiApplicationTests
    {
        private class WebApiApplicationTestClass : WebApiApplication
        {
            #region Properties

            public UnityContainer TestContainer => Container;

            #endregion

            #region Public Methods

            public void TestApplication_Error()
            {
                Application_Error();
            }

            public async Task TestApplication_OnStart(object sender, EventArgs e)
            {
                await Application_OnStart(sender, e);
            }

            #endregion
        }

        [Test]
        public void Application_ErrorTest()
        {
            var waa = new WebApiApplicationTestClass();
            Assert.That(() => waa.TestApplication_Error(), Throws.Exception);
        }

        [Test]
        public async Task Application_StartTest()
        {
            var httpConfig = GlobalConfiguration.Configuration;
            httpConfig.Routes.MapHttpRoute("DefaultApi", "api/{controller}/");
            httpConfig.EnsureInitialized();

            var waa = new WebApiApplicationTestClass();
            await waa.TestApplication_OnStart(new object(), new EventArgs());

            var resolver = GlobalConfiguration.Configuration.DependencyResolver;
            Assert.IsNotNull(resolver);
            Assert.IsInstanceOf<UnityResolver>(resolver);
            var container = waa.TestContainer;
            Assert.IsNotNull(container);
            Assert.AreEqual(6, container.Registrations.Count());

            var dataAccess = container.Resolve<IDataAccess>(Resources.DataAccessObjectName);
            Assert.IsNotNull(dataAccess);
            Assert.IsInstanceOf<DataAccess>(dataAccess);
            Assert.AreEqual(Resources.DatabaseName, ((DataAccess)dataAccess).DatabaseName);

            var albumController = container.Resolve<AlbumController>();
            Assert.IsNotNull(albumController);
            Assert.IsInstanceOf<AbstractController<Album>>(albumController);

            var artistController = container.Resolve<ArtistController>();
            Assert.IsNotNull(artistController);
            Assert.IsInstanceOf<AbstractController<Artist>>(artistController);

            var genreController = container.Resolve<GenreController>();
            Assert.IsNotNull(genreController);
            Assert.IsInstanceOf<AbstractController<Genre>>(genreController);

            var statisticsController = container.Resolve<StatisticsController>();
            Assert.IsNotNull(statisticsController);
            Assert.IsInstanceOf<StatisticsController>(statisticsController);
        }
    }
}