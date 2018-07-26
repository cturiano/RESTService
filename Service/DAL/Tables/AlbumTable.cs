using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using Service.Abstract;
using Service.Extensions;
using Service.Models;

namespace Service.DAL.Tables
{
    internal class AlbumTable : AbstractTable<Album>
    {
        #region Constructors

        public AlbumTable() => TableName = "albums";

        #endregion

        #region Public Methods

        public override Dictionary<string, int> GetColumnNameIndexMap()
        {
            var map = base.GetColumnNameIndexMap();
            map[ArtistIdColumnName] = 0;
            map[GenreIdColumnName] = 0;
            map[YearColumnName] = 0;
            return map;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Creates a SQL query string to create the database table.
        /// </summary>
        public override string GetCreateSql() => $"CREATE TABLE {TableName} ({IdColumnName} INTEGER NOT NULL PRIMARY KEY, {NameColumnName} TEXT UNIQUE NOT NULL COLLATE NOCASE, {YearColumnName} INTEGER NOT NULL, {ArtistIdColumnName} INTEGER NOT NULL, {GenreIdColumnName} INTEGER NOT NULL, FOREIGN KEY({ArtistIdColumnName}) REFERENCES artists({IdColumnName}), FOREIGN KEY({GenreIdColumnName}) REFERENCES genres({IdColumnName})) WITHOUT ROWID;";

        /// <inheritdoc />
        public override string GetInsertSql(Album value, ref List<SQLiteParameter> parameters)
        {
            var idParameter = SqliteExtensions.Create(IdColumnName, DbType.Int32, value.Id);
            var artistIdParameter = SqliteExtensions.Create(ArtistIdColumnName, DbType.Int32, value.ArtistId);
            var genreIdParameter = SqliteExtensions.Create(GenreIdColumnName, DbType.Int32, value.GenreId);
            var nameParameter = SqliteExtensions.Create(NameColumnName, DbType.String, value.Name);
            var yearParameter = SqliteExtensions.Create(YearColumnName, DbType.Int32, value.Year);
            parameters.Add(idParameter);
            parameters.Add(artistIdParameter);
            parameters.Add(genreIdParameter);
            parameters.Add(nameParameter);
            parameters.Add(yearParameter);

            return $"INSERT OR IGNORE INTO {TableName} ({IdColumnName}, {ArtistIdColumnName}, {GenreIdColumnName}, {NameColumnName}, {YearColumnName}) VALUES({idParameter.ParameterName}, {artistIdParameter.ParameterName}, {genreIdParameter.ParameterName}, {nameParameter.ParameterName}, {yearParameter.ParameterName});";
        }

        /// <inheritdoc />
        public override string GetUpdateSql(Album value, ref List<SQLiteParameter> parameters)
        {
            var idParameter = SqliteExtensions.Create(IdColumnName, DbType.Int32, value.Id);
            var artistIdParameter = SqliteExtensions.Create(ArtistIdColumnName, DbType.Int32, value.ArtistId);
            var genreIdParameter = SqliteExtensions.Create(GenreIdColumnName, DbType.Int32, value.GenreId);
            var nameParameter = SqliteExtensions.Create(NameColumnName, DbType.String, value.Name);
            var yearParameter = SqliteExtensions.Create(YearColumnName, DbType.Int32, value.Year);
            parameters.Add(idParameter);
            parameters.Add(artistIdParameter);
            parameters.Add(genreIdParameter);
            parameters.Add(nameParameter);
            parameters.Add(yearParameter);

            var clauseParameters = new List<SQLiteParameter>
                                   {
                                       idParameter
                                   };

            return $"UPDATE {TableName} SET {ArtistIdColumnName} = {artistIdParameter.ParameterName}, {GenreIdColumnName} = {genreIdParameter.ParameterName}, {NameColumnName} = {nameParameter.ParameterName}, {YearColumnName} = {yearParameter.ParameterName} {GenerateWhereString(GenerateWhereClauses(clauseParameters))};";
        }

        #endregion

        #region Statics And Constants

        internal const string ArtistIdColumnName = "artist_id";
        internal const string GenreIdColumnName = "genre_id";
        internal const string YearColumnName = "year";

        #endregion
    }
}