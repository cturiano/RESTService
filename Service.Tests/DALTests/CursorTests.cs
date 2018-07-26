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

// ReSharper disable AssignNullToNotNullAttribute

#pragma warning disable 1584,1711,1572,1581,1580

namespace Service.Tests.DALTests
{
    [TestFixture]
    public class CursorTests
    {
        private const int NumberOfEntriesInTable = 20;
        private const int TestBlobSize = 4096;
        private const string TestDbLocation = "ICursorTests";
        private static DateTime _now;
        private static IDatabaseWrapper DatabaseWrapper { get; set; }
        private static string TableName { get; set; }
        private static string DBPath { get; set; }

        [OneTimeSetUp]
        public void Initialize()
        {
            InitializeAsync().Wait();
        }

        private static async Task InitializeAsync()
        {
            var random = new Random();
            DatabaseWrapper = new DatabaseWrapper();
            DBPath = Path.Combine(Path.GetDirectoryName(Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Path)), TestDbLocation);
            await DatabaseWrapper.OpenAsync(DBPath);

            _now = DateTime.Now.ToUniversalTime();
            TableName = "testingTable" + HelperObjectFactory.GetRandomInt(0, 100);
            DatabaseWrapper.ExecuteSql($"CREATE TABLE {TableName} (string TEXT PRIMARY KEY, stringNullable TEXT, boolTrue NUMERIC NOT NULL, boolFalse NUMERIC NOT NULL, boolNullable NUMERIC, short INTEGER, int INTEGER, long INTEGER, float REAL, double REAL, guid TEXT, dateTime NUMERIC, nullColumn TEXT, blob NONE) WITHOUT ROWID;");

            for (var i = 0; i < NumberOfEntriesInTable; i++)
            {
                var mod = i & 1;
                var randomBytes = new byte[TestBlobSize];
                random.NextBytes(randomBytes);

                var parameters = new List<SQLiteParameter>();
                var stringParameter = SqliteExtensions.Create("@string", DbType.String, "this is a string" + HelperObjectFactory.GetRandomInt());
                var stringNullableParameter = SqliteExtensions.Create("@stringNullable", DbType.String, mod == 0 ? null : "this is a string" + HelperObjectFactory.GetRandomInt());
                var boolTrueParameter = SqliteExtensions.Create("@boolTrue", DbType.Boolean, true);
                var boolFalseParameter = SqliteExtensions.Create("@boolFalse", DbType.Boolean, false);
                var boolNullableParameter = SqliteExtensions.Create("@boolNullable", DbType.Boolean, mod == 0 ? null : new bool?(Convert.ToBoolean(mod)));
                var shortParameter = SqliteExtensions.Create("@short", DbType.Int16, mod == 0 ? null : (short?)HelperObjectFactory.GetRandomInt());
                var intParameter = SqliteExtensions.Create("@int", DbType.Int32, mod == 0 ? null : (int?)HelperObjectFactory.GetRandomInt());
                var longParameter = SqliteExtensions.Create("@long", DbType.Int64, mod == 0 ? null : (long?)HelperObjectFactory.GetRandomInt());
                var floatParameter = SqliteExtensions.Create("@float", DbType.Single, mod == 0 ? null : (float?)random.NextDouble());
                var doubleParameter = SqliteExtensions.Create("@double", DbType.Double, mod == 0 ? null : (double?)random.NextDouble());
                var guidParameter = SqliteExtensions.Create("@guid", DbType.Guid, mod == 0 ? null : (Guid?)Guid.NewGuid());
                var dateTimeParameter = SqliteExtensions.Create("@dateTime", DbType.Int64, mod == 0 ? null : (long?)_now.Ticks);
                var nullParameter = SqliteExtensions.Create("@nullColumn", DbType.Int32, null);
                var blobParameter = SqliteExtensions.Create("@blob", DbType.Binary, mod == 0 ? null : randomBytes);

                parameters.Add(stringParameter);
                parameters.Add(stringNullableParameter);
                parameters.Add(boolTrueParameter);
                parameters.Add(boolFalseParameter);
                parameters.Add(boolNullableParameter);
                parameters.Add(shortParameter);
                parameters.Add(intParameter);
                parameters.Add(longParameter);
                parameters.Add(floatParameter);
                parameters.Add(doubleParameter);
                parameters.Add(dateTimeParameter);
                parameters.Add(guidParameter);
                parameters.Add(nullParameter);
                parameters.Add(blobParameter);

                DatabaseWrapper.ExecuteSqlWithParameters($"INSERT INTO {TableName} (string, stringNullable, boolTrue, boolFalse, boolNullable, short, int, long, float, double, dateTime, guid, nullColumn, blob) VALUES ({stringParameter.ParameterName}, {stringNullableParameter.ParameterName}, {boolTrueParameter.ParameterName}, {boolFalseParameter.ParameterName}, {boolNullableParameter.ParameterName}, {shortParameter.ParameterName}, {intParameter.ParameterName}, {longParameter.ParameterName}, {floatParameter.ParameterName}, {doubleParameter.ParameterName}, {dateTimeParameter.ParameterName}, {guidParameter.ParameterName}, {nullParameter.ParameterName}, {blobParameter.ParameterName});", parameters);
            }
        }

