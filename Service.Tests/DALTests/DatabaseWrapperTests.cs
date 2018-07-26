using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using NUnit.Framework;
using Service.DAL;
using Service.Extensions;
using Service.Interfaces;

namespace Service.Tests.DALTests
{
    [TestFixture]
    public class DatabaseWrapperTests
    {
        private IDatabaseWrapper DatabaseWrapper { get; set; }

        private string TestDbLocation { get; set; }

        [OneTimeTearDown]
        public void CleanUpDb()
        {
            DatabaseWrapper.Dispose();
            DatabaseWrapper.Destroy(TestDbLocation);
        }

        [OneTimeSetUp]
        public void Initialize()
        {
            InitializeAsync().Wait();
        }

        private async Task InitializeAsync()
        {
            TestDbLocation = Path.Combine(Path.GetDirectoryName(Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Path)), "TestDb" + HelperObjectFactory.GetRandomInt(0));
            DatabaseWrapper = new DatabaseWrapper();
            await DatabaseWrapper.OpenAsync(TestDbLocation);
        }

        [Test]
        public void CloseTest()
        {
            DatabaseWrapper.Close();
            Assert.IsFalse(DatabaseWrapper.IsOpen);

            // if DB is closed, reopen for other tests
            if (!DatabaseWrapper.IsOpen)
            {
                DatabaseWrapper.OpenAsync(TestDbLocation).Wait();
            }
        }

        [Test]
        public async Task CreateOpenTest()
        {
            if (DatabaseWrapper.IsOpen)
            {
                DatabaseWrapper.Close();
            }

            Assert.IsFalse(DatabaseWrapper.IsOpen);
            await DatabaseWrapper.OpenAsync(TestDbLocation);
            Assert.IsTrue(DatabaseWrapper.IsOpen);
        }

        [Test]
        public void DestroyTest()
        {
            DatabaseWrapper.Close();
            DatabaseWrapper.Destroy(TestDbLocation);
            Assert.IsFalse(File.Exists(TestDbLocation));
        }

        [Test]
        public void DisposeTest()
        {
            try
            {
                DatabaseWrapper.Dispose();
                Assert.IsFalse(DatabaseWrapper.IsOpen);

                // if DB is closed (aka Disposed), reopen for other tests
                if (!DatabaseWrapper.IsOpen)
                {
                    InitializeAsync().Wait();
                }
            }
            catch
            {
                Assert.Fail();
            }
        }

        [Test]
        public async Task DropTableTest()
        {
            var tableName = "testingTable" + HelperObjectFactory.GetRandomInt(0);
            DatabaseWrapper.ExecuteSql($"CREATE TABLE {tableName}(a,b);");

            await DatabaseWrapper.DropTableAsync(tableName);

            Assert.IsFalse(DatabaseWrapper.TableExists(tableName));
        }

        [Test]
        public void ExecuteCountQueryTest()
        {
            var tableName = "testingTable" + HelperObjectFactory.GetRandomInt(0);

            DatabaseWrapper.ExecuteSql($"CREATE TABLE {tableName}(a,b);");
            Assert.AreEqual(DatabaseWrapper.ExecuteCountQuery($"SELECT COUNT(a) FROM {tableName}"), 0);

            DatabaseWrapper.ExecuteSql($"INSERT INTO {tableName}(a,b) VALUES ('test0', 'test1');");
            Assert.AreEqual(DatabaseWrapper.ExecuteCountQuery($"SELECT COUNT(a) FROM {tableName}"), 1);

            DatabaseWrapper.ExecuteSql($"INSERT INTO {tableName}(a,b) VALUES ('test6', 'test3');");
            Assert.AreEqual(DatabaseWrapper.ExecuteCountQuery($"SELECT COUNT(a) FROM {tableName}"), 2);

            DatabaseWrapper.ExecuteSql($"DROP TABLE {tableName};");
        }

        [Test]
        public void ExecuteCountQueryWithParametersTest()
        {
            var tableName = "testingTable" + HelperObjectFactory.GetRandomInt(0);
            var parameters = new List<SQLiteParameter>();
            var aParameter = SqliteExtensions.Create("a", DbType.String, "string1");
            var bParameter = SqliteExtensions.Create("b", DbType.Int32, 32);
            parameters.Add(aParameter);
            parameters.Add(bParameter);

            DatabaseWrapper.ExecuteSql($"CREATE TABLE {tableName}(a, b);");
            Assert.AreEqual(DatabaseWrapper.ExecuteCountQueryWithParameters($"SELECT COUNT(a) FROM {tableName} WHERE a = 'nothing';", parameters), 0);

            DatabaseWrapper.ExecuteSqlWithParameters($"INSERT INTO {tableName}(a, b) VALUES ({aParameter.ParameterName}, {bParameter.ParameterName});", parameters);
            Assert.AreEqual(DatabaseWrapper.ExecuteCountQueryWithParameters($"SELECT COUNT(a) FROM {tableName} WHERE a = {aParameter.ParameterName};", parameters), 1);

            aParameter = SqliteExtensions.Create("a", DbType.String, "string2");
            bParameter = SqliteExtensions.Create("b", DbType.Int32, 64);
            DatabaseWrapper.ExecuteSqlWithParameters($"INSERT INTO {tableName}(a, b) VALUES ({aParameter.ParameterName}, {bParameter.ParameterName});", parameters);
            Assert.AreEqual(DatabaseWrapper.ExecuteCountQueryWithParameters($"SELECT COUNT(b) FROM {tableName} WHERE b > 20;", parameters), 2);

            DatabaseWrapper.ExecuteSql($"DROP TABLE {tableName};");
        }

        [Test]
        public void ExecuteQueryTest()
        {
            var tableName = "testingTable" + HelperObjectFactory.GetRandomInt(0);
            DatabaseWrapper.ExecuteSql($"CREATE TABLE {tableName}(a,b);");
            DatabaseWrapper.ExecuteSql($"INSERT INTO {tableName}(a,b) VALUES ('test0', 'test1');");

            using (var cursor = DatabaseWrapper.ExecuteQuery($"SELECT * FROM {tableName};"))
            {
                Assert.IsNotNull(cursor);
                Assert.IsNotNull(cursor.GetString(cursor.GetColumnIndex("a")));
                Assert.IsNotNull(cursor.GetString(cursor.GetColumnIndex("b")));
            }

            DatabaseWrapper.ExecuteSql($"DROP TABLE {tableName};");
        }

        [Test]
        public void ExecuteQueryWithParametersTest()
        {
            var tableName = "testingTable" + HelperObjectFactory.GetRandomInt(0);
            var parameters = new List<SQLiteParameter>();
            var aParameter = SqliteExtensions.Create("a", DbType.String, "string1");
            var bParameter = SqliteExtensions.Create("b", DbType.Int32, 32);
            parameters.Add(aParameter);
            parameters.Add(bParameter);

            DatabaseWrapper.ExecuteSql($"CREATE TABLE {tableName}(a, b);");
            using (var c = DatabaseWrapper.ExecuteSingleResultQuery($"SELECT a FROM {tableName} WHERE a = 'nothing';"))
            {
                Assert.IsTrue(c.IsEmpty);
            }

            DatabaseWrapper.ExecuteSqlWithParameters($"INSERT INTO {tableName}(a, b) VALUES ({aParameter.ParameterName}, {bParameter.ParameterName});", parameters);
            using (var c = DatabaseWrapper.ExecuteQueryWithParameters($"SELECT a FROM {tableName} WHERE a = {aParameter.ParameterName};", parameters))
            {
                Assert.AreEqual(c.GetString(0), "string1");
            }

            aParameter = SqliteExtensions.Create("a", DbType.String, "string2");
            bParameter = SqliteExtensions.Create("b", DbType.Int32, 64);
            DatabaseWrapper.ExecuteSqlWithParameters($"INSERT INTO {tableName}(a, b) VALUES ({aParameter.ParameterName}, {bParameter.ParameterName});", parameters);
            using (var c = DatabaseWrapper.ExecuteQueryWithParameters($"SELECT b FROM {tableName} WHERE b = {bParameter.ParameterName};", parameters))
            {
                Assert.AreEqual(32, c.GetInt(0));
            }

            DatabaseWrapper.ExecuteSql($"DROP TABLE {tableName};");
        }

        [Test]
        public void ExecuteSingleResultQueryTest()
        {
            var tableName = "testingTable" + HelperObjectFactory.GetRandomInt(0);
            DatabaseWrapper.ExecuteSql($"CREATE TABLE {tableName}(a,b);");
            DatabaseWrapper.ExecuteSql($"INSERT INTO {tableName}(a,b) VALUES ('test0', 'test1');");
            DatabaseWrapper.ExecuteSql($"INSERT INTO {tableName}(a,b) VALUES ('test6', 'test3');");

            using (var cursor = DatabaseWrapper.ExecuteSingleResultQuery($"SELECT * FROM {tableName};"))
            {
                Assert.IsNotNull(cursor);
                Assert.IsFalse(cursor.MoveToNextRow());
            }

            DatabaseWrapper.ExecuteSql($"DROP TABLE {tableName};");
        }

        [Test]
        public void ExecuteSqlTest()
        {
            var tableName = "testingTable" + HelperObjectFactory.GetRandomInt(0);

            try
            {
                DatabaseWrapper.ExecuteSql($"CREATE TABLE {tableName}(a,b);");
                DatabaseWrapper.ExecuteSql($"DROP TABLE {tableName};");
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }

        [Test]
        public void ExecuteSqlWithParametersTest()
        {
            var tableName = "testingTable" + HelperObjectFactory.GetRandomInt(0);
            DatabaseWrapper.ExecuteSql($"CREATE TABLE {tableName}(a,b);");

            var parameters = new List<SQLiteParameter>();
            var testParameter = new SQLiteParameter("@testParamName", DbType.String)
                                {
                                    Value = "testValue"
                                };

            parameters.Add(testParameter);

            try
            {
                DatabaseWrapper.ExecuteSqlWithParameters($"INSERT INTO {tableName}(a,b) VALUES ('test0', {testParameter.ParameterName});", parameters);
            }
            catch (Exception)
            {
                Assert.Fail();
            }

            DatabaseWrapper.ExecuteSql($"DROP TABLE {tableName};");
        }

        [Test]
        public void GetColumnNamesTest()
        {
            var tableName = "testingTable" + HelperObjectFactory.GetRandomInt(0);
            DatabaseWrapper.ExecuteSql($"CREATE TABLE {tableName}(a,b);");

            var columnNames = DatabaseWrapper.GetColumnNames(tableName);
            Assert.AreEqual(columnNames.Count, 2);
            Assert.IsTrue(columnNames.Contains("a"));

            DatabaseWrapper.ExecuteSql($"DROP TABLE {tableName};");
        }

        [Test]
        public void HasOneRecordAndCloseTest()
        {
            var tableName = "testingTable" + HelperObjectFactory.GetRandomInt(0);
            DatabaseWrapper.ExecuteSql($"CREATE TABLE {tableName}(a,b);");

            using (var cursor = DatabaseWrapper.ExecuteSingleResultQuery($"SELECT * FROM {tableName};"))
            {
                Assert.IsFalse(DatabaseWrapper.HasOneRecordAndClose(cursor));
                Assert.IsTrue(cursor.IsClosed());
            }

            DatabaseWrapper.ExecuteSql($"INSERT INTO {tableName}(a,b) VALUES ('test0', 'test1');");

            using (var cursor = DatabaseWrapper.ExecuteSingleResultQuery($"SELECT * FROM {tableName};"))
            {
                Assert.IsTrue(DatabaseWrapper.HasOneRecordAndClose(cursor));
                Assert.IsTrue(cursor.IsClosed());
            }

            DatabaseWrapper.ExecuteSql($"DROP TABLE {tableName};");
        }

        [Test]
        public void IsOpenTest()
        {
            Assert.IsTrue(DatabaseWrapper.IsOpen);
            DatabaseWrapper.Close();
            Assert.IsFalse(DatabaseWrapper.IsOpen);
        }

        [Test]
        public async Task PasswordTest()
        {
            try
            {
                DatabaseWrapper = new DatabaseWrapper();
                Assert.IsFalse(DatabaseWrapper.IsOpen);
                await DatabaseWrapper.OpenAsync(TestDbLocation);
                Assert.IsTrue(DatabaseWrapper.IsOpen);
            }
            catch (Exception e)
            {
                Assert.Fail($"Exception was thrown: {e}");
            }
        }

        [Test]
        public void TableExistsTest()
        {
            var tableName = "testingTable" + HelperObjectFactory.GetRandomInt(0);

            Assert.IsFalse(DatabaseWrapper.TableExists(tableName));

            DatabaseWrapper.ExecuteSql($"CREATE TABLE {tableName}(a,b);");
            Assert.IsTrue(DatabaseWrapper.TableExists(tableName));
            DatabaseWrapper.ExecuteSql($"DROP TABLE {tableName};");
        }

        [Test]
        public void TransactionsTest()
        {
            DatabaseWrapper.BeginTransaction();
            var tableName = "testingTable" + HelperObjectFactory.GetRandomInt(0);
            DatabaseWrapper.ExecuteSql($"CREATE TABLE {tableName}(a,b);");
            DatabaseWrapper.ExecuteSql($"INSERT INTO {tableName}(a,b) VALUES ('test0', 'test1');");
            DatabaseWrapper.CommitTransaction();

            using (var cursor = DatabaseWrapper.ExecuteSingleResultQuery($"SELECT * FROM {tableName};"))
            {
                Assert.IsTrue(DatabaseWrapper.HasOneRecordAndClose(cursor));
            }

            DatabaseWrapper.BeginTransaction();
            DatabaseWrapper.ExecuteSql($"INSERT INTO {tableName}(a,b) VALUES ('test20', 'test21');");
            DatabaseWrapper.ExecuteSql($"INSERT INTO {tableName}(a,b) VALUES ('test20', 'test21');");
            DatabaseWrapper.RollBackTransaction();

            Assert.AreEqual(DatabaseWrapper.ExecuteCountQuery($"SELECT COUNT(a) FROM {tableName};"), 1);

            DatabaseWrapper.BeginTransaction();
            DatabaseWrapper.ExecuteSql($"INSERT INTO {tableName}(a,b) VALUES ('test20', 'test21');");
            DatabaseWrapper.ExecuteSql($"INSERT INTO {tableName}(a,b) VALUES ('test20', 'test21');");
            DatabaseWrapper.CommitTransaction();

            Assert.AreEqual(DatabaseWrapper.ExecuteCountQuery($"SELECT COUNT(a) FROM {tableName};"), 3);
        }

        [Test]
        public void VersionTest()
        {
            var randomVersion = HelperObjectFactory.GetRandomInt(0);

            try
            {
                DatabaseWrapper.SetVersionAsync(randomVersion);
                Assert.AreEqual(randomVersion, DatabaseWrapper.GetVersion());
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }
    }
}