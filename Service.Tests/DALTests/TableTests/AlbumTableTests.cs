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
    public class AlbumTableTests
    {
        [Test]
        public void ConstructorTest()
        {
            var boolNullable = new AlbumTable();
            Assert.AreEqual("albums", TableFactory<Album>.GetTable<AlbumTable>().TableName);
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
            var table = new AlbumTable();
            var expected = $"SELECT COUNT(*) FROM {TableFactory<Album>.GetTable<AlbumTable>().TableName} WHERE {AlbumTable.IdColumnName} = @{AlbumTable.IdColumnName};";

            Assert.AreEqual(expected, table.GetCountSql(value, ref parameters));
            Assert.AreEqual(1, parameters.Count);

            parameters.Clear();

            value = new IdentifyingInfo
                    {
                        Id = id,
                        Name = StringExtensions.GetRandomStringAsync(25).Result
                    };

            expected = $"SELECT COUNT(*) FROM {TableFactory<Album>.GetTable<AlbumTable>().TableName} WHERE {AlbumTable.IdColumnName} = @{AlbumTable.IdColumnName} AND {AlbumTable.NameColumnName} = @{AlbumTable.NameColumnName};";

            Assert.AreEqual(expected, table.GetCountSql(value, ref parameters));
            Assert.AreEqual(2, parameters.Count);
        }

        [Test]
        public void GetCreateSqlTest()
        {
            var table = new AlbumTable();
            var expected = $"CREATE TABLE {TableFactory<Album>.GetTable<AlbumTable>().TableName} ({AlbumTable.IdColumnName} INTEGER NOT NULL PRIMARY KEY, {AlbumTable.NameColumnName} TEXT UNIQUE NOT NULL COLLATE NOCASE, {AlbumTable.YearColumnName} INTEGER NOT NULL, {AlbumTable.ArtistIdColumnName} INTEGER NOT NULL, {AlbumTable.GenreIdColumnName} INTEGER NOT NULL, FOREIGN KEY({AlbumTable.ArtistIdColumnName}) REFERENCES {TableFactory<Artist>.GetTable<ArtistTable>().TableName}({ArtistTable.IdColumnName}), FOREIGN KEY({AlbumTable.GenreIdColumnName}) REFERENCES {TableFactory<Genre>.GetTable<GenreTable>().TableName}({GenreTable.IdColumnName})) WITHOUT ROWID;";

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
            var table = new AlbumTable();

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
            var table = new AlbumTable();
            var expected = $"DELETE FROM {TableFactory<Album>.GetTable<AlbumTable>().TableName} WHERE {AlbumTable.IdColumnName} = @{AlbumTable.IdColumnName};";

            Assert.AreEqual(expected, table.GetDeleteSql(value, ref parameters));
            Assert.AreEqual(1, parameters.Count);
            parameters.Clear();

            value = new IdentifyingInfo
                    {
                        Name = StringExtensions.GetRandomStringAsync(25).Result
                    };

            expected = $"DELETE FROM {TableFactory<Album>.GetTable<AlbumTable>().TableName} WHERE {AlbumTable.NameColumnName} = @{AlbumTable.NameColumnName};";

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
            var table = new AlbumTable();
            var expected = $"SELECT * FROM {TableFactory<Album>.GetTable<AlbumTable>().TableName} WHERE {AlbumTable.IdColumnName} = @{AlbumTable.IdColumnName};";

            Assert.AreEqual(expected, table.GetFetchSql(value, ref parameters));
            Assert.AreEqual(1, parameters.Count);
            parameters.Clear();

            value = new IdentifyingInfo
                    {
                        Name = StringExtensions.GetRandomStringAsync(25).Result
                    };

            expected = $"SELECT * FROM {TableFactory<Album>.GetTable<AlbumTable>().TableName} WHERE {AlbumTable.NameColumnName} = @{AlbumTable.NameColumnName};";

            Assert.AreEqual(expected, table.GetFetchSql(value, ref parameters));
            Assert.AreEqual(1, parameters.Count);

            parameters.Clear();
        }

        [Test]
        public void GetInsertSqlTest()
        {
            var id = HelperObjectFactory.GetRandomInt(0, 100);
            var value = new Album(id, id, id, StringExtensions.GetRandomStringAsync(23).Result, 2002);
            var parameters = new List<SQLiteParameter>();
            var table = new AlbumTable();
            var expected = $"INSERT OR IGNORE INTO {TableFactory<Album>.GetTable<AlbumTable>().TableName} ({AlbumTable.IdColumnName}, {AlbumTable.ArtistIdColumnName}, {AlbumTable.GenreIdColumnName}, {AlbumTable.NameColumnName}, {AlbumTable.YearColumnName}) VALUES(@{AlbumTable.IdColumnName}, @{AlbumTable.ArtistIdColumnName}, @{AlbumTable.GenreIdColumnName}, @{AlbumTable.NameColumnName}, @{AlbumTable.YearColumnName});";

            Assert.AreEqual(expected, table.GetInsertSql(value, ref parameters));
            Assert.AreEqual(5, parameters.Count);
        }

        [Test]
        public void GetUpdateSqlTest()
        {
            var id = HelperObjectFactory.GetRandomInt(0, 100);
            var value = new Album(id, id, id, StringExtensions.GetRandomStringAsync(23).Result, 2002);
            var parameters = new List<SQLiteParameter>();
            var table = new AlbumTable();
            var expected = $"UPDATE {TableFactory<Album>.GetTable<AlbumTable>().TableName} SET {AlbumTable.ArtistIdColumnName} = @{AlbumTable.ArtistIdColumnName}, {AlbumTable.GenreIdColumnName} = @{AlbumTable.GenreIdColumnName}, {AlbumTable.NameColumnName} = @{AlbumTable.NameColumnName}, {AlbumTable.YearColumnName} = @{AlbumTable.YearColumnName} WHERE {AlbumTable.IdColumnName} = @{AlbumTable.IdColumnName};";

            Assert.AreEqual(expected, table.GetUpdateSql(value, ref parameters));
            Assert.AreEqual(5, parameters.Count);
        }
    }
}