        [OneTimeTearDown]
        public void TearDownTest()
        {
            TearDownTestAsync().Wait();
        }

        private static async Task TearDownTestAsync()
        {
            using (DatabaseWrapper)
            {
                await DatabaseWrapper.DropTableAsync(TableName);
                DatabaseWrapper.Close();
                DatabaseWrapper.Destroy(DBPath);
            }
        }

        [Test]
        public void DisposeTest()
        {
            try
            {
                using (var c = DatabaseWrapper.ExecuteSingleResultQuery($"SELECT * FROM {TableName};"))
                {
                    Assert.IsNotNull(c);
                    c.Dispose();
                }
            }
            catch (Exception e)
            {
                Assert.Fail($"Should not have thrown exception: {e}");
            }
        }

        [Test]
        public void GetBlobMemoryStreamTest()
        {
            using (var cursor = DatabaseWrapper.ExecuteSingleResultQuery($"SELECT blob FROM {TableName} WHERE blob IS NOT NULL;"))
            {
                Assert.IsFalse(cursor.IsEmpty);
                using (var memoryStream = cursor.GetBlobMemoryStream(cursor.GetFirstFieldIndex()))
                {
                    Assert.IsNotNull(memoryStream);
                    Assert.AreEqual(TestBlobSize, memoryStream.Length);
                }
            }
        }

        /// <exception cref="OverflowException">
        ///     The array is multidimensional and contains more than
        ///     <see cref="F:System.Int32.MaxValue" /> elements.
        /// </exception>
        [Test]
        public void GetBlobTest()
        {
            using (var cursor = DatabaseWrapper.ExecuteSingleResultQuery($"SELECT blob FROM {TableName} WHERE blob NOT NULL;"))
            {
                Assert.IsFalse(cursor.IsEmpty);
                var blob = cursor.GetBlob(cursor.GetFirstFieldIndex());

                Assert.IsNotNull(blob);
                Assert.AreEqual(TestBlobSize, blob.Length);
            }

            using (var cursor = DatabaseWrapper.ExecuteSingleResultQuery($"SELECT blob FROM {TableName} WHERE blob NOT NULL;"))
            {
                Assert.IsFalse(cursor.IsEmpty);
                var blob = new byte[TestBlobSize];
                cursor.GetBlob(cursor.GetFirstFieldIndex(), 0, ref blob, 0, TestBlobSize);

                Assert.IsNotNull(blob);
                Assert.AreEqual(TestBlobSize, blob.Length);
            }

            using (var cursor = DatabaseWrapper.ExecuteSingleResultQuery($"SELECT blob FROM {TableName} WHERE blob NOT NULL;"))
            {
                using (var memoryStream = cursor.GetBlobMemoryStream(cursor.GetFirstFieldIndex()))
                {
                    Assert.IsFalse(cursor.IsEmpty);
                    var blob = memoryStream.ToArray();

                    Assert.IsNotNull(blob);
                    Assert.AreEqual(TestBlobSize, blob.Length);
                }
            }
        }

