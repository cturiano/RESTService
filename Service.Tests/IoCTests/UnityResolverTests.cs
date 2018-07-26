using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Service.Abstract;
using Service.Controllers;
using Service.DAL;
using Service.Interfaces;
using Service.IoC;
using Service.Models;
using Unity;
using Unity.Lifetime;

namespace Service.Tests.IoCTests
{
    [TestFixture]
    public class UnityResolverTests
    {
        private class UnityResolverTestClass : UnityResolver
        {
            #region Constructors

            public UnityResolverTestClass(IUnityContainer container) : base(container)
            {
            }

            #endregion

            #region Properties

            public IUnityContainer TestContainer => Container;

            #endregion
        }

        [Test]
        public void BeginScopeTest()
        {
            using (var container = new UnityContainer())
            {
                using (var resolver = new UnityResolver(container))
                {
                    var scope = resolver.BeginScope();
                    Assert.IsNotNull(scope);
                    Assert.AreNotSame(resolver, scope);
                }
            }
        }

        [Test]
        public void ConstructorAndPropertiesExceptionTest()
        {
            Assert.That(() => new UnityResolver(null), Throws.ArgumentNullException);
        }

        [Test]
        public void ConstructorAndPropertiesTest()
        {
            using (var container = new UnityContainer())
            {
                using (var resolver = new UnityResolverTestClass(container))
                {
                    Assert.IsNotNull(resolver);
                    Assert.IsInstanceOf<UnityResolver>(resolver);
                    Assert.IsNotNull(resolver.TestContainer);
                    Assert.AreSame(container, resolver.TestContainer);
                }
            }
        }

        [Test]
        public void DisposeTest()
        {
            using (var container = new UnityContainer())
            {
                var resolver = new UnityResolverTestClass(container);
                resolver.Dispose();
            }
        }

        [Test]
        public void GetServiceExceptionTest()
        {
            using (var dataAccess = new DataAccess("database"))
            {
                using (var container = new UnityContainer())
                {
                    container.RegisterInstance<IDataAccess>(dataAccess);
                    container.RegisterType<AbstractController<Album>, AlbumController>(new SingletonLifetimeManager());
                    container.RegisterType<AbstractController<Artist>, ArtistController>(new SingletonLifetimeManager());
                    container.RegisterType<AbstractController<Genre>, GenreController>(new SingletonLifetimeManager());

                    using (var resolver = new UnityResolverTestClass(container))
                    {
                        var service = resolver.GetService(typeof(ICursor));
                        Assert.IsNull(service);
                    }
                }
            }
        }

        [Test]
        public void GetServicesExceptionTest()
        {
            using (var dataAccess = new DataAccess("database"))
            {
                using (var container = new UnityContainer())
                {
                    container.RegisterInstance<IDataAccess>(dataAccess);
                    container.RegisterType<AbstractController<Album>, AlbumController>(new SingletonLifetimeManager());
                    container.RegisterType<AbstractController<Artist>, ArtistController>(new SingletonLifetimeManager());
                    container.RegisterType<AbstractController<Genre>, GenreController>(new SingletonLifetimeManager());

                    using (var resolver = new UnityResolverTestClass(container))
                    {
                        var services = resolver.GetServices(typeof(ICursor));
                        Assert.IsNotNull(services);
                        Assert.IsInstanceOf<List<object>>(services);
                        Assert.AreEqual(0, services.Count());
                    }
                }
            }
        }

        [Test]
        public void GetServicesTest()
        {
            using (var dataAccess = new DataAccess("database"))
            {
                using (var container = new UnityContainer())
                {
                    container.RegisterInstance<IDataAccess>("dataAccess", dataAccess);
                    container.RegisterType<AbstractController<Album>, AlbumController>(new SingletonLifetimeManager());
                    container.RegisterType<AbstractController<Artist>, ArtistController>(new SingletonLifetimeManager());
                    container.RegisterType<AbstractController<Genre>, GenreController>(new SingletonLifetimeManager());

                    using (var resolver = new UnityResolverTestClass(container))
                    {
                        var services = resolver.GetServices(typeof(IDataAccess));
                        Assert.IsNotNull(services);
                        Assert.AreEqual(1, services.Count());
                        Assert.AreSame(dataAccess, services.ElementAt(0));
                    }
                }
            }
        }

        [Test]
        public void GetServiceTest()
        {
            using (var dataAccess = new DataAccess("database"))
            {
                using (var container = new UnityContainer())
                {
                    container.RegisterInstance<IDataAccess>(dataAccess);
                    container.RegisterType<AbstractController<Album>, AlbumController>(new SingletonLifetimeManager());
                    container.RegisterType<AbstractController<Artist>, ArtistController>(new SingletonLifetimeManager());
                    container.RegisterType<AbstractController<Genre>, GenreController>(new SingletonLifetimeManager());

                    using (var resolver = new UnityResolverTestClass(container))
                    {
                        var service = resolver.GetService(typeof(IDataAccess));
                        Assert.IsNotNull(service);
                        Assert.AreEqual(dataAccess, service);
                    }
                }
            }
        }
    }
}