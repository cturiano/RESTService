using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using NUnit.Framework;
using Service.DAL;
using Service.DAL.Tables;
using Service.Extensions;
using Service.Models;

namespace Service.Tests.DALTests
{
    [TestFixture]
    public class DataAccessTests
    {
        [OneTimeSetUp]
        public void Initialize()
        {
            var dirName = Path.Combine(Path.GetDirectoryName(Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Path)), TestDbLocation);

            // this is also the constructor test
            _dataAccess = new DataAccess(dirName);

            // this is the create db test
            Assert.AreEqual(DatabaseState.New, _dataAccess.OpenOrCreateDatabaseAsync().Result);

            // this is the open db test
            Assert.AreEqual(DatabaseState.Existing, _dataAccess.OpenOrCreateDatabaseAsync().Result);
        }

        private const string TestDbLocation = "DataAccessTests";
        private const string FilterText = "the filter text";
        private static readonly int AlbumId = HelperObjectFactory.GetRandomInt();
        private static readonly int ArtistId = HelperObjectFactory.GetRandomInt();
        private static readonly int GenreId = HelperObjectFactory.GetRandomInt();
        private static readonly int Year = HelperObjectFactory.GetRandomInt();
        private static readonly string AlbumName = StringExtensions.GetRandomStringAsync(15).Result;
        private static readonly string ArtistName = StringExtensions.GetRandomStringAsync(15).Result;
        private static readonly string GenreName = StringExtensions.GetRandomStringAsync(15).Result;

        private DataAccess _dataAccess;

        private void ExecuteCountQueryWithParameters()
        {
            var parameters = new List<SQLiteParameter>();

            // Assert the tables exist
            Assert.AreEqual(1, _dataAccess.ExecuteCountQueryWithParameters($"SELECT COUNT(*) FROM {TableFactory<Album>.GetTable<AlbumTable>().TableName};", parameters));
            Assert.AreEqual(1, _dataAccess.ExecuteCountQueryWithParameters($"SELECT COUNT(*) FROM {TableFactory<Artist>.GetTable<ArtistTable>().TableName};", parameters));
            Assert.AreEqual(1, _dataAccess.ExecuteCountQueryWithParameters($"SELECT COUNT(*) FROM {TableFactory<Genre>.GetTable<GenreTable>().TableName};", parameters));
        }

        private void ExecuteSqlWithParametersTest_Delete()
        {
            var parameters = new List<SQLiteParameter>();

            // Assert the tables exist
            Assert.AreEqual(1,
                            _dataAccess.ExecuteSqlWithParametersAsync(TableFactory<Album>.GetTable<AlbumTable>()
                                                                                         .GetDeleteSql(new IdentifyingInfo
                                                                                                       {
                                                                                                           Id = AlbumId,
                                                                                                           FilterText = FilterText
                                                                                                       },
                                                                                                       ref parameters),
                                                                      parameters)
                                       .Result);

            parameters.Clear();
            Assert.AreEqual(1,
                            _dataAccess.ExecuteSqlWithParametersAsync(TableFactory<Artist>.GetTable<ArtistTable>()
                                                                                          .GetDeleteSql(new IdentifyingInfo
                                                                                                        {
                                                                                                            Id = ArtistId,
                                                                                                            FilterText = FilterText
                                                                                                        },
                                                                                                        ref parameters),
                                                                      parameters)
                                       .Result);

            parameters.Clear();
            Assert.AreEqual(1,
                            _dataAccess.ExecuteSqlWithParametersAsync(TableFactory<Genre>.GetTable<GenreTable>()
                                                                                         .GetDeleteSql(new IdentifyingInfo
                                                                                                       {
                                                                                                           Id = GenreId,
                                                                                                           FilterText = FilterText
                                                                                                       },
                                                                                                       ref parameters),
                                                                      parameters)
                                       .Result);

            parameters.Clear();
        }

        private void ExecuteSqlWithParametersTest_Insert()
        {
            var album = HelperObjectFactory.MakeAlbum(AlbumId, ArtistId, GenreId, AlbumName, Year);
            var artist = HelperObjectFactory.MakeArtist(ArtistId, ArtistName);
            var genre = HelperObjectFactory.MakeGenre(GenreId, GenreName);

            var parameters = new List<SQLiteParameter>();

            // Assert the tables exist
            Assert.AreEqual(1, _dataAccess.ExecuteSqlWithParametersAsync(TableFactory<Genre>.GetTable<GenreTable>().GetInsertSql(genre, ref parameters), parameters).Result);
            parameters.Clear();
            Assert.AreEqual(1, _dataAccess.ExecuteSqlWithParametersAsync(TableFactory<Album>.GetTable<AlbumTable>().GetInsertSql(album, ref parameters), parameters).Result);
            parameters.Clear();
            Assert.AreEqual(1, _dataAccess.ExecuteSqlWithParametersAsync(TableFactory<Artist>.GetTable<ArtistTable>().GetInsertSql(artist, ref parameters), parameters).Result);
        }

        private void ExecuteSqlWithParametersTest_Update()
        {
            var album = HelperObjectFactory.MakeAlbum(AlbumId, ArtistId, GenreId, AlbumName, Year);
            var artist = HelperObjectFactory.MakeArtist(ArtistId, ArtistName);
            var genre = HelperObjectFactory.MakeGenre(GenreId, GenreName);

            var parameters = new List<SQLiteParameter>();

            // Assert the tables exist
            Assert.AreEqual(1, _dataAccess.ExecuteSqlWithParametersAsync(TableFactory<Album>.GetTable<AlbumTable>().GetUpdateSql(album, ref parameters), parameters).Result);
            parameters.Clear();
            Assert.AreEqual(1, _dataAccess.ExecuteSqlWithParametersAsync(TableFactory<Artist>.GetTable<ArtistTable>().GetUpdateSql(artist, ref parameters), parameters).Result);
            parameters.Clear();
            Assert.AreEqual(1, _dataAccess.ExecuteSqlWithParametersAsync(TableFactory<Genre>.GetTable<GenreTable>().GetUpdateSql(genre, ref parameters), parameters).Result);
        }

        [OneTimeTearDown]
        public void CleanupTest()
        {
            // this is the close db test
            _dataAccess.CloseDatabase();

            // this is the destroy db test
            _dataAccess.DestroyDatabase();

            // this is the dispose db test
            _dataAccess.Dispose();
        }

        [Test]
        public void ExecuteCountQueryWithParametersTest()
        {
            var parameters = new List<SQLiteParameter>();

            // Assert the tables exist
            Assert.AreEqual(1, _dataAccess.ExecuteCountQueryWithParameters($"SELECT COUNT(name) FROM sqlite_master WHERE type='table' AND name='{TableFactory<Album>.GetTable<AlbumTable>().TableName}';", parameters));
            Assert.AreEqual(1, _dataAccess.ExecuteCountQueryWithParameters($"SELECT COUNT(name) FROM sqlite_master WHERE type='table' AND name='{TableFactory<Artist>.GetTable<ArtistTable>().TableName}';", parameters));
            Assert.AreEqual(1, _dataAccess.ExecuteCountQueryWithParameters($"SELECT COUNT(name) FROM sqlite_master WHERE type='table' AND name='{TableFactory<Genre>.GetTable<GenreTable>().TableName}';", parameters));
        }

        [Test]
        public async Task ExecuteQueryWithParametersTests()
        {
            var parameters = new List<SQLiteParameter>();
            ExecuteSqlWithParametersTest_Insert();
            using (var cursor = await _dataAccess.ExecuteQueryWithParametersAsync($"SELECT * FROM {TableFactory<Genre>.GetTable<GenreTable>().TableName};", parameters))
            {
                Assert.IsFalse(Cursor.IsNullOrEmpty(cursor));
                Assert.AreEqual(2, cursor.GetRowValues().Count);
            }

            ExecuteSqlWithParametersTest_Delete();
        }

        [Test]
        public void ExecuteSqlWithParametersTests()
        {
            ExecuteSqlWithParametersTest_Insert();
            ExecuteCountQueryWithParameters();
            ExecuteSqlWithParametersTest_Update();
            ExecuteCountQueryWithParameters();
            ExecuteSqlWithParametersTest_Delete();
        }
    }
}