        [Test]
        public void GetBooleanFalseTest()
        {
            using (var cursor = DatabaseWrapper.ExecuteQuery($"SELECT boolFalse FROM {TableName};"))
            {
                Assert.IsFalse(cursor.IsEmpty);
                Assert.IsFalse(cursor.GetBoolean(cursor.GetFirstFieldIndex()));
                Assert.IsTrue(cursor.MoveToNextRow());
                Assert.IsFalse(cursor.GetBoolean(cursor.GetFirstFieldIndex()));
            }
        }

        [Test]
        public void GetBooleanTrueTest()
        {
            using (var cursor = DatabaseWrapper.ExecuteQuery($"SELECT boolTrue FROM {TableName};"))
            {
                Assert.IsFalse(cursor.IsEmpty);
                Assert.IsTrue(cursor.GetBoolean(cursor.GetFirstFieldIndex()));
                Assert.IsTrue(cursor.MoveToNextRow());
                Assert.IsTrue(cursor.GetBoolean(cursor.GetFirstFieldIndex()));
            }
        }

        [Test]
        public void GetByteBufferLengthTest()
        {
            using (var cursor = DatabaseWrapper.ExecuteQuery($"SELECT boolTrue FROM {TableName};"))
            {
                Assert.IsTrue(cursor.GetByteBufferLength() == 1024);
            }
        }

        [Test]
        public void GetColumnCountTest()
        {
            using (var cursor = DatabaseWrapper.ExecuteSingleResultQuery($"SELECT boolTrue FROM {TableName};"))
            {
                Assert.IsFalse(cursor.IsEmpty);
                Assert.AreEqual(1, cursor.GetColumnCount());
            }

            using (var cursor = DatabaseWrapper.ExecuteSingleResultQuery($"SELECT int, boolTrue FROM {TableName};"))
            {
                Assert.IsFalse(cursor.IsEmpty);
                Assert.AreEqual(2, cursor.GetColumnCount());
            }

            using (var cursor = DatabaseWrapper.ExecuteSingleResultQuery($"SELECT int, short, long, string, boolFalse FROM {TableName};"))
            {
                Assert.IsFalse(cursor.IsEmpty);
                Assert.AreEqual(5, cursor.GetColumnCount());
            }
        }

        [Test]
        public void GetColumnIndexTest()
        {
            using (var cursor = DatabaseWrapper.ExecuteSingleResultQuery($"SELECT boolFalse FROM {TableName};"))
            {
                Assert.IsFalse(cursor.IsEmpty);
                Assert.AreEqual(cursor.GetFirstFieldIndex(), cursor.GetColumnIndex("boolFalse"));
            }

            using (var cursor = DatabaseWrapper.ExecuteSingleResultQuery($"SELECT boolTrue FROM {TableName};"))
            {
                Assert.IsFalse(cursor.IsEmpty);
                Assert.AreEqual(-1, cursor.GetColumnIndex("boo"));
            }
        }

        [Test]
        public void GetColumnNamesTest()
        {
            using (var cursor = DatabaseWrapper.ExecuteSingleResultQuery($"SELECT boolTrue FROM {TableName};"))
            {
                Assert.IsFalse(cursor.IsEmpty);
                var columnNames = cursor.GetColumnNames();
                Assert.AreEqual(1, columnNames.Count);
                Assert.AreEqual("boolTrue", columnNames[0]);
            }

            var expectedColumnNames = new List<string>
                                      {
                                          "boolTrue",
                                          "int",
                                          "string"
                                      };

            var sql = $"SELECT {expectedColumnNames[0]}, {expectedColumnNames[1]}, {expectedColumnNames[2]} FROM {TableName};";

            using (var cursor = DatabaseWrapper.ExecuteSingleResultQuery(sql))
            {
                Assert.IsFalse(cursor.IsEmpty);
                var returnedColumnNames = cursor.GetColumnNames();
                Assert.AreEqual(expectedColumnNames.Count, returnedColumnNames.Count);

                foreach (var returnedColumnName in returnedColumnNames)
                {
                    Assert.IsTrue(expectedColumnNames.Contains(returnedColumnName));
                }
            }
        }

