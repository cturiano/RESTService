using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Threading.Tasks;
using Service.DAL;

namespace Service.Interfaces
{
    public interface IDataAccess : IDisposable
    {
        #region Public Methods

        void CloseDatabase();

        void DestroyDatabase();

        long ExecuteCountQueryWithParameters(string sql, List<SQLiteParameter> parameters);

        Task<ICursor> ExecuteQueryWithParametersAsync(string sql, List<SQLiteParameter> parameters);

        Task<int> ExecuteSqlWithParametersAsync(string sql, List<SQLiteParameter> parameters);

        Task<DatabaseState> OpenOrCreateDatabaseAsync();

        #endregion
    }
}