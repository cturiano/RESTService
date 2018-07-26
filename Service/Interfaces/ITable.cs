using System.Collections.Generic;
using System.Data.SQLite;
using Service.DAL;

namespace Service.Interfaces
{
    internal interface ITable<in T>
    {
        #region Public Methods

        Dictionary<string, int> GetColumnNameIndexMap();

        string GetCountSql(IdentifyingInfo value, ref List<SQLiteParameter> parameters);

        string GetCreateSql();

        string GetDeleteSql(IdentifyingInfo value, ref List<SQLiteParameter> parameters);

        string GetFetchSql(IdentifyingInfo value, ref List<SQLiteParameter> parameters);

        string GetInsertSql(T value, ref List<SQLiteParameter> parameters);

        string GetUpdateSql(T value, ref List<SQLiteParameter> parameters);

        #endregion
    }
}