        [Test]
        public void GetColumnNameTest()
        {
            using (var cursor = DatabaseWrapper.ExecuteQuery($"SELECT boolFalse FROM {TableName};"))
            {
                Assert.IsTrue(cursor.GetColumnName(0) == "boolFalse");
            }
        }

        [Test]
        public void GetDateTimeFromUtcTicksToLocalTimeTest()
        {
            using (var cursor = DatabaseWrapper.ExecuteSingleResultQuery($"SELECT dateTime FROM {TableName} WHERE dateTime IS NOT NULL;"))
            {
                Assert.IsFalse(cursor.IsEmpty);
                Assert.IsNotNull(cursor.GetDateTimeFromUtcTicksToLocalTime(cursor.GetFirstFieldIndex()));
                Assert.AreEqual(_now.ToLocalTime(), cursor.GetDateTimeFromUtcTicksToLocalTime(cursor.GetFirstFieldIndex()));
            }
        }

        [Test]
        public void GetDoubleTest()
        {
            using (var cursor = DatabaseWrapper.ExecuteSingleResultQuery($"SELECT double FROM {TableName} WHERE double IS NOT NULL;"))
            {
                Assert.IsFalse(cursor.IsEmpty);
                cursor.GetDouble(cursor.GetFirstFieldIndex());
            }
        }

        [Test]
        public void GetFirstFieldIndexTest()
        {
            using (var cursor = DatabaseWrapper.ExecuteSingleResultQuery($"SELECT * FROM {TableName};"))
            {
                Assert.AreEqual(0, cursor.GetFirstFieldIndex());
            }
        }

        [Test]
        public void GetFloatTest()
        {
            using (var cursor = DatabaseWrapper.ExecuteSingleResultQuery($"SELECT float FROM {TableName} WHERE float IS NOT NULL;"))
            {
                Assert.IsFalse(cursor.IsEmpty);
                cursor.GetFloat(cursor.GetFirstFieldIndex());
            }
        }

        [Test]
        public void GetGuidTest()
        {
            using (var cursor = DatabaseWrapper.ExecuteSingleResultQuery($"SELECT guid FROM {TableName} WHERE guid IS NOT NULL;"))
            {
                Assert.IsFalse(cursor.IsEmpty);
                cursor.GetGuid(cursor.GetFirstFieldIndex());
            }
        }

        [Test]
        public void GetIntTest()
        {
            using (var cursor = DatabaseWrapper.ExecuteSingleResultQuery($"SELECT int FROM {TableName} WHERE int IS NOT NULL;"))
            {
                Assert.IsFalse(cursor.IsEmpty);
                cursor.GetInt(cursor.GetFirstFieldIndex());
            }
        }

        [Test]
        public void GetLongTest()
        {
            using (var cursor = DatabaseWrapper.ExecuteSingleResultQuery($"SELECT long FROM {TableName} WHERE long IS NOT NULL;"))
            {
                Assert.IsFalse(cursor.IsEmpty);
                cursor.GetLong(cursor.GetFirstFieldIndex());
            }
        }

        [Test]
        public void GetNullableBlobMemoryStreamTest()
        {
            using (var cursor = DatabaseWrapper.ExecuteSingleResultQuery($"SELECT blob FROM {TableName} WHERE blob IS NOT NULL;"))
            {
                Assert.IsFalse(cursor.IsEmpty);
                var memoryStream = cursor.GetNullableBlobMemoryStream(cursor.GetFirstFieldIndex());

                Assert.IsNotNull(memoryStream);
                Assert.AreEqual(TestBlobSize, memoryStream.Length);
            }

            using (var cursor = DatabaseWrapper.ExecuteSingleResultQuery($"SELECT blob FROM {TableName} WHERE blob IS NULL;"))
            {
                Assert.IsFalse(cursor.IsEmpty);
                var memoryStream = cursor.GetNullableBlobMemoryStream(cursor.GetFirstFieldIndex());

                Assert.IsNull(memoryStream);
            }
        }

