using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Service.Abstract;
using Service.Controllers;
using Service.DAL.Tables;
using Service.Interfaces;
using Service.Models;

namespace Service.Tests.ControllersTests
{
    [TestFixture]
    public class AlbumControllerTests
    {
        private class AlbumControllerTestClass : AlbumController
        {
            #region Constructors

            public AlbumControllerTestClass(IDataAccess dataAccess) : base(dataAccess)
            {
            }

            #endregion

            #region Properties

            public AbstractTable<Album> TestTable => Table;

            #endregion

            #region Public Methods

            public Album MakeItem(ICursor cursor, Dictionary<string, int> indexMap) => MakeItemAsync(cursor, indexMap).Result;

            #endregion
        }

        [Test]
        public void ConstructorAndPropertiesTest()
        {
            using (var dataAccess = Substitute.For<IDataAccess>())
            {
                using (var albumController = new AlbumControllerTestClass(dataAccess))
                {
                    Assert.IsNotNull(albumController);
                    Assert.IsInstanceOf<AlbumController>(albumController);
                    Assert.IsInstanceOf<AbstractController<Album>>(albumController);
                    Assert.IsNotNull(albumController.TestTable);
                    Assert.IsInstanceOf<AbstractTable<Album>>(albumController.TestTable);
                }
            }
        }

        [Test]
        public void DeleteAsyncIntTest()
        {
            using (var dataAccess = Substitute.For<IDataAccess>())
            {
                dataAccess.ExecuteSqlWithParametersAsync(Arg.Any<string>(), Arg.Any<List<SQLiteParameter>>()).Returns(1);

                using (var albumController = new AlbumController(dataAccess))
                {
                    var t = albumController.DeleteAsync(1);
                    Assert.IsInstanceOf<Task>(t);
                }
            }
        }

        [Test]
        public void DeleteAsyncStringTest()
        {
            using (var dataAccess = Substitute.For<IDataAccess>())
            {
                dataAccess.ExecuteSqlWithParametersAsync(Arg.Any<string>(), Arg.Any<List<SQLiteParameter>>()).Returns(1);

                using (var albumController = new AlbumController(dataAccess))
                {
                    var t = albumController.DeleteAsync("string");
                    Assert.IsInstanceOf<Task>(t);
                }
            }
        }

        [Test]
        public void DisposeTest()
        {
            using (var dataAccess = Substitute.For<IDataAccess>())
            {
                var counter = 0;
                dataAccess.When(d => d.Dispose()).Do(d => counter++);
                using (var albumController = new AlbumController(dataAccess))
                {
                    albumController.Dispose();
                    GC.ReRegisterForFinalize(albumController);
                    Assert.AreEqual(1, counter);
                }
            }
        }

        [Test]
        public void GetAsyncIntTest()
        {
            using (var cursor = Substitute.For<ICursor>())
            {
                cursor.GetInt(0).Returns(0);
                cursor.GetInt(1).Returns(1);
                cursor.GetInt(2).Returns(2);
                cursor.GetInt(3).Returns(1999);
                cursor.GetString(4).Returns("string");
                cursor.GetColumnIndex("id").Returns(0);
                cursor.GetColumnIndex("artist_id").Returns(1);
                cursor.GetColumnIndex("genre_id").Returns(2);
                cursor.GetColumnIndex("year").Returns(3);
                cursor.GetColumnIndex("name").Returns(4);

                using (var dataAccess = Substitute.For<IDataAccess>())
                {
                    dataAccess.ExecuteQueryWithParametersAsync(Arg.Any<string>(), Arg.Any<List<SQLiteParameter>>()).Returns(cursor);

                    using (var albumController = new AlbumController(dataAccess))
                    {
                        var album = albumController.GetAsync(1).Result;
                        Assert.IsNotNull(album);
                        Assert.IsInstanceOf<Album>(album);
                        Assert.AreEqual(0, album.Id);
                        Assert.AreEqual(1, album.ArtistId);
                        Assert.AreEqual(2, album.GenreId);
                        Assert.AreEqual(1999, album.Year);
                        Assert.AreEqual("string", album.Name);
                    }
                }
            }
        }

        [Test]
        public void GetAsyncStringTest()
        {
            using (var cursor = Substitute.For<ICursor>())
            {
                cursor.GetInt(0).Returns(0);
                cursor.GetInt(1).Returns(1);
                cursor.GetInt(2).Returns(2);
                cursor.GetInt(3).Returns(1999);
                cursor.GetString(4).Returns("string");
                cursor.GetColumnIndex("id").Returns(0);
                cursor.GetColumnIndex("artist_id").Returns(1);
                cursor.GetColumnIndex("genre_id").Returns(2);
                cursor.GetColumnIndex("year").Returns(3);
                cursor.GetColumnIndex("name").Returns(4);

                using (var dataAccess = Substitute.For<IDataAccess>())
                {
                    dataAccess.ExecuteQueryWithParametersAsync(Arg.Any<string>(), Arg.Any<List<SQLiteParameter>>()).Returns(cursor);

                    using (var albumController = new AlbumController(dataAccess))
                    {
                        var album = albumController.GetAsync("string").Result;
                        Assert.IsNotNull(album);
                        Assert.IsInstanceOf<Album>(album);
                        Assert.AreEqual(0, album.Id);
                        Assert.AreEqual(1, album.ArtistId);
                        Assert.AreEqual(2, album.GenreId);
                        Assert.AreEqual(1999, album.Year);
                        Assert.AreEqual("string", album.Name);
                    }
                }
            }
        }

