using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Service.DAL.Tables;
using Service.Interfaces;
using Service.Models;

namespace Service.DAL
{
    public class DataAccess : IDataAccess
    {
        #region Constructors

        public DataAccess(string databaseName)
        {
            DatabaseName = databaseName;
            DatabaseWrapper = new DatabaseWrapper();
        }

        #endregion

        #region Properties

        public string DatabaseName { get; }

        protected IDatabaseWrapper DatabaseWrapper { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Closes the database on a separate task.
        /// </summary>
        /// <exception cref="DbException">The connection-level error that occurred while opening the connection. </exception>
        public void CloseDatabase()
        {
            DatabaseWrapper.Close();
        }

        /// <summary>
        ///     Destroys the database file within the Obfuscated File System.
        /// </summary>
        /// <exception cref="ArgumentNullException">The parameter is null. </exception>
        /// <exception cref="UriFormatException">The format of the parameter is not a URI.</exception>
        /// <exception cref="ArgumentException">
        ///     The path parameter contains invalid characters, is empty, or contains only white
        ///     spaces.
        /// </exception>
        /// <exception cref="PathTooLongException">The path parameter is longer than the system-defined maximum length.</exception>
        /// <exception cref="DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive). </exception>
        /// <exception cref="IOException">
        ///     The specified file is in use. -or-There is an open handle on the file, and the operating
        ///     system is Windows XP or earlier. This open handle can result from enumerating directories and files. For more
        ///     information, see How to: Enumerate Directories and Files.
        /// </exception>
        /// <exception cref="NotSupportedException">
        ///     Path is in an invalid format.
        /// </exception>
        /// <exception cref="UnauthorizedAccessException">
        ///     The caller does not have the required permission.-or- The file is an
        ///     executable file that is in use.-or- path is a directory.-or- path specified a read-only file.
        /// </exception>
        public async void DestroyDatabase()
        {
            DatabaseWrapper.Destroy(await GetDatabaseLocation());
        }

        /// <inheritdoc />
        /// <summary>
        ///     Disposes of the DatabaseWrapper.
        /// </summary>
        /// <exception cref="ArgumentNullException">The parameter is null. </exception>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Executes the given sql statement with the given parameter list.
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public long ExecuteCountQueryWithParameters(string sql, List<SQLiteParameter> parameters) => DatabaseWrapper.ExecuteCountQueryWithParameters(sql, parameters);

        /// <summary>
        ///     Executes the given Query statement with the given parameter list.
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns>An ICursor containing the fetched data</returns>
        /// <exception cref="ArgumentNullException">The parameter is null. </exception>
        public async Task<ICursor> ExecuteQueryWithParametersAsync(string sql, List<SQLiteParameter> parameters)
        {
            return await Task.Run(() => DatabaseWrapper.ExecuteQueryWithParameters(sql, parameters));
        }

        /// <summary>
        ///     Executes the given sql statement with the given parameter list.
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<int> ExecuteSqlWithParametersAsync(string sql, List<SQLiteParameter> parameters) => await DatabaseWrapper.ExecuteSqlWithParametersAsync(sql, parameters);

        /// <summary>
        ///     Opens the database associated with this user. If such a database does not exist, the file is created.
        /// </summary>
        /// <exception cref="ArgumentNullException">The parameter is null. </exception>
        /// <exception cref="UriFormatException">The format of the parameter is not a URI.</exception>
        /// <exception cref="ArgumentException">
        ///     The path parameter contains invalid characters, is empty, or contains only white
        ///     spaces.
        /// </exception>
        /// <exception cref="PathTooLongException">The path parameter is longer than the system-defined maximum length.</exception>
        /// <exception cref="MissingMethodException">
        ///     In the .NET for Windows Store apps or the Portable Class Library, catch the
        ///     base class exception, <see cref="T:System.MissingMemberException" />, instead.
        ///     The type that is specified does not have a parameterless constructor.
        /// </exception>
        public async Task<DatabaseState> OpenOrCreateDatabaseAsync()
        {
            // Check if the database file exists before trying to open.
            var newDatabase = !await DatabaseFileExistsAsync();

            // If the database doesn't exist, it will be created automatically.
            await DatabaseWrapper.OpenAsync(await GetDatabaseLocation());

            if (newDatabase)
            {
                await CreateDatabaseTablesAsync();
                return DatabaseState.New;
            }

            return DatabaseState.Existing;
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Creates the tables in the database.
        /// </summary>
        /// <exception cref="ArgumentNullException">The parameter was null.</exception>
        /// <exception cref="MissingMethodException">
        ///     In the .NET for Windows Store apps or the Portable Class Library, catch the
        ///     base class exception, <see cref="T:System.MissingMemberException" />, instead.
        ///     The type that is specified does not have a parameterless constructor.
        /// </exception>
        private async Task CreateDatabaseTablesAsync()
        {
            await DatabaseWrapper.ExecuteSqlWithParametersAsync(TableFactory<Album>.GetTable<AlbumTable>().GetCreateSql(), null);
            await DatabaseWrapper.ExecuteSqlWithParametersAsync(TableFactory<Artist>.GetTable<ArtistTable>().GetCreateSql(), null);
            await DatabaseWrapper.ExecuteSqlWithParametersAsync(TableFactory<Genre>.GetTable<GenreTable>().GetCreateSql(), null);
        }

        /// <summary>
        ///     Checks that the database file exists within the Obfuscated File System.
        /// </summary>
        /// <exception cref="ArgumentNullException">The parameter is null. </exception>
        /// <exception cref="UriFormatException">The format of the parameter is not a URI.</exception>
        /// <exception cref="ArgumentException">
        ///     The path parameter contains invalid characters, is empty, or contains only white
        ///     spaces.
        /// </exception>
        /// <exception cref="PathTooLongException">The path parameter is longer than the system-defined maximum length.</exception>
        private async Task<bool> DatabaseFileExistsAsync() => File.Exists(await GetDatabaseLocation());

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                DatabaseWrapper.Dispose();
            }
        }

        /// <summary>
        ///     Gets the location of the database
        /// </summary>
        /// <exception cref="ArgumentNullException">The parameter is null. </exception>
        /// <exception cref="UriFormatException">The format of the parameter is not a URI.</exception>
        /// <exception cref="ArgumentException">
        ///     The path parameter contains invalid characters, is empty, or contains only white
        ///     spaces.
        /// </exception>
        /// <exception cref="PathTooLongException">The path parameter is longer than the system-defined maximum length.</exception>
        /// <returns>The location of the database or an empty string if the directory could not be found.</returns>
        private Task<string> GetDatabaseLocation()
        {
            return Task.Run(() =>
                            {
                                var dirName = Path.GetDirectoryName(Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Path));
                                return dirName != null ? Path.Combine(dirName, DatabaseName) : string.Empty;
                            });
        }

        #endregion
    }
}