        [Test]
        public void GetNullableBooleanTest()
        {
            using (var cursor = DatabaseWrapper.ExecuteSingleResultQuery($"SELECT boolNullable FROM {TableName} WHERE boolNullable IS NOT NULL;"))
            {
                Assert.IsFalse(cursor.IsEmpty);
                Assert.IsNotNull(cursor.GetNullableBoolean(cursor.GetFirstFieldIndex()));
            }

            using (var cursor = DatabaseWrapper.ExecuteSingleResultQuery($"SELECT boolNullable FROM {TableName} WHERE boolNullable IS NULL;"))
            {
                Assert.IsFalse(cursor.IsEmpty);
                Assert.IsNull(cursor.GetNullableBoolean(cursor.GetFirstFieldIndex()));
            }
        }

        [Test]
        public void GetNullableDateTimeFromUtcTicksToLocalTime()
        {
            using (var cursor = DatabaseWrapper.ExecuteSingleResultQuery($"SELECT dateTime FROM {TableName} WHERE dateTime IS NOT NULL;"))
            {
                Assert.IsFalse(cursor.IsEmpty);
                Assert.IsNotNull(cursor.GetNullableDateTimeFromUtcTicksToLocalTime(cursor.GetFirstFieldIndex()));
            }

            using (var cursor = DatabaseWrapper.ExecuteSingleResultQuery($"SELECT dateTime FROM {TableName} WHERE dateTime IS NULL;"))
            {
                Assert.IsFalse(cursor.IsEmpty);
                Assert.IsNull(cursor.GetNullableDateTimeFromUtcTicksToLocalTime(cursor.GetFirstFieldIndex()));
            }
        }

        [Test]
        public void GetNullableDoubleTest()
        {
            using (var cursor = DatabaseWrapper.ExecuteSingleResultQuery($"SELECT double FROM {TableName} WHERE double IS NOT NULL;"))
            {
                Assert.IsFalse(cursor.IsEmpty);
                Assert.IsNotNull(cursor.GetNullableDouble(cursor.GetFirstFieldIndex()));
            }

            using (var cursor = DatabaseWrapper.ExecuteSingleResultQuery($"SELECT double FROM {TableName} WHERE double IS NULL;"))
            {
                Assert.IsFalse(cursor.IsEmpty);
                Assert.IsNull(cursor.GetNullableDouble(cursor.GetFirstFieldIndex()));
            }
        }

        [Test]
        public void GetNullableFloatTest()
        {
            using (var cursor = DatabaseWrapper.ExecuteSingleResultQuery($"SELECT float FROM {TableName} WHERE float IS NOT NULL;"))
            {
                Assert.IsFalse(cursor.IsEmpty);
                Assert.IsNotNull(cursor.GetNullableFloat(cursor.GetFirstFieldIndex()));
            }

            using (var cursor = DatabaseWrapper.ExecuteSingleResultQuery($"SELECT float FROM {TableName} WHERE float IS NULL;"))
            {
                Assert.IsFalse(cursor.IsEmpty);
                Assert.IsNull(cursor.GetNullableFloat(cursor.GetFirstFieldIndex()));
            }
        }

        [Test]
        public void GetNullableIntTest()
        {
            using (var cursor = DatabaseWrapper.ExecuteSingleResultQuery($"SELECT int FROM {TableName} WHERE int IS NOT NULL;"))
            {
                Assert.IsFalse(cursor.IsEmpty);
                Assert.IsNotNull(cursor.GetNullableInt(cursor.GetFirstFieldIndex()));
            }

            using (var cursor = DatabaseWrapper.ExecuteSingleResultQuery($"SELECT int FROM {TableName} WHERE int IS NULL;"))
            {
                Assert.IsFalse(cursor.IsEmpty);
                Assert.IsNull(cursor.GetNullableInt(cursor.GetFirstFieldIndex()));
            }
        }

