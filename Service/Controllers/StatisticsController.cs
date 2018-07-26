using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Threading.Tasks;
using System.Web.Http;
using Service.Abstract;
using Service.DAL;
using Service.DAL.Tables;
using Service.Interfaces;
using Service.Models;

namespace Service.Controllers
{
    [RoutePrefix("api/stats")]
    public class StatisticsController : ApiController
    {
        #region Fields

        private readonly AbstractTable<Album> _albumTable;
        private readonly IDataAccess _dataAccess;
        private readonly AbstractTable<Genre> _genreTable;

        #endregion

        #region Static Fields and Constants

        private const string CountColumnName = "count";

        #endregion

        #region Constructors

        public StatisticsController(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
            _albumTable = new AlbumTable();
            _genreTable = new GenreTable();
        }

        ~StatisticsController()
        {
            Dispose(false);
        }

        #endregion

        #region Public Methods

        public new void Dispose()
        {
            base.Dispose();
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Gets the number of albums in each genre
        /// </summary>
        /// <returns>IEnumerable of the number of albums in each genre</returns>
        // GET: api/stats/genre
        [AcceptVerbs("GET")]
        [HttpGet]
        [Route("genre")]
        public async Task<Dictionary<string, int>> GetCountOfAlbumsPerGenreAsync()
        {
            return await Task.Run(async () =>
                                  {
                                      var values = new Dictionary<string, int>();
                                      var parameters = new List<SQLiteParameter>();
                                      var sql = $"SELECT genreTable.{GenreTable.NameColumnName}, COUNT(*) AS '{CountColumnName}' FROM {_albumTable.TableName} albumTable INNER JOIN {_genreTable.TableName} genreTable ON albumTable.{AlbumTable.GenreIdColumnName} = genreTable.{GenreTable.IdColumnName} GROUP BY genreTable.{GenreTable.NameColumnName} ORDER BY '{CountColumnName}';";
                                      using (var cursor = await _dataAccess.ExecuteQueryWithParametersAsync(sql, parameters))
                                      {
                                          if (!Cursor.IsNullOrEmpty(cursor))
                                          {
                                              var genreIndex = cursor.GetColumnIndex(GenreTable.NameColumnName);
                                              var countIndex = cursor.GetColumnIndex(CountColumnName);

                                              do
                                              {
                                                  values[cursor.GetString(genreIndex)] = cursor.GetInt(countIndex);
                                              } while (cursor.MoveToNextRow());
                                          }
                                      }

                                      return values;
                                  });
        }

        /// <summary>
        ///     Gets the number of albums in each year
        /// </summary>
        /// <returns>IEnumerable of the number of albums in each year</returns>
        // GET: api/stats/year
        [AcceptVerbs("GET")]
        [HttpGet]
        [Route("year")]
        public async Task<Dictionary<int, int>> GetCountofAlbumsPerYearAsync()
        {
            return await Task.Run(async () =>
                                  {
                                      var values = new Dictionary<int, int>();
                                      var parameters = new List<SQLiteParameter>();
                                      var sql = $"SELECT {AlbumTable.YearColumnName}, COUNT(*) AS '{CountColumnName}' FROM {_albumTable.TableName} GROUP BY {AlbumTable.YearColumnName} ORDER BY '{CountColumnName}';";
                                      using (var cursor = await _dataAccess.ExecuteQueryWithParametersAsync(sql, parameters))
                                      {
                                          if (!Cursor.IsNullOrEmpty(cursor))
                                          {
                                              var yearIndex = cursor.GetColumnIndex(AlbumTable.YearColumnName);
                                              var countIndex = cursor.GetColumnIndex(CountColumnName);

                                              do
                                              {
                                                  values[cursor.GetInt(yearIndex)] = cursor.GetInt(countIndex);
                                              } while (cursor.MoveToNextRow());
                                          }
                                      }

                                      return values;
                                  });
        }

        #endregion

        #region Private Methods

        private new void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                _dataAccess.Dispose();
            }
        }

        #endregion
    }
}