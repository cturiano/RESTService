using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Service.Controllers;
using Service.DAL;
using Service.DAL.Tables;
using Service.Interfaces;
using Service.Models;
using Service.Properties;
using Unity;
using Unity.Injection;
using Unity.Lifetime;

namespace Service.Tests.DALTests
{
    [TestFixture]
    public class DatabaseHelpersTests
    {
        [Test]
        public async Task InitializeDatabaseAlbumExistsExceptionTest()
        {
            using (var container = Substitute.For<UnityContainer>())
            {
                var dataAccess = Substitute.For<IDataAccess>();
                dataAccess.OpenOrCreateDatabaseAsync().Returns(DatabaseState.New);
                dataAccess.ExecuteSqlWithParametersAsync(Arg.Any<string>(), Arg.Any<List<SQLiteParameter>>()).Returns(0);
                dataAccess.ExecuteQueryWithParametersAsync(Arg.Any<string>(), Arg.Any<List<SQLiteParameter>>()).Returns(a => (ICursor)null);

                container.RegisterInstance(Resources.DataAccessObjectName, dataAccess, new ContainerControlledLifetimeManager());

                container.RegisterType<AlbumController>(new InjectionFactory(s => new AlbumController(dataAccess)));
                container.RegisterType<ArtistController>(new InjectionFactory(s => new ArtistController(dataAccess)));
                container.RegisterType<GenreController>(new InjectionFactory(s => new GenreController(dataAccess)));
                container.RegisterType<StatisticsController>(new InjectionFactory(s => new StatisticsController(dataAccess)));

                try
                {
                    await DatabaseHelpers.InitializeDatabase(container);
                }
                catch (Exception ex)
                {
                    Assert.IsInstanceOf<SQLiteException>(ex);
                    Assert.AreEqual($"unknown error\r\n{Resources.AlbumExistsExceptionMessage}", ex.Message);
                }
            }
        }

        [Test]
        public async Task InitializeDatabaseAllUniqueIdsTest()
        {
            using (var container = Substitute.For<UnityContainer>())
            {
                var dataAccess = Substitute.For<IDataAccess>();
                dataAccess.OpenOrCreateDatabaseAsync().Returns(DatabaseState.New);
                dataAccess.ExecuteSqlWithParametersAsync(Arg.Any<string>(), Arg.Any<List<SQLiteParameter>>()).Returns(1);

                container.RegisterInstance(Resources.DataAccessObjectName, dataAccess, new ContainerControlledLifetimeManager());

                container.RegisterType<AlbumController>(new InjectionFactory(s => new AlbumController(dataAccess)));
                container.RegisterType<ArtistController>(new InjectionFactory(s => new ArtistController(dataAccess)));
                container.RegisterType<GenreController>(new InjectionFactory(s => new GenreController(dataAccess)));
                container.RegisterType<StatisticsController>(new InjectionFactory(s => new StatisticsController(dataAccess)));

                await DatabaseHelpers.InitializeDatabase(container);
            }
        }