        [Test]
        public void GetNullableLongTest()
        {
            using (var cursor = DatabaseWrapper.ExecuteSingleResultQuery($"SELECT long FROM {TableName} WHERE long IS NOT NULL;"))
            {
                Assert.IsFalse(cursor.IsEmpty);
                Assert.IsNotNull(cursor.GetNullableLong(cursor.GetFirstFieldIndex()));
            }

            using (var cursor = DatabaseWrapper.ExecuteSingleResultQuery($"SELECT long FROM {TableName} WHERE long IS NULL;"))
            {
                Assert.IsFalse(cursor.IsEmpty);
                Assert.IsNull(cursor.GetNullableLong(cursor.GetFirstFieldIndex()));
            }
        }

        [Test]
        public void GetNullableShortTest()
        {
            using (var cursor = DatabaseWrapper.ExecuteSingleResultQuery($"SELECT short FROM {TableName} WHERE short IS NOT NULL;"))
            {
                Assert.IsFalse(cursor.IsEmpty);
                Assert.IsNotNull(cursor.GetNullableShort(cursor.GetFirstFieldIndex()));
            }

            using (var cursor = DatabaseWrapper.ExecuteSingleResultQuery($"SELECT short FROM {TableName} WHERE short IS NULL;"))
            {
                Assert.IsFalse(cursor.IsEmpty);
                Assert.IsNull(cursor.GetNullableShort(cursor.GetFirstFieldIndex()));
            }
        }

        [Test]
        public void GetNullableStringTest()
        {
            using (var cursor = DatabaseWrapper.ExecuteSingleResultQuery($"SELECT stringNullable FROM {TableName} WHERE stringNullable IS NOT NULL;"))
            {
                Assert.IsFalse(cursor.IsEmpty);
                Assert.IsNotNull(cursor.GetNullableString(cursor.GetFirstFieldIndex()));
            }

            using (var cursor = DatabaseWrapper.ExecuteSingleResultQuery($"SELECT stringNullable FROM {TableName} WHERE stringNullable IS NULL;"))
            {
                Assert.IsFalse(cursor.IsEmpty);
                Assert.IsNull(cursor.GetNullableString(cursor.GetFirstFieldIndex()));
            }
        }

        [Test]
        public void GetRowValues()
        {
            var expectedColumnNames = new List<string>
                                      {
                                          "boolTrue",
                                          "int",
                                          "string"
                                      };

            var sql = $"SELECT {expectedColumnNames[0]}, {expectedColumnNames[1]}, {expectedColumnNames[2]} FROM {TableName};";

            using (var cursor = DatabaseWrapper.ExecuteSingleResultQuery(sql))
            {
                Assert.IsFalse(cursor.IsEmpty);
                var values = cursor.GetRowValues();
                Assert.AreEqual(expectedColumnNames.Count, values.Count);
            }
        }

        [Test]
        public void GetShortTest()
        {
            using (var cursor = DatabaseWrapper.ExecuteSingleResultQuery($"SELECT short FROM {TableName} WHERE short IS NOT NULL;"))
            {
                Assert.IsFalse(cursor.IsEmpty);
                cursor.GetShort(cursor.GetFirstFieldIndex());
            }
        }

        [Test]
        public void GetStringTest()
        {
            using (var cursor = DatabaseWrapper.ExecuteSingleResultQuery($"SELECT string FROM {TableName} WHERE string IS NOT NULL;"))
            {
                Assert.IsFalse(cursor.IsEmpty);
                cursor.GetString(cursor.GetFirstFieldIndex());
            }
        }

