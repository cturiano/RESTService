using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using Service.Abstract;
using Service.Extensions;
using Service.Models;
using Service.Properties;

namespace Service.DAL.Tables
{
    internal class ArtistTable : AbstractTable<Artist>
    {
        #region Fields

        private readonly AlbumTable _albumTable;

        #endregion

        #region Constructors

        public ArtistTable()
        {
            TableName = "artists";
            _albumTable = new AlbumTable();
        }

        #endregion

        #region Public Methods

        public string GetAlbumsByArtistSql(IdentifyingInfo value, ref List<SQLiteParameter> parameters)
        {
            SQLiteParameter idParameter;
            if (value.Id.HasValue)
            {
                idParameter = SqliteExtensions.Create(AlbumTable.ArtistIdColumnName, DbType.Int32, value.Id.Value);
                parameters.Add(idParameter);
            }
            else
            {
                throw new ArgumentException(Resources.IdOrNameNotProvided, nameof(value));
            }

            return $"SELECT * FROM {_albumTable.TableName} WHERE {AlbumTable.ArtistIdColumnName} = {idParameter.ParameterName};";
        }

        public string GetDeleteAlbumsByArtistSql(IdentifyingInfo value, ref List<SQLiteParameter> parameters)
        {
            SQLiteParameter idParameter;
            if (value.Id.HasValue)
            {
                idParameter = SqliteExtensions.Create(AlbumTable.ArtistIdColumnName, DbType.Int32, value.Id.Value);
                parameters.Add(idParameter);
            }
            else
            {
                throw new ArgumentException(Resources.IdOrNameNotProvided, nameof(value));
            }

            return $"DELETE FROM {_albumTable.TableName} WHERE {AlbumTable.ArtistIdColumnName} = {idParameter.ParameterName};";
        }

        #endregion
    }
}