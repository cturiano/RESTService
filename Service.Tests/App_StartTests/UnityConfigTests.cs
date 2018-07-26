using System.Linq;
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
    public class UnityConfigTests
    {
        [Test]
        public void RegisterComponentsTest()
        {
            var container = new UnityContainer();
            UnityConfig.RegisterComponents(container);
            var resolver = GlobalConfiguration.Configuration.DependencyResolver;
            Assert.IsNotNull(resolver);
            Assert.IsInstanceOf<UnityResolver>(resolver);
            Assert.IsNotNull(container);
            Assert.AreEqual(6, container.Registrations.Count());

            var dataAccess = container.Resolve<IDataAccess>(Resources.DataAccessObjectName);
            Assert.IsNotNull(dataAccess);
            Assert.IsInstanceOf<DataAccess>(dataAccess);
            Assert.AreEqual(Resources.DatabaseName, (dataAccess as DataAccess).DatabaseName);

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