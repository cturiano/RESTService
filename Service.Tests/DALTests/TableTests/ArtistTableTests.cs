using System;
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
    public class ArtistTableTests
    {
        [Test]
        public void ConstructorTest()
        {
            var boolNullable = new ArtistTable();
            Assert.AreEqual("artists", TableFactory<Artist>.GetTable<ArtistTable>().TableName);
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
            var table = new ArtistTable();
            var expected = $"SELECT COUNT(*) FROM {TableFactory<Artist>.GetTable<ArtistTable>().TableName} WHERE {ArtistTable.IdColumnName} = @{ArtistTable.IdColumnName};";

            Assert.AreEqual(expected, table.GetCountSql(value, ref parameters));
            Assert.AreEqual(1, parameters.Count);
        }

        [Test]
        public void GetCreateSqlTest()
        {
            var table = new ArtistTable();
            var expected = $"CREATE TABLE {TableFactory<Artist>.GetTable<ArtistTable>().TableName} ({ArtistTable.IdColumnName} INTEGER NOT NULL PRIMARY KEY, {ArtistTable.NameColumnName} TEXT UNIQUE NOT NULL COLLATE NOCASE) WITHOUT ROWID;";

            Assert.AreEqual(expected, table.GetCreateSql());
        }

        [Test]
        public void GetDeleteAlbumsByArtistSqlExceptionTest()
        {
            var value = new IdentifyingInfo
                        {
                            Id = null
                        };

            var parameters = new List<SQLiteParameter>();
            var table = new ArtistTable();

            Assert.That(() => table.GetDeleteAlbumsByArtistSql(value, ref parameters), Throws.ArgumentException);
        }

        [Test]
        public void GetDeleteAlbumsByArtistSqlTest()
        {
            var id = HelperObjectFactory.GetRandomInt(0, 100);
            var value = new IdentifyingInfo
                        {
                            Id = id
                        };

            var parameters = new List<SQLiteParameter>();
            var table = new ArtistTable();
            var expected = $"DELETE FROM {TableFactory<Album>.GetTable<AlbumTable>().TableName} WHERE {AlbumTable.ArtistIdColumnName} = @{AlbumTable.ArtistIdColumnName};";

            Assert.AreEqual(expected, table.GetDeleteAlbumsByArtistSql(value, ref parameters));
            Assert.AreEqual(1, parameters.Count);
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
            var table = new ArtistTable();
            var expected = $"DELETE FROM {TableFactory<Artist>.GetTable<ArtistTable>().TableName} WHERE {ArtistTable.IdColumnName} = @{ArtistTable.IdColumnName};";

            Assert.AreEqual(expected, table.GetDeleteSql(value, ref parameters));
            Assert.AreEqual(1, parameters.Count);
            parameters.Clear();

            value = new IdentifyingInfo
                    {
                        Name = StringExtensions.GetRandomStringAsync(25).Result
                    };

            expected = $"DELETE FROM {TableFactory<Artist>.GetTable<ArtistTable>().TableName} WHERE {ArtistTable.NameColumnName} = @{ArtistTable.NameColumnName};";

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
            var table = new ArtistTable();
            var expected = $"SELECT * FROM {TableFactory<Artist>.GetTable<ArtistTable>().TableName} WHERE {ArtistTable.IdColumnName} = @{ArtistTable.IdColumnName};";

            Assert.AreEqual(expected, table.GetFetchSql(value, ref parameters));
            Assert.AreEqual(1, parameters.Count);
            parameters.Clear();

            value = new IdentifyingInfo
                    {
                        Name = StringExtensions.GetRandomStringAsync(25).Result
                    };

            expected = $"SELECT * FROM {TableFactory<Artist>.GetTable<ArtistTable>().TableName} WHERE {ArtistTable.NameColumnName} = @{ArtistTable.NameColumnName};";

            Assert.AreEqual(expected, table.GetFetchSql(value, ref parameters));
            Assert.AreEqual(1, parameters.Count);
        }
        
        [Test]
        public void GetAlbumsByArtistSqlExceptionTest()
        {
            var value = new IdentifyingInfo
                        {
                            Id = null
                        };

            var parameters = new List<SQLiteParameter>();
            var table = new ArtistTable();

            Assert.Throws<ArgumentException>(() =>table.GetAlbumsByArtistSql(value, ref parameters));
        }

        [Test]
        public void GetIdFromNameExceptionTest()
        {
            var id = HelperObjectFactory.GetRandomInt(0, 100);
            var value = new IdentifyingInfo
                        {
                            Id = id
                        };

            var parameters = new List<SQLiteParameter>();
            var table = new ArtistTable();

            Assert.That(() => table.GetIdFromNameSql(value, ref parameters), Throws.ArgumentException);
        }

        [Test]
        public void GetIdFromNameTest()
        {
            var value = new IdentifyingInfo
                        {
                            Name = StringExtensions.GetRandomStringAsync(25).Result
                        };

            var parameters = new List<SQLiteParameter>();
            var table = new ArtistTable();
            var expected = $"SELECT {ArtistTable.IdColumnName} FROM {table.TableName} WHERE {ArtistTable.NameColumnName} = @{ArtistTable.NameColumnName};";

            Assert.AreEqual(expected, table.GetIdFromNameSql(value, ref parameters));
            Assert.AreEqual(1, parameters.Count);
        }

        [Test]
        public void GetInsertSqlTest()
        {
            var id = HelperObjectFactory.GetRandomInt(0, 100);
            var value = new Artist(id, StringExtensions.GetRandomStringAsync(23).Result);
            var parameters = new List<SQLiteParameter>();
            var table = new ArtistTable();
            var expected = $"INSERT OR IGNORE INTO {TableFactory<Artist>.GetTable<ArtistTable>().TableName} ({ArtistTable.IdColumnName}, {AlbumTable.NameColumnName}) VALUES(@{ArtistTable.IdColumnName}, @{ArtistTable.NameColumnName});";

            Assert.AreEqual(expected, table.GetInsertSql(value, ref parameters));
            Assert.AreEqual(2, parameters.Count);
        }

        [Test]
        public void GetUpdateSqlTest()
        {
            var id = HelperObjectFactory.GetRandomInt(0, 100);
            var value = new Artist(id, StringExtensions.GetRandomStringAsync(23).Result);
            var parameters = new List<SQLiteParameter>();
            var table = new ArtistTable();
            var expected = $"UPDATE {TableFactory<Artist>.GetTable<ArtistTable>().TableName} SET {ArtistTable.NameColumnName} = @{ArtistTable.NameColumnName} WHERE {ArtistTable.IdColumnName} = @{ArtistTable.IdColumnName};";

            Assert.AreEqual(expected, table.GetUpdateSql(value, ref parameters));
            Assert.AreEqual(2, parameters.Count);
        }
    }
}