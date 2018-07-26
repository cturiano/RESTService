using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Text;
using Service.DAL;
using Service.Extensions;
using Service.Interfaces;
using Service.Models;
using Service.Properties;

namespace Service.Abstract
{
    public abstract class AbstractTable<T> : ITable<T> where T : BaseModel
    {
        #region Static Fields and Constants

        internal const string IdColumnName = "id";
        internal const string NameColumnName = "name";

        #endregion

        #region Properties

        public string TableName { get; protected set; }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Creates a dictionary whose keys are the names of the columns
        /// </summary>
        /// <returns></returns>
        public virtual Dictionary<string, int> GetColumnNameIndexMap() => new Dictionary<string, int>
                                                                          {
                                                                              [IdColumnName] = 0,
                                                                              [NameColumnName] = 0
                                                                          };

        /// <summary>
        ///     Creates a SQL query string to fetch the count of rows based on criteria relevant to the type of value.
        /// </summary>
        /// <param name="value">The value to use for data or criteria.</param>
        /// <param name="parameters">
        ///     A <see cref="List{T}" /> of <see cref="SQLiteParameter" /> populated with the values need to
        ///     complete the query.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Enlarging the value of this instance would exceed
        ///     <see cref="P:System.Text.StringBuilder.MaxCapacity" />.
        /// </exception>
        public virtual string GetCountSql(IdentifyingInfo value, ref List<SQLiteParameter> parameters)
        {
            if (value.Id.HasValue)
            {
                var idParameter = SqliteExtensions.Create(IdColumnName, DbType.Int32, value.Id.Value);
                parameters.Add(idParameter);
            }
            
            if (!value.Name.IsNullOrEmpty())
            {
                var nameParameter = SqliteExtensions.Create(NameColumnName, DbType.String, value.Name);
                parameters.Add(nameParameter);
            }

            return $"SELECT COUNT(*) FROM {TableName} {GenerateWhereString(GenerateWhereClauses(parameters))};";
        }

        /// <summary>
        ///     Creates a SQL query string to create the database table.
        /// </summary>
        public virtual string GetCreateSql() => $"CREATE TABLE {TableName} ({IdColumnName} INTEGER NOT NULL PRIMARY KEY, {NameColumnName} TEXT UNIQUE NOT NULL COLLATE NOCASE) WITHOUT ROWID;";

        /// <summary>
        ///     Creates a SQL query string to delete the rows meeting the criteria relevant to the type of value.
        /// </summary>
        /// <param name="value">The value to use for data or criteria.</param>
        /// <param name="parameters">
        ///     A <see cref="List{T}" /> of <see cref="SQLiteParameter" /> populated with the values need to
        ///     complete the query.
        /// </param>
        /// <exception cref="ArgumentException">Thrown if the id and name are both null.</exception>
        public virtual string GetDeleteSql(IdentifyingInfo value, ref List<SQLiteParameter> parameters)
        {
            if (value.Id.HasValue)
            {
                var idParameter = SqliteExtensions.Create(IdColumnName, DbType.Int32, value.Id.Value);
                parameters.Add(idParameter);
            }
            else if (!value.Name.IsNullOrEmpty())
            {
                var idParameter = SqliteExtensions.Create(NameColumnName, DbType.String, value.Name);
                parameters.Add(idParameter);
            }
            else
            {
                throw new ArgumentException(Resources.IdOrNameNotProvided, nameof(value));
            }

            return $"DELETE FROM {TableName} {GenerateWhereString(GenerateWhereClauses(parameters))};";
        }

        /// <summary>
        ///     Creates a SQL query string to fetch a value from the database table based on criteria relevant to the type of
        ///     value.
        /// </summary>
        /// <param name="value">The value to use for data or criteria.</param>
        /// <param name="parameters">
        ///     A <see cref="List{T}" /> of <see cref="SQLiteParameter" /> populated with the values need to
        ///     complete the query.
        /// </param>
        public virtual string GetFetchSql(IdentifyingInfo value, ref List<SQLiteParameter> parameters)
        {
            if (value.Id.HasValue)
            {
                var idParameter = SqliteExtensions.Create(IdColumnName, DbType.Int32, value.Id.Value);
                parameters.Add(idParameter);
            }
            else if (!value.Name.IsNullOrEmpty())
            {
                var nameParameter = SqliteExtensions.Create(NameColumnName, DbType.String, value.Name);
                parameters.Add(nameParameter);
            }

            return $"SELECT * FROM {TableName} {GenerateWhereString(GenerateWhereClauses(parameters))};";
        }

        /// <summary>
        ///     Creates a SQL query string to fetch the id of an item from its name
        /// </summary>
        /// <param name="value">The value to use for data or criteria.</param>
        /// <param name="parameters">
        ///     A <see cref="List{T}" /> of <see cref="SQLiteParameter" /> populated with the values need to
        ///     complete the query.
        /// </param>
        public virtual string GetIdFromNameSql(IdentifyingInfo value, ref List<SQLiteParameter> parameters)
        {
            SQLiteParameter nameParameter;
            if (!value.Name.IsNullOrEmpty())
            {
                nameParameter = SqliteExtensions.Create(NameColumnName, DbType.String, value.Name);
                parameters.Add(nameParameter);
            }
            else
            {
                throw new ArgumentException(Resources.IdOrNameNotProvided, nameof(value));
            }

            return $"SELECT {IdColumnName} FROM {TableName} WHERE {NameColumnName} = {nameParameter.ParameterName};";
        }

        /// <summary>
        ///     Creates a SQL query string to insert the value into the database table.
        /// </summary>
        /// <param name="value">The value to use for data or criteria.</param>
        /// <param name="parameters">
        ///     A <see cref="List{T}" /> of <see cref="SQLiteParameter" /> populated with the values need to
        ///     complete the query.
        /// </param>
        public virtual string GetInsertSql(T value, ref List<SQLiteParameter> parameters)
        {
            var idParameter = SqliteExtensions.Create(IdColumnName, DbType.Int32, value.Id);
            var nameParameter = SqliteExtensions.Create(NameColumnName, DbType.String, value.Name);
            parameters.Add(idParameter);
            parameters.Add(nameParameter);

            return $"INSERT OR IGNORE INTO {TableName} ({IdColumnName}, {NameColumnName}) VALUES({idParameter.ParameterName}, {nameParameter.ParameterName});";
        }

        /// <summary>
        ///     Creates a SQL query string to update the value in the database table based on criteria relevant to the type of
        ///     value.
        /// </summary>
        /// <param name="value">The value to use for data or criteria.</param>
        /// <param name="parameters">
        ///     A <see cref="List{T}" /> of <see cref="SQLiteParameter" /> populated with the values need to
        ///     complete the query.
        /// </param>
        public virtual string GetUpdateSql(T value, ref List<SQLiteParameter> parameters)
        {
            var idParameter = SqliteExtensions.Create(IdColumnName, DbType.Int32, value.Id);
            var nameParameter = SqliteExtensions.Create(NameColumnName, DbType.String, value.Name);
            parameters.Add(idParameter);
            parameters.Add(nameParameter);

            var clauseParameters = new List<SQLiteParameter>
                                   {
                                       idParameter
                                   };

            return $"UPDATE {TableName} SET {NameColumnName} = {nameParameter.ParameterName} {GenerateWhereString(GenerateWhereClauses(clauseParameters))};";
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Creates a list of AND-ed where clauses based on the givne list of <see cref="SQLiteParameter" />
        /// </summary>
        /// <param name="parameters">The potentially empty <see cref="List{T}" /> of <see cref="SQLiteParameter" /></param>
        /// <returns></returns>
        protected List<SqlWhereClause> GenerateWhereClauses(List<SQLiteParameter> parameters)
        {
            if (!parameters.IsNullOrEmpty())
            {
                var clauses = new List<SqlWhereClause>();
                foreach (var parameter in parameters)
                {
                    clauses.Add(new SqlWhereClause(Conjunction.And, parameter.ParameterName.TrimStart('@'), Operator.Equal, parameter.ParameterName));
                }

                return clauses;
            }

            return null;
        }

        /// <summary>
        ///     Creates a string out of the list of clauses.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Enlarging the value of this instance would exceed
        ///     <see cref="P:System.Text.StringBuilder.MaxCapacity" />.
        /// </exception>
        protected string GenerateWhereString(List<SqlWhereClause> clauses)
        {
            if (!clauses.IsNullOrEmpty())
            {
                var sb = new StringBuilder("WHERE ");
                for (var i = 0; i < clauses.Count; i++)
                {
                    var clause = clauses[i];

                    // get rid of the conjunction on the first clause
                    if (i == 0)
                    {
                        clause.DumpConjunction();
                    }

                    sb.Append(clause);

                    // add a space to the back of all but the last clause
                    if (i < clauses.Count - 1)
                    {
                        sb.Append(' ');
                    }
                }

                return sb.ToString();
            }

            return null;
        }

        #endregion
    }
}