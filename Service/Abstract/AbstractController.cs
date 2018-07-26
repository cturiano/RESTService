using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Service.DAL;
using Service.Interfaces;
using Service.Models;

namespace Service.Abstract
{
    public abstract class AbstractController<T> : ApiController where T : BaseModel
    {
        #region Fields

        protected readonly IDataAccess DataAccess;

        protected AbstractTable<T> Table;

        #endregion

        #region Constructors

        protected AbstractController(IDataAccess dataAccess) => DataAccess = dataAccess;

        ~AbstractController()
        {
            Dispose(false);
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Deletes the item with the given id
        /// </summary>
        /// <param name="id">The id of the item to delete</param>
        // DELETE: api/<T>/5
        [AcceptVerbs("DELETE")]
        [HttpDelete]
        [Route("{id:int}")]
        public virtual async Task DeleteAsync(int id)
        {
            await Task.Run(async () =>
                           {
                               var parameters = new List<SQLiteParameter>();
                               var sql = Table.GetDeleteSql(new IdentifyingInfo(id), ref parameters);
                               await DataAccess.ExecuteSqlWithParametersAsync(sql, parameters);
                           });
        }

        /// <summary>
        ///     Deletes the item with the given title
        /// </summary>
        /// <param name="name">The name of the item to delete</param>
        // DELETE: api/<T>/<title>
        [AcceptVerbs("DELETE")]
        [HttpDelete]
        [Route("{name:alpha}")]
        public virtual async Task DeleteAsync(string name)
        {
            await Task.Run(async () =>
                           {
                               var parameters = new List<SQLiteParameter>();
                               var sql = Table.GetDeleteSql(new IdentifyingInfo(name: name), ref parameters);
                               await DataAccess.ExecuteSqlWithParametersAsync(sql, parameters);
                           });
        }

        public new void Dispose()
        {
            base.Dispose();
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Gets all items in database
        /// </summary>
        /// <returns>IEnumerable of all items of this type in the database</returns>
        // GET: api/<T>
        [AcceptVerbs("GET")]
        [HttpGet]
        [Route("")]
        public async Task<IEnumerable<T>> GetAsync()
        {
            return await Task.Run(async () =>
                                  {
                                      var items = new List<T>();
                                      var parameters = new List<SQLiteParameter>();
                                      var sql = Table.GetFetchSql(new IdentifyingInfo(), ref parameters);
                                      using (var cursor = await DataAccess.ExecuteQueryWithParametersAsync(sql, parameters))
                                      {
                                          if (!Cursor.IsNullOrEmpty(cursor))
                                          {
                                              var indexMap = Table.GetColumnNameIndexMap();
                                              var keys = indexMap.Keys.ToList();
                                              foreach (var key in keys)
                                              {
                                                  indexMap[key] = cursor.GetColumnIndex(key);
                                              }

                                              do
                                              {
                                                  items.Add(await MakeItemAsync(cursor, indexMap));
                                              } while (cursor.MoveToNextRow());
                                          }
                                      }

                                      return items;
                                  });
        }

        /// <summary>
        ///     Gets the item with the given id
        /// </summary>
        /// <param name="id">The id of the item to fetch</param>
        /// <returns>The item with the given id</returns>
        // GET: api/<T>/5
        [AcceptVerbs("GET")]
        [HttpGet]
        [Route("{id:int}")]
        public async Task<T> GetAsync(int id)
        {
            return await Task.Run(async () =>
                                  {
                                      T item = null;
                                      var parameters = new List<SQLiteParameter>();
                                      var sql = Table.GetFetchSql(new IdentifyingInfo(id), ref parameters);
                                      using (var cursor = await DataAccess.ExecuteQueryWithParametersAsync(sql, parameters))
                                      {
                                          if (!Cursor.IsNullOrEmpty(cursor))
                                          {
                                              var indexMap = Table.GetColumnNameIndexMap();
                                              var keys = indexMap.Keys.ToList();
                                              foreach (var key in keys)
                                              {
                                                  indexMap[key] = cursor.GetColumnIndex(key);
                                              }

                                              item = await MakeItemAsync(cursor, indexMap);
                                          }
                                      }

                                      return item;
                                  });
        }

        /// <summary>
        ///     Gets the item with the given name
        /// </summary>
        /// <param name="name">The name of the item to fetch</param>
        /// <returns>The item with the given name</returns>
        // GET: api/<T>/<item name>
        [AcceptVerbs("GET")]
        [HttpGet]
        [Route("{name:alpha}")]
        public async Task<T> GetAsync(string name)
        {
            return await Task.Run(async () =>
                                  {
                                      T item = null;
                                      var parameters = new List<SQLiteParameter>();
                                      var sql = Table.GetFetchSql(new IdentifyingInfo(name: name), ref parameters);
                                      using (var cursor = await DataAccess.ExecuteQueryWithParametersAsync(sql, parameters))
                                      {
                                          if (!Cursor.IsNullOrEmpty(cursor))
                                          {
                                              var indexMap = Table.GetColumnNameIndexMap();
                                              var keys = indexMap.Keys.ToList();
                                              foreach (var key in keys)
                                              {
                                                  indexMap[key] = cursor.GetColumnIndex(key);
                                              }

                                              item = await MakeItemAsync(cursor, indexMap);
                                          }
                                      }

                                      return item;
                                  });
        }

        public abstract Task<T> MakeItemAsync(ICursor cursor, Dictionary<string, int> indexMap);

        /// <summary>
        ///     Adds the given item to the database at the next available id
        /// </summary>
        /// <param name="value">The item to add</param>
        /// <exception cref="SQLiteException">Thrown if the item could not be added to the database for any reason.</exception>
        // POST: api/<T>
        [AcceptVerbs("POST")]
        [HttpPost]
        public async Task PostAsync([FromBody] T value)
        {
            await Task.Run(async () =>
                           {
                               var parameters = new List<SQLiteParameter>();
                               var sql = Table.GetInsertSql(value, ref parameters);
                               var rowsAffected = await DataAccess.ExecuteSqlWithParametersAsync(sql, parameters);

                               if (rowsAffected == 0)
                               {
                                   throw new SQLiteException($"Could not insert item: {value}.");
                               }
                           });
        }

        /// <summary>
        ///     Updates or creates the item in the database at the given id
        /// </summary>
        /// <param name="id">The id of the item to update</param>
        /// <param name="value">The item to update</param>
        /// <exception cref="SQLiteException">Thrown if the item could not be added to the database for any reason.</exception>
        // PUT: api/<T>/5
        [AcceptVerbs("PUT")]
        [HttpPut]
        public async Task PutAsync(int id, [FromBody] T value)
        {
            await Task.Run(async () =>
                           {
                               var parameters = new List<SQLiteParameter>();
                               var sql = Table.GetUpdateSql(value, ref parameters);
                               var rowsAffected = await DataAccess.ExecuteSqlWithParametersAsync(sql, parameters);

                               if (rowsAffected == 0)
                               {
                                   await PostAsync(value);
                               }
                           });
        }

        #endregion

        #region Private Methods

        private new void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                DataAccess.Dispose();
            }
        }

        #endregion
    }
}