        [Test]
        public async Task InitializeDatabaseErrorAddingItemExceptionTest()
        {
            using (var container = Substitute.For<UnityContainer>())
            {
                var artistTable = new ArtistTable();
                var albumTable = new AlbumTable();
                var genreTable = new GenreTable();

                var artistParameters = new List<SQLiteParameter>();
                var albumParameters = new List<SQLiteParameter>();
                var genreParameters = new List<SQLiteParameter>();

                var artistSql = artistTable.GetInsertSql(new Artist(0, "Michael Jackson"), ref artistParameters);
                var genreSql = genreTable.GetInsertSql(new Genre(0, "Pop"), ref genreParameters);
                var albumSql = albumTable.GetInsertSql(new Album(0, 0, 0, "Thriller", 1982), ref albumParameters);

                var dataAccess = Substitute.For<IDataAccess>();
                dataAccess.OpenOrCreateDatabaseAsync().Returns(DatabaseState.New);
                dataAccess.ExecuteQueryWithParametersAsync(Arg.Any<string>(), Arg.Any<List<SQLiteParameter>>()).Returns(a => (ICursor)null);

                dataAccess.ExecuteSqlWithParametersAsync(artistSql, Arg.Any<List<SQLiteParameter>>()).Returns(1);
                dataAccess.ExecuteSqlWithParametersAsync(genreSql, Arg.Any<List<SQLiteParameter>>()).Returns(0);
                dataAccess.ExecuteSqlWithParametersAsync(albumSql, Arg.Any<List<SQLiteParameter>>()).Returns(1);

                container.RegisterInstance(Resources.DataAccessObjectName, dataAccess, new ContainerControlledLifetimeManager());

                container.RegisterType<AlbumController>(new InjectionFactory(s => new AlbumController(dataAccess)));
                container.RegisterType<ArtistController>(new InjectionFactory(s => new ArtistController(dataAccess)));
                container.RegisterType<GenreController>(new InjectionFactory(s => new GenreController(dataAccess)));
                container.RegisterType<StatisticsController>(new InjectionFactory(s => new StatisticsController(dataAccess)));

                try
                {
                    await DatabaseHelpers.InitializeDatabase(container);
                }
                catch (Exception ex)
                {
                    Assert.IsInstanceOf<SQLiteException>(ex);
                    Assert.AreEqual($"unknown error\r\n{Resources.ErrorAddingItemMessage}", ex.Message);
                }
            }
        }
        
        [Test]
        public async Task InitializeDatabaseNonUniqueIdsExceptionTest()
        {
            using (var container = Substitute.For<UnityContainer>())
            {
                using (var cursor = Substitute.For<ICursor>())
                {
                    cursor.GetInt(0).Returns(1);

                    var artistTable = new ArtistTable();
                    var albumTable = new AlbumTable();
                    var genreTable = new GenreTable();

                    var artistParameters = new List<SQLiteParameter>();
                    var albumParameters = new List<SQLiteParameter>();
                    var genreParameters = new List<SQLiteParameter>();

                    var artistSql = artistTable.GetInsertSql(new Artist(0, "Michael Jackson"), ref artistParameters);
                    var genreSql = genreTable.GetInsertSql(new Genre(0, "Pop"), ref genreParameters);
                    var albumSql = albumTable.GetInsertSql(new Album(0, 0, 0, "Thriller", 1982), ref albumParameters);

                    var dataAccess = Substitute.For<IDataAccess>();
                    dataAccess.OpenOrCreateDatabaseAsync().Returns(DatabaseState.New);
                    dataAccess.ExecuteQueryWithParametersAsync(Arg.Any<string>(), Arg.Any<List<SQLiteParameter>>()).Returns(cursor);

                    dataAccess.ExecuteSqlWithParametersAsync(artistSql, Arg.Any<List<SQLiteParameter>>()).Returns(1);
                    dataAccess.ExecuteSqlWithParametersAsync(genreSql, Arg.Any<List<SQLiteParameter>>()).Returns(0);
                    dataAccess.ExecuteSqlWithParametersAsync(albumSql, Arg.Any<List<SQLiteParameter>>()).Returns(1);

                    container.RegisterInstance(Resources.DataAccessObjectName, dataAccess, new ContainerControlledLifetimeManager());

                    container.RegisterType<AlbumController>(new InjectionFactory(s => new AlbumController(dataAccess)));
                    container.RegisterType<ArtistController>(new InjectionFactory(s => new ArtistController(dataAccess)));
                    container.RegisterType<GenreController>(new InjectionFactory(s => new GenreController(dataAccess)));
                    container.RegisterType<StatisticsController>(new InjectionFactory(s => new StatisticsController(dataAccess)));

                    try
                    {
                        await DatabaseHelpers.InitializeDatabase(container);
                    }
                    catch (Exception ex)
                    {
                        Assert.IsInstanceOf<SQLiteException>(ex);
                        Assert.AreEqual($"unknown error\r\n{Resources.ErrorAddingItemMessage}", ex.Message);
                    }
                }
            }
        }
    }
}