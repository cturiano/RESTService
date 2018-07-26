using System.Collections.Generic;
using System.Data.SQLite;
using NUnit.Framework;
using Service.DAL;
using Service.DAL.Tables;
using Service.Extensions;
using Service.Models;

namespace Service.Tests.DALTests.TableTests
{
    [TestFixture]
    public class GenreTableTests
    {
        [Test]
        public void ConstructorTest()
        {
            var boolNullable = new GenreTable();
            Assert.AreEqual("genres", TableFactory<Genre>.GetTable<GenreTable>().TableName);
            Assert.IsNotNull(boolNullable);
        }

        [Test]
        public void GetCountSqlTest()
        {
            var id = HelperObjectFactory.GetRandomInt(0, 100);
            var value = new IdentifyingInfo
                        {
                            Id = id
                        };

            var parameters = new List<SQLiteParameter>();
            var table = new GenreTable();
            var expected = $"SELECT COUNT(*) FROM {TableFactory<Genre>.GetTable<GenreTable>().TableName} WHERE {GenreTable.IdColumnName} = @{GenreTable.IdColumnName};";

            Assert.AreEqual(expected, table.GetCountSql(value, ref parameters));
            Assert.AreEqual(1, parameters.Count);
        }

        [Test]
        public void GetCreateSqlTest()
        {
            var table = new GenreTable();
            var expected = $"CREATE TABLE {TableFactory<Genre>.GetTable<GenreTable>().TableName} ({GenreTable.IdColumnName} INTEGER NOT NULL PRIMARY KEY, {GenreTable.NameColumnName} TEXT UNIQUE NOT NULL COLLATE NOCASE) WITHOUT ROWID;";

            Assert.AreEqual(expected, table.GetCreateSql());
        }

        [Test]
        public void GetDeleteSqlExceptionTest()
        {
            var value = new IdentifyingInfo
                        {
                            Id = null,
                            Name = null
                        };

            var parameters = new List<SQLiteParameter>();
            var table = new GenreTable();

            Assert.That(() => table.GetDeleteSql(value, ref parameters), Throws.ArgumentException);
        }

        [Test]
        public void GetDeleteSqlTest()
        {
            var id = HelperObjectFactory.GetRandomInt(0, 100);
            var value = new IdentifyingInfo
                        {
                            Id = id
                        };

            var parameters = new List<SQLiteParameter>();
            var table = new GenreTable();
            var expected = $"DELETE FROM {TableFactory<Genre>.GetTable<GenreTable>().TableName} WHERE {GenreTable.IdColumnName} = @{GenreTable.IdColumnName};";

            Assert.AreEqual(expected, table.GetDeleteSql(value, ref parameters));
            Assert.AreEqual(1, parameters.Count);
            parameters.Clear();

            value = new IdentifyingInfo
                    {
                        Name = StringExtensions.GetRandomStringAsync(25).Result
                    };

            expected = $"DELETE FROM {TableFactory<Genre>.GetTable<GenreTable>().TableName} WHERE {GenreTable.NameColumnName} = @{GenreTable.NameColumnName};";

            Assert.AreEqual(expected, table.GetDeleteSql(value, ref parameters));
            Assert.AreEqual(1, parameters.Count);
        }

        [Test]
        public void GetFetchSqlTest()
        {
            var id = HelperObjectFactory.GetRandomInt(0, 100);
            var value = new IdentifyingInfo
                        {
                            Id = id
                        };

            var parameters = new List<SQLiteParameter>();
            var table = new GenreTable();
            var expected = $"SELECT * FROM {TableFactory<Genre>.GetTable<GenreTable>().TableName} WHERE {GenreTable.IdColumnName} = @{GenreTable.IdColumnName};";

            Assert.AreEqual(expected, table.GetFetchSql(value, ref parameters));
            Assert.AreEqual(1, parameters.Count);
            parameters.Clear();

            value = new IdentifyingInfo
                    {
                        Name = StringExtensions.GetRandomStringAsync(25).Result
                    };

            expected = $"SELECT * FROM {TableFactory<Genre>.GetTable<GenreTable>().TableName} WHERE {GenreTable.NameColumnName} = @{GenreTable.NameColumnName};";

            Assert.AreEqual(expected, table.GetFetchSql(value, ref parameters));
            Assert.AreEqual(1, parameters.Count);
        }

        [Test]
        public void GetInsertSqlTest()
        {
            var id = HelperObjectFactory.GetRandomInt(0, 100);
            var value = new Genre(id, StringExtensions.GetRandomStringAsync(23).Result);
            var parameters = new List<SQLiteParameter>();
            var table = new GenreTable();
            var expected = $"INSERT OR IGNORE INTO {TableFactory<Genre>.GetTable<GenreTable>().TableName} ({GenreTable.IdColumnName}, {AlbumTable.NameColumnName}) VALUES(@{GenreTable.IdColumnName}, @{GenreTable.NameColumnName});";

            Assert.AreEqual(expected, table.GetInsertSql(value, ref parameters));
            Assert.AreEqual(2, parameters.Count);
        }

        [Test]
        public void GetUpdateSqlTest()
        {
            var id = HelperObjectFactory.GetRandomInt(0, 100);
            var value = new Genre(id, StringExtensions.GetRandomStringAsync(23).Result);
            var parameters = new List<SQLiteParameter>();
            var table = new GenreTable();
            var expected = $"UPDATE {TableFactory<Genre>.GetTable<GenreTable>().TableName} SET {GenreTable.NameColumnName} = @{GenreTable.NameColumnName} WHERE {GenreTable.IdColumnName} = @{GenreTable.IdColumnName};";

            Assert.AreEqual(expected, table.GetUpdateSql(value, ref parameters));
            Assert.AreEqual(2, parameters.Count);
        }
    }
}