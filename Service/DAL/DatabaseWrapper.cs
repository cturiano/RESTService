using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;
using System.Threading.Tasks;
using Service.Interfaces;

#pragma warning disable 1584,1711,1572,1581,1580

namespace Service.DAL
{
    internal class DatabaseWrapper : IDatabaseWrapper
    {
        #region Properties

        private SQLiteTransaction CurrentTransaction { get; set; }

        private SQLiteConnection DatabaseConnection { get; set; }

        public bool IsOpen { get; private set; }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Properly disposes of this object and its resources.
        /// </summary>
        /// <exception cref="DbException">The connection-level error that occurred while opening the connection. </exception>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Close();
            }
        }

        #endregion

        #region Private Methods

        private static void AddParametersToCommand(SQLiteCommand command, IEnumerable<SQLiteParameter> parameters)
        {
            foreach (var sqlParameter in parameters)
            {
                command.Parameters.Add(sqlParameter);
            }
        }

        /// <exception cref="ArgumentNullException"><paramref name="format" /> is null. </exception>
        /// <exception cref="FormatException">
        ///     <paramref name="format" /> is invalid.-or- The index of a format item is less than
        ///     zero, or greater than two.
        /// </exception>
        private static string GetConnectionString(string location, bool poolingOn) => new SQLiteConnectionStringBuilder
                                                                                      {
                                                                                          DataSource = location,
                                                                                          Pooling = poolingOn,
                                                                                          Version = 3,
                                                                                          SyncMode = SynchronizationModes.Full
                                                                                      }.ConnectionString;

        #endregion

        #region Statics And Constants

        private const string DropTableSqlFormat = "DROP TABLE {0};";
        private const string GetOurDatabaseVersionSql = "PRAGMA user_version;";
        private const string SetOurDatabaseVersionSqlFormat = "PRAGMA user_version = {0};";
        private const string TableExistsSqlFormat = "SELECT name FROM sqlite_master WHERE type='table' AND name='{0}';";
        private const string TableInfoNameColumnName = "name";
        private const string TableInfoSqlFormat = "PRAGMA table_info({0});";

        #endregion

        #region Interface Implementations

        public void BeginTransaction()
        {
            CurrentTransaction = DatabaseConnection.BeginTransaction();
        }

        /// <summary>
        ///     Clears the connection pool, closes and disposes the current db connection
        /// </summary>
        /// <exception cref="DbException">The connection-level error that occurred while opening the connection. </exception>
        public void Close()
        {
            if (IsOpen)
            {
                SQLiteConnection.ClearPool(DatabaseConnection);
                DatabaseConnection.Close();
                DatabaseConnection.Dispose();
                IsOpen = false;
            }
        }

        public void CommitTransaction()
        {
            using (CurrentTransaction)
            {
                CurrentTransaction.Commit();
            }
        }

        /// <exception cref="PathTooLongException">
        ///     The specified path, file name, or both exceed the system-defined maximum length.
        ///     For example, on Windows-based platforms, paths must be less than 248 characters, and file names must be less than
        ///     260 characters.
        /// </exception>
        /// <exception cref="DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive). </exception>
        /// <exception cref="IOException">
        ///     The specified file is in use. -or-There is an open handle on the file, and the operating
        ///     system is Windows XP or earlier. This open handle can result from enumerating directories and files. For more
        ///     information, see How to: Enumerate Directories and Files.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid
        ///     characters as defined by <see cref="F:System.IO.Path.GetInvalidFileNameChars" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="path" /> is null.
        /// </exception>
        /// <exception cref="NotSupportedException">
        ///     <paramref name="path" /> is in an invalid format.
        /// </exception>
        /// <exception cref="UnauthorizedAccessException">
        ///     The caller does not have the required permission.-or- The file is an
        ///     executable file that is in use.-or- <paramref name="path" /> is a directory.-or- <paramref name="path" /> specified
        ///     a read-only file.
        /// </exception>
        public void Destroy(string location)
        {
            Close();
            File.Delete(location);
        }

        /// <exception cref="ArgumentNullException"><paramref name="format" /> is null. </exception>
        /// <exception cref="FormatException">
        ///     The format item in <paramref name="format" /> is invalid.-or- The index of a format
        ///     item is not zero.
        /// </exception>
        /// <exception cref="DbException">An error occurred while executing the command text.</exception>
        public async Task DropTableAsync(string tableName)
        {
            await ExecuteSqlWithParametersAsync(string.Format(DropTableSqlFormat, tableName), null);
        }

        public long ExecuteCountQuery(string countSql)
        {
            using (var cursor = ExecuteSingleResultQuery(countSql))
            {
                return cursor.GetLong(Cursor.FirstFieldIndex);
            }
        }

        public long ExecuteCountQueryWithParameters(string countSql, List<SQLiteParameter> parameters)
        {
            using (var cursor = ExecuteQueryWithParameters(countSql, parameters, true))
            {
                return cursor.GetLong(Cursor.FirstFieldIndex);
            }
        }

        public ICursor ExecuteQuery(string sql)
        {
            using (var command = DatabaseConnection.CreateCommand())
            {
                command.CommandText = sql;
                return new Cursor(command);
            }
        }

        public ICursor ExecuteQueryWithParameters(string sql, List<SQLiteParameter> parameters, bool singleResult = false)
        {
            using (var command = DatabaseConnection.CreateCommand())
            {
                command.CommandText = sql;
                if (parameters != null)
                {
                    AddParametersToCommand(command, parameters);
                }

                return new Cursor(command, singleResult);
            }
        }

        public ICursor ExecuteSingleResultQuery(string sql)
        {
            using (var command = DatabaseConnection.CreateCommand())
            {
                command.CommandText = sql;
                return new Cursor(command, true);
            }
        }

        /// <inheritdoc />
        /// <summary>
        ///     Execute non query sql.
        /// </summary>
        /// <param name="sql">sql to execute</param>
        /// <returns>the number of rows inserted/updated affected by it</returns>
        public int ExecuteSql(string sql) => ExecuteSqlWithParameters(sql, null);

        /// <summary>
        ///     Synchronously executes the given SQL statement using the given parameters.
        /// </summary>
        /// <param name="sql">The SQL statement to execute.</param>
        /// <param name="parameters">
        ///     The parameters to use to protect against injection attacks.  Can be null if no parameters are
        ///     used in the sql statement.
        /// </param>
        public int ExecuteSqlWithParameters(string sql, List<SQLiteParameter> parameters)
        {
            using (var command = DatabaseConnection.CreateCommand())
            {
                command.CommandText = sql;
                if (parameters != null)
                {
                    AddParametersToCommand(command, parameters);
                }

                return command.ExecuteNonQuery();
            }
        }

        /// <summary>
        ///     Asynchronously executes the given SQL statement using the given parameters.
        /// </summary>
        /// <param name="sql">The SQL statement to execute.</param>
        /// <param name="parameters">
        ///     The parameters to use to protect against injection attacks.  Can be null if no parameters are
        ///     used in the sql statement.
        /// </param>
        /// <exception cref="DbException">An error occurred while executing the command text.</exception>
        public async Task<int> ExecuteSqlWithParametersAsync(string sql, List<SQLiteParameter> parameters)
        {
            using (var command = DatabaseConnection.CreateCommand())
            {
                command.CommandText = sql;
                if (parameters != null)
                {
                    AddParametersToCommand(command, parameters);
                }

                return await command.ExecuteNonQueryAsync();
            }
        }

        /// <exception cref="ArgumentNullException"><paramref name="format" /> is null. </exception>
        /// <exception cref="FormatException">
        ///     The format item in <paramref name="format" /> is invalid.-or- The index of a format
        ///     item is not zero.
        /// </exception>
        public List<string> GetColumnNames(string tableName)
        {
            var columnNames = new List<string>();
            using (var cursor = ExecuteQuery(string.Format(TableInfoSqlFormat, tableName)))
            {
                var nameColumnIndex = cursor.GetColumnIndex(TableInfoNameColumnName);
                do
                {
                    columnNames.Add(cursor.GetString(nameColumnIndex));
                } while (cursor.MoveToNextRow());

                return columnNames;
            }
        }

        public int GetVersion()
        {
            using (var cursor = ExecuteSingleResultQuery(GetOurDatabaseVersionSql))
            {
                return cursor.GetInt(Cursor.FirstFieldIndex);
            }
        }

        public bool HasOneRecordAndClose(ICursor cursor)
        {
            var hasRecord = !cursor.IsEmpty;
            cursor.Dispose();
            return hasRecord;
        }

        /// <summary>
        ///     Asynchronously opens the database
        /// </summary>
        /// <param name="location">The location of the database file.</param>
        public async Task OpenAsync(string location)
        {
            if (IsOpen)
            {
                return;
            }

            if (!File.Exists(location))
            {
                SQLiteConnection.CreateFile(location);
            }

            DatabaseConnection = new SQLiteConnection(GetConnectionString(location, true), true);
            await DatabaseConnection.OpenAsync();
            IsOpen = true;
        }

        public void RollBackTransaction()
        {
            using (CurrentTransaction)
            {
                CurrentTransaction.Rollback();
            }
        }

        /// <summary>
        ///     Asynchronously sets the version of the database to the given value.
        /// </summary>
        /// <param name="version">The version to which to set the database.</param>
        /// <exception cref="ArgumentNullException"><paramref name="format" /> is null. </exception>
        /// <exception cref="FormatException">
        ///     The format item in <paramref name="format" /> is invalid.-or- The index of a format
        ///     item is not zero.
        /// </exception>
        /// <exception cref="DbException">An error occurred while executing the command text.</exception>
        public async Task SetVersionAsync(int version)
        {
            await ExecuteSqlWithParametersAsync(string.Format(SetOurDatabaseVersionSqlFormat, version), null);
        }

        /// <exception cref="ArgumentNullException"><paramref name="format" /> is null. </exception>
        /// <exception cref="FormatException">
        ///     The format item in <paramref name="format" /> is invalid.-or- The index of a format
        ///     item is not zero.
        /// </exception>
        public bool TableExists(string tableName)
        {
            using (var cursor = ExecuteQuery(string.Format(TableExistsSqlFormat, tableName)))
            {
                return !cursor.IsEmpty;
            }
        }

        /// <inheritdoc />
        /// <summary>
        ///     Properly disposes of this object and its resources
        /// </summary>
        /// <exception cref="T:System.Data.Common.DbException">
        ///     The connection-level error that occurred while opening the
        ///     connection.
        /// </exception>
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        #endregion
    }
}