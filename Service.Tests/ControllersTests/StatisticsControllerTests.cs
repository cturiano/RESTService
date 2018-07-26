using System.Collections.Generic;
using System.Data.SQLite;
using NSubstitute;
using NUnit.Framework;
using Service.Controllers;
using Service.Interfaces;

namespace Service.Tests.ControllersTests
{
    [TestFixture]
    public class StatisticsControllerTests
    {
        [Test]
        public void ConstructorAndPropertiesTest()
        {
            using (var dataAccess = Substitute.For<IDataAccess>())
            {
                using (var statisticsController = new StatisticsController(dataAccess))
                {
                    Assert.IsNotNull(statisticsController);
                    Assert.IsInstanceOf<StatisticsController>(statisticsController);
                }
            }
        }

        [Test]
        public void DisposeTest()
        {
            using (var dataAccess = Substitute.For<IDataAccess>())
            {
                using (var statisticsController = new StatisticsController(dataAccess))
                {
                    statisticsController.Dispose();
                }
            }
        }

        [Test]
        public void GetGenreTest()
        {
            using (var cursor = Substitute.For<ICursor>())
            {
                cursor.GetString(0).Returns("genre");
                cursor.GetInt(1).Returns(7);
                cursor.GetColumnIndex("name").Returns(0);
                cursor.GetColumnIndex("count").Returns(1);

                using (var dataAccess = Substitute.For<IDataAccess>())
                {
                    dataAccess.ExecuteQueryWithParametersAsync(Arg.Any<string>(), Arg.Any<List<SQLiteParameter>>()).Returns(cursor);

                    using (var statisticsController = new StatisticsController(dataAccess))
                    {
                        var countAlbumsPerGenre = statisticsController.GetCountOfAlbumsPerGenreAsync().Result;
                        Assert.IsNotNull(countAlbumsPerGenre);
                        Assert.IsInstanceOf<Dictionary<string, int>>(countAlbumsPerGenre);
                        Assert.AreEqual(1, countAlbumsPerGenre.Count);
                        Assert.IsTrue(countAlbumsPerGenre.ContainsKey("genre"));
                        Assert.AreEqual(7, countAlbumsPerGenre["genre"]);
                    }
                }
            }
        }

        [Test]
        public void GetYearTest()
        {
            using (var cursor = Substitute.For<ICursor>())
            {
                cursor.GetInt(0).Returns(1999);
                cursor.GetInt(1).Returns(7);
                cursor.GetColumnIndex("year").Returns(0);
                cursor.GetColumnIndex("count").Returns(1);

                using (var dataAccess = Substitute.For<IDataAccess>())
                {
                    dataAccess.ExecuteQueryWithParametersAsync(Arg.Any<string>(), Arg.Any<List<SQLiteParameter>>()).Returns(cursor);

                    using (var statisticsController = new StatisticsController(dataAccess))
                    {
                        var countAlbumsPerYear = statisticsController.GetCountofAlbumsPerYearAsync().Result;
                        Assert.IsNotNull(countAlbumsPerYear);
                        Assert.IsInstanceOf<Dictionary<int, int>>(countAlbumsPerYear);
                        Assert.AreEqual(1, countAlbumsPerYear.Count);
                        Assert.IsTrue(countAlbumsPerYear.ContainsKey(1999));
                        Assert.AreEqual(7, countAlbumsPerYear[1999]);
                    }
                }
            }
        }
    }
}