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
    public class GenreControllerTest
    {
        private class GenreControllerTestClass : GenreController
        {
            #region Constructors

            public GenreControllerTestClass(IDataAccess dataAccess) : base(dataAccess)
            {
            }

            #endregion

            #region Properties

            public AbstractTable<Genre> TestTable => Table;

            #endregion

            #region Public Methods

            public Genre MakeItem(ICursor cursor, Dictionary<string, int> indexMap) => MakeItemAsync(cursor, indexMap).Result;

            #endregion
        }

        [Test]
        public void ConstructorAndPropertiesTest()
        {
            using (var dataAccess = Substitute.For<IDataAccess>())
            {
                using (var genreController = new GenreControllerTestClass(dataAccess))
                {
                    Assert.IsNotNull(genreController);
                    Assert.IsInstanceOf<GenreController>(genreController);
                    Assert.IsInstanceOf<AbstractController<Genre>>(genreController);
                    Assert.IsNotNull(genreController.TestTable);
                    Assert.IsInstanceOf<AbstractTable<Genre>>(genreController.TestTable);
                }
            }
        }

        [Test]
        public void DeleteAsyncIntTest()
        {
            using (var dataAccess = Substitute.For<IDataAccess>())
            {
                dataAccess.ExecuteSqlWithParametersAsync(Arg.Any<string>(), Arg.Any<List<SQLiteParameter>>()).Returns(1);

                using (var genreController = new GenreController(dataAccess))
                {
                    var t = genreController.DeleteAsync(1);
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

                using (var genreController = new GenreController(dataAccess))
                {
                    var t = genreController.DeleteAsync("string");
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
                using (var genreController = new GenreController(dataAccess))
                {
                    genreController.Dispose();
                    GC.ReRegisterForFinalize(genreController);
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

                    using (var genreController = new GenreController(dataAccess))
                    {
                        var genre = genreController.GetAsync(1).Result;
                        Assert.IsNotNull(genre);
                        Assert.IsInstanceOf<Genre>(genre);
                        Assert.AreEqual(0, genre.Id);
                        Assert.AreEqual("string", genre.Name);
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

                    using (var genreController = new GenreController(dataAccess))
                    {
                        var genre = genreController.GetAsync("string").Result;
                        Assert.IsNotNull(genre);
                        Assert.IsInstanceOf<Genre>(genre);
                        Assert.AreEqual(0, genre.Id);
                        Assert.AreEqual("string", genre.Name);
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
                cursor.GetString(1).Returns("string");
                cursor.GetColumnIndex("id").Returns(0);
                cursor.GetColumnIndex("name").Returns(1);

                using (var dataAccess = Substitute.For<IDataAccess>())
                {
                    dataAccess.ExecuteQueryWithParametersAsync(Arg.Any<string>(), Arg.Any<List<SQLiteParameter>>()).Returns(cursor);

                    using (var genreController = new GenreController(dataAccess))
                    {
                        var genres = genreController.GetAsync().Result.ToList();
                        Assert.IsNotNull(genres);
                        Assert.IsInstanceOf<List<Genre>>(genres);
                        Assert.AreEqual(1, genres.Count);
                        Assert.AreEqual(0, genres[0].Id);
                        Assert.AreEqual("string", genres[0].Name);
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
                                  [GenreTable.IdColumnName] = 0,
                                  [GenreTable.NameColumnName] = 1
                              };

                    using (var genreController = new GenreControllerTestClass(dataAccess))
                    {
                        var genre = genreController.MakeItem(cursor, map);
                        Assert.IsNotNull(genre);
                        Assert.IsInstanceOf<Genre>(genre);
                        Assert.AreEqual(0, genre.Id);
                        Assert.AreEqual("string", genre.Name);
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

                using (var genreController = new GenreController(dataAccess))
                {
                    Assert.That(async () => await genreController.PostAsync(new Genre(0, "name")), Throws.InstanceOf<SQLiteException>());
                }
            }
        }

        [Test]
        public void PostAsyncTest()
        {
            using (var dataAccess = Substitute.For<IDataAccess>())
            {
                dataAccess.ExecuteSqlWithParametersAsync(Arg.Any<string>(), Arg.Any<List<SQLiteParameter>>()).Returns(1);

                using (var genreController = new GenreController(dataAccess))
                {
                    genreController.PostAsync(new Genre(0, "name")).Wait();
                }
            }
        }

        [Test]
        public void PutAsyncExceptionTest()
        {
            using (var dataAccess = Substitute.For<IDataAccess>())
            {
                dataAccess.ExecuteSqlWithParametersAsync(Arg.Any<string>(), Arg.Any<List<SQLiteParameter>>()).Returns(0);

                using (var genreController = new GenreController(dataAccess))
                {
                    Assert.That(async () => await genreController.PutAsync(0, new Genre(0, "name")), Throws.InstanceOf<SQLiteException>());
                }
            }
        }

        [Test]
        public void PutAsyncTest()
        {
            using (var dataAccess = Substitute.For<IDataAccess>())
            {
                dataAccess.ExecuteSqlWithParametersAsync(Arg.Any<string>(), Arg.Any<List<SQLiteParameter>>()).Returns(1);

                using (var genreController = new GenreController(dataAccess))
                {
                    genreController.PutAsync(0, new Genre(0, "name")).Wait();
                }
            }
        }
    }
}