        /// <exception cref="Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException">
        ///     <paramref name="condition" /> evaluates to true.
        /// </exception>
        [Test]
        public void GetValueTest()
        {
            using (var cursor = DatabaseWrapper.ExecuteSingleResultQuery($"SELECT boolNullable FROM {TableName} WHERE boolNullable IS NOT NULL;"))
            {
                Assert.IsFalse(cursor.IsEmpty);
                try
                {
                    // ReSharper disable once UnusedVariable
                    var boolean = Convert.ToBoolean(cursor.GetValue(cursor.GetFirstFieldIndex()));
                }
                catch (Exception)
                {
                    Assert.Fail();
                }
            }

            using (var cursor = DatabaseWrapper.ExecuteSingleResultQuery($"SELECT string FROM {TableName} WHERE string IS NOT NULL;"))
            {
                Assert.IsFalse(cursor.IsEmpty);
                try
                {
                    // ReSharper disable once UnusedVariable
                    var s = cursor.GetString(cursor.GetFirstFieldIndex());
                }
                catch (Exception)
                {
                    Assert.Fail();
                }
            }
        }

        [Test]
        public void IsClosed()
        {
            using (var cursor = DatabaseWrapper.ExecuteSingleResultQuery($"SELECT string FROM {TableName} WHERE string IS NOT NULL;"))
            {
                Assert.IsFalse(cursor.IsClosed());
                cursor.Close();
                Assert.IsTrue(cursor.IsClosed());
            }
        }

        [Test]
        public void IsEmptyTest()
        {
            using (var cursor = DatabaseWrapper.ExecuteSingleResultQuery($"SELECT string FROM {TableName};"))
            {
                Assert.IsFalse(cursor.IsEmpty);
            }

            using (var cursor = DatabaseWrapper.ExecuteSingleResultQuery($"SELECT string FROM {TableName} WHERE string = 'notExistingValue';"))
            {
                Assert.IsTrue(cursor.IsEmpty);
            }
        }

        [Test]
        public void IsNullTest()
        {
            using (var cursor = DatabaseWrapper.ExecuteSingleResultQuery($"SELECT string FROM {TableName} WHERE string IS NOT NULL;"))
            {
                Assert.IsFalse(cursor.IsEmpty);
                Assert.IsFalse(cursor.IsNull(cursor.GetFirstFieldIndex()));
            }

            using (var cursor = DatabaseWrapper.ExecuteSingleResultQuery($"SELECT nullColumn FROM {TableName};"))
            {
                Assert.IsFalse(cursor.IsEmpty);
                Assert.IsTrue(cursor.IsNull(cursor.GetFirstFieldIndex()));
            }
        }

        [Test]
        public void MoveToNextRow()
        {
            using (var cursor = DatabaseWrapper.ExecuteSingleResultQuery($"SELECT boolNullable FROM {TableName} WHERE boolNullable IS NOT NULL;"))
            {
                Assert.IsFalse(cursor.IsEmpty);
                Assert.IsTrue(cursor.IsFirstRow());
                Assert.IsFalse(cursor.MoveToNextRow());
            }

            using (var cursor = DatabaseWrapper.ExecuteQuery($"SELECT boolNullable FROM {TableName} WHERE boolNullable IS NOT NULL;"))
            {
                Assert.IsFalse(cursor.IsEmpty);
                Assert.IsTrue(cursor.IsFirstRow());

                var lastRowPosition = cursor.GetRowPosition();
                while (cursor.MoveToNextRow())
                {
                    var currentRowPosition = cursor.GetRowPosition();
                    Assert.AreEqual(lastRowPosition + 1, currentRowPosition);

                    lastRowPosition = currentRowPosition;
                }

                cursor.MoveToNextRow();
                Assert.AreEqual(lastRowPosition, cursor.GetRowPosition());
            }
        }

        [Test]
        public void OpenCloseTest()
        {
            using (var cursor = DatabaseWrapper.ExecuteSingleResultQuery($"SELECT int FROM {TableName} WHERE int IS NOT NULL;"))
            {
                Assert.IsFalse(cursor.IsEmpty);
                Assert.IsFalse(cursor.IsClosed());

                cursor.Close();
                Assert.IsTrue(cursor.IsClosed());

                cursor.Close();
                Assert.IsTrue(cursor.IsClosed());
            }
        }
    }
}