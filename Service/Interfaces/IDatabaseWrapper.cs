using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IDatabaseWrapper : IDisposable
    {
        #region Properties

        bool IsOpen { get; }

        #endregion

        #region Public Methods

        void BeginTransaction();

        void Close();

        void CommitTransaction();

        void Destroy(string location);

        Task DropTableAsync(string tableName);

        long ExecuteCountQuery(string countSql);

        long ExecuteCountQueryWithParameters(string countSql, List<SQLiteParameter> parameters);

        ICursor ExecuteQuery(string sql);

        ICursor ExecuteQueryWithParameters(string sql, List<SQLiteParameter> parameters, bool singleResult = false);

        ICursor ExecuteSingleResultQuery(string sql);

        /// <summary>
        ///     Execute non query sql.
        /// </summary>
        /// <param name="sql">sql to execute</param>
        /// <returns>the number of rows inserted/updated affected by it</returns>
        int ExecuteSql(string sql);

        int ExecuteSqlWithParameters(string sql, List<SQLiteParameter> parameters);

        Task<int> ExecuteSqlWithParametersAsync(string sql, List<SQLiteParameter> parameters);

        List<string> GetColumnNames(string tableName);

        int GetVersion();

        bool HasOneRecordAndClose(ICursor cursor);

        Task OpenAsync(string location);

        void RollBackTransaction();

        Task SetVersionAsync(int version);

        bool TableExists(string tableName);

        #endregion
    }
}