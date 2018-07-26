using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Service.Abstract;
using Service.Controllers;
using Service.DAL;
using Service.DAL.Tables;
using Service.Interfaces;
using Service.Models;

namespace Service.Tests.ControllersTests
{
    [TestFixture]
    public class ArtistControllerTest
    {
        private class ArtistControllerTestClass : ArtistController
        {
            #region Constructors

            public ArtistControllerTestClass(IDataAccess dataAccess) : base(dataAccess)
            {
            }

            #endregion

            #region Properties

            public AbstractTable<Artist> TestTable => Table;

            #endregion

            #region Public Methods

            public Artist MakeItem(ICursor cursor, Dictionary<string, int> indexMap) => MakeItemAsync(cursor, indexMap).Result;

            #endregion
        }

        [Test]
        public void ConstructorAndPropertiesTest()
        {
            using (var dataAccess = Substitute.For<IDataAccess>())
            {
                using (var artistController = new ArtistControllerTestClass(dataAccess))
                {
                    Assert.IsNotNull(artistController);
                    Assert.IsInstanceOf<ArtistController>(artistController);
                    Assert.IsInstanceOf<AbstractController<Artist>>(artistController);
                    Assert.IsNotNull(artistController.TestTable);
                    Assert.IsInstanceOf<AbstractTable<Artist>>(artistController.TestTable);
                }
            }
        }

        [Test]
        public void DeleteAsyncIntTest()
        {
            using (var dataAccess = Substitute.For<IDataAccess>())
            {
                dataAccess.ExecuteSqlWithParametersAsync(Arg.Any<string>(), Arg.Any<List<SQLiteParameter>>()).Returns(1);

                using (var artistController = new ArtistController(dataAccess))
                {
                    var t = artistController.DeleteAsync(1);
                    Assert.IsInstanceOf<Task>(t);
                }
            }
        }

        [Test]
        public void DeleteAsyncStringExceptionTest()
        {
            using (var dataAccess = Substitute.For<IDataAccess>())
            {
                dataAccess.ExecuteQueryWithParametersAsync(Arg.Any<string>(), Arg.Any<List<SQLiteParameter>>()).Returns((ICursor)null);
                dataAccess.ExecuteSqlWithParametersAsync(Arg.Any<string>(), Arg.Any<List<SQLiteParameter>>()).Returns(1);

                using (var artistController = new ArtistController(dataAccess))
                {
                    Assert.That(() => artistController.DeleteAsync("string"), Throws.InstanceOf<SQLiteException>());
                }
            }
        }

        [Test]
        public void DeleteAsyncStringTest()
        {
            using (var cursor = Substitute.For<ICursor>())
            {
                cursor.GetInt(0).Returns(0);
                cursor.GetColumnIndex("id").Returns(0);
                {
                    using (var dataAccess = Substitute.For<IDataAccess>())
                    {
                        dataAccess.ExecuteQueryWithParametersAsync(Arg.Any<string>(), Arg.Any<List<SQLiteParameter>>()).Returns(cursor);
                        dataAccess.ExecuteSqlWithParametersAsync(Arg.Any<string>(), Arg.Any<List<SQLiteParameter>>()).Returns(1);

                        using (var artistController = new ArtistController(dataAccess))
                        {
                            var t = artistController.DeleteAsync("string");
                            Assert.IsInstanceOf<Task>(t);
                        }
                    }
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
                using (var artistController = new ArtistController(dataAccess))
                {
                    artistController.Dispose();
                    GC.ReRegisterForFinalize(artistController);
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
                cursor.GetString(1).Returns("string");
                cursor.GetColumnIndex("id").Returns(0);
                cursor.GetColumnIndex("name").Returns(1);

                using (var dataAccess = Substitute.For<IDataAccess>())
                {
                    dataAccess.ExecuteQueryWithParametersAsync(Arg.Any<string>(), Arg.Any<List<SQLiteParameter>>()).Returns(cursor);

                    using (var artistController = new ArtistController(dataAccess))
                    {
                        var artist = artistController.GetAsync(1).Result;
                        Assert.IsNotNull(artist);
                        Assert.IsInstanceOf<Artist>(artist);
                        Assert.AreEqual(0, artist.Id);
                        Assert.AreEqual("string", artist.Name);
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
                cursor.GetString(1).Returns("string");
                cursor.GetColumnIndex("id").Returns(0);
                cursor.GetColumnIndex("name").Returns(1);

                using (var dataAccess = Substitute.For<IDataAccess>())
                {
                    dataAccess.ExecuteQueryWithParametersAsync(Arg.Any<string>(), Arg.Any<List<SQLiteParameter>>()).Returns(cursor);

                    using (var artistController = new ArtistController(dataAccess))
                    {
                        var artist = artistController.GetAsync("string").Result;
                        Assert.IsNotNull(artist);
                        Assert.IsInstanceOf<Artist>(artist);
                        Assert.AreEqual(0, artist.Id);
                        Assert.AreEqual("string", artist.Name);
                    }
                }
            }
        }

        [Test]
        public void GetAlbumsAsyncTest()
        {
            using (var cursor = Substitute.For<ICursor>())
            {
                cursor.GetInt(0).Returns(13);
                cursor.GetString(1).Returns("string");
                cursor.GetInt(2).Returns(13);
                cursor.GetInt(3).Returns(7);
                cursor.GetInt(4).Returns(1999);
                cursor.GetColumnIndex("id").Returns(0);
                cursor.GetColumnIndex("name").Returns(1);
                cursor.GetColumnIndex("artist_id").Returns(2);
                cursor.GetColumnIndex("genre_id").Returns(3);
                cursor.GetColumnIndex("year").Returns(4);

                using (var dataAccess = Substitute.For<IDataAccess>())
                {
                    dataAccess.ExecuteQueryWithParametersAsync(Arg.Any<string>(), Arg.Any<List<SQLiteParameter>>()).Returns(cursor);

                    using (var artistController = new ArtistController(dataAccess))
                    {
                        var albums = artistController.GetAlbumsAsync("string").Result;
                        Assert.IsNotNull(albums);
                        Assert.IsInstanceOf<List<Album>>(albums);
                        Assert.AreEqual(1, albums.Count);
                        Assert.AreEqual(13, albums[0].Id);
                        Assert.AreEqual("string", albums[0].Name);
                        Assert.AreEqual(13, albums[0].ArtistId);
                        Assert.AreEqual(7, albums[0].GenreId);
                        Assert.AreEqual(1999, albums[0].Year);
                    }
                }
            }
        }

        [Test]
        public void GetAlbumsAsyncCursorNullTest()
        {
            using (var dataAccess = Substitute.For<IDataAccess>())
            {
                dataAccess.ExecuteQueryWithParametersAsync(Arg.Any<string>(), Arg.Any<List<SQLiteParameter>>()).Returns(a => (Cursor)null);

                using (var artistController = new ArtistController(dataAccess))
                {
                    Assert.ThrowsAsync<SQLiteException>(() => artistController.GetAlbumsAsync("string"));
                }
            }
        }
        
        [Test]
        public void GetAsyncTest()
        {
            using (var cursor = Substitute.For<ICursor>())
            {
                cursor.GetInt(0).Returns(0);
                cursor.GetString(1).Returns("string");
                cursor.GetColumnIndex("id").Returns(0);
                cursor.GetColumnIndex("name").Returns(1);

                using (var dataAccess = Substitute.For<IDataAccess>())
                {
                    dataAccess.ExecuteQueryWithParametersAsync(Arg.Any<string>(), Arg.Any<List<SQLiteParameter>>()).Returns(cursor);

                    using (var artistController = new ArtistController(dataAccess))
                    {
                        var artists = artistController.GetAsync().Result.ToList();
                        Assert.IsNotNull(artists);
                        Assert.IsInstanceOf<List<Artist>>(artists);
                        Assert.AreEqual(1, artists.Count);
                        Assert.AreEqual(0, artists[0].Id);
                        Assert.AreEqual("string", artists[0].Name);
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
                    cursor.GetString(1).Returns("string");

                    var map = new Dictionary<string, int>
                              {
                                  [ArtistTable.IdColumnName] = 0,
                                  [ArtistTable.NameColumnName] = 1
                              };

                    using (var artistController = new ArtistControllerTestClass(dataAccess))
                    {
                        var artist = artistController.MakeItem(cursor, map);
                        Assert.IsNotNull(artist);
                        Assert.IsInstanceOf<Artist>(artist);
                        Assert.AreEqual(0, artist.Id);
                        Assert.AreEqual("string", artist.Name);
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

                using (var artistController = new ArtistController(dataAccess))
                {
                    Assert.That(async () => await artistController.PostAsync(new Artist(0, "name")), Throws.InstanceOf<SQLiteException>());
                }
            }
        }

        [Test]
        public void PostAsyncTest()
        {
            using (var dataAccess = Substitute.For<IDataAccess>())
            {
                dataAccess.ExecuteSqlWithParametersAsync(Arg.Any<string>(), Arg.Any<List<SQLiteParameter>>()).Returns(1);

                using (var artistController = new ArtistController(dataAccess))
                {
                    artistController.PostAsync(new Artist(0, "name")).Wait();
                }
            }
        }

        [Test]
        public void PutAsyncExceptionTest()
        {
            using (var dataAccess = Substitute.For<IDataAccess>())
            {
                dataAccess.ExecuteSqlWithParametersAsync(Arg.Any<string>(), Arg.Any<List<SQLiteParameter>>()).Returns(0);

                using (var artistController = new ArtistController(dataAccess))
                {
                    Assert.That(async () => await artistController.PutAsync(0, new Artist(0, "name")), Throws.InstanceOf<SQLiteException>());
                }
            }
        }

        [Test]
        public void PutAsyncTest()
        {
            using (var dataAccess = Substitute.For<IDataAccess>())
            {
                dataAccess.ExecuteSqlWithParametersAsync(Arg.Any<string>(), Arg.Any<List<SQLiteParameter>>()).Returns(1);

                using (var artistController = new ArtistController(dataAccess))
                {
                    artistController.PutAsync(0, new Artist(0, "name")).Wait();
                }
            }
        }
    }
}