        [Test]
        public void GetAsyncTest()
        {
            using (var cursor = Substitute.For<ICursor>())
            {
                cursor.GetInt(0).Returns(0);
                cursor.GetInt(1).Returns(1);
                cursor.GetInt(2).Returns(2);
                cursor.GetInt(3).Returns(1999);
                cursor.GetString(4).Returns("string");
                cursor.GetColumnIndex("id").Returns(0);
                cursor.GetColumnIndex("artist_id").Returns(1);
                cursor.GetColumnIndex("genre_id").Returns(2);
                cursor.GetColumnIndex("year").Returns(3);
                cursor.GetColumnIndex("name").Returns(4);

                using (var dataAccess = Substitute.For<IDataAccess>())
                {
                    dataAccess.ExecuteQueryWithParametersAsync(Arg.Any<string>(), Arg.Any<List<SQLiteParameter>>()).Returns(cursor);

                    using (var albumController = new AlbumController(dataAccess))
                    {
                        var albums = albumController.GetAsync().Result.ToList();
                        Assert.IsNotNull(albums);
                        Assert.IsInstanceOf<List<Album>>(albums);
                        Assert.AreEqual(1, albums.Count);
                        Assert.AreEqual(0, albums[0].Id);
                        Assert.AreEqual(1, albums[0].ArtistId);
                        Assert.AreEqual(2, albums[0].GenreId);
                        Assert.AreEqual(1999, albums[0].Year);
                        Assert.AreEqual("string", albums[0].Name);
                    }
                }
            }
        }

        [Test]
        public void MakeItemTest()
        {
            using (var dataAccess = Substitute.For<IDataAccess>())
            {
                using (var cursor = Substitute.For<ICursor>())
                {
                    cursor.GetInt(0).Returns(0);
                    cursor.GetInt(1).Returns(1);
                    cursor.GetInt(2).Returns(2);
                    cursor.GetInt(3).Returns(1999);
                    cursor.GetString(4).Returns("string");

                    var map = new Dictionary<string, int>
                              {
                                  [AlbumTable.IdColumnName] = 0,
                                  [AlbumTable.ArtistIdColumnName] = 1,
                                  [AlbumTable.GenreIdColumnName] = 2,
                                  [AlbumTable.YearColumnName] = 3,
                                  [AlbumTable.NameColumnName] = 4
                              };

                    using (var albumController = new AlbumControllerTestClass(dataAccess))
                    {
                        var album = albumController.MakeItem(cursor, map);
                        Assert.IsNotNull(album);
                        Assert.IsInstanceOf<Album>(album);
                        Assert.AreEqual(0, album.Id);
                        Assert.AreEqual(1, album.ArtistId);
                        Assert.AreEqual(2, album.GenreId);
                        Assert.AreEqual(1999, album.Year);
                        Assert.AreEqual("string", album.Name);
                    }
                }
            }
        }

        [Test]
        public void PostAsyncExceptionTest()
        {
            using (var dataAccess = Substitute.For<IDataAccess>())
            {
                dataAccess.ExecuteSqlWithParametersAsync(Arg.Any<string>(), Arg.Any<List<SQLiteParameter>>()).Returns(0);

                using (var albumController = new AlbumController(dataAccess))
                {
                    Assert.That(async () => await albumController.PostAsync(new Album(0, 1, 2, "name", 1999)), Throws.InstanceOf<SQLiteException>());
                }
            }
        }

        [Test]
        public void PostAsyncTest()
        {
            using (var dataAccess = Substitute.For<IDataAccess>())
            {
                dataAccess.ExecuteSqlWithParametersAsync(Arg.Any<string>(), Arg.Any<List<SQLiteParameter>>()).Returns(1);

                using (var albumController = new AlbumController(dataAccess))
                {
                    albumController.PostAsync(new Album(0, 1, 2, "name", 1999)).Wait();
                }
            }
        }

        [Test]
        public void PutAsyncExceptionTest()
        {
            using (var dataAccess = Substitute.For<IDataAccess>())
            {
                dataAccess.ExecuteSqlWithParametersAsync(Arg.Any<string>(), Arg.Any<List<SQLiteParameter>>()).Returns(0);

                using (var albumController = new AlbumController(dataAccess))
                {
                    Assert.That(async () => await albumController.PutAsync(0, new Album(0, 1, 2, "name", 1999)), Throws.InstanceOf<SQLiteException>());
                }
            }
        }

        [Test]
        public void PutAsyncTest()
        {
            using (var dataAccess = Substitute.For<IDataAccess>())
            {
                dataAccess.ExecuteSqlWithParametersAsync(Arg.Any<string>(), Arg.Any<List<SQLiteParameter>>()).Returns(1);

                using (var albumController = new AlbumController(dataAccess))
                {
                    albumController.PutAsync(0, new Album(0, 1, 2, "name", 1999)).Wait();
                }
            }
        }
    }
}