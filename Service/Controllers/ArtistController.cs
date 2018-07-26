using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Service.Abstract;
using Service.DAL;
using Service.DAL.Tables;
using Service.Interfaces;
using Service.Models;

namespace Service.Controllers
{
    [RoutePrefix("api/artist")]
    public class ArtistController : AbstractController<Artist>
    {
        #region Constructors

        public ArtistController(IDataAccess dataAccess) : base(dataAccess) => Table = new ArtistTable();

        #endregion

        #region Public Methods

        /// <inheritdoc />
        public override async Task DeleteAsync(int id)
        {
            await Task.Run(async () =>
                           {
                               var parameters = new List<SQLiteParameter>();

                               // remove all albums where the artist_id == id
                               var sql = (Table as ArtistTable)?.GetDeleteAlbumsByArtistSql(new IdentifyingInfo(id), ref parameters);
                               await DataAccess.ExecuteSqlWithParametersAsync(sql, parameters);
                               parameters.Clear();

                               sql = Table.GetDeleteSql(new IdentifyingInfo(id), ref parameters);
                               await DataAccess.ExecuteSqlWithParametersAsync(sql, parameters);
                           });
        }

        /// <inheritdoc />
        public override async Task DeleteAsync(string name)
        {
            await Task.Run(async () =>
                           {
                               var parameters = new List<SQLiteParameter>();

                               // get the id of the artist wit the given name
                               if (Table is ArtistTable artistTable)
                               {
                                   int id;
                                   var sql = artistTable.GetIdFromNameSql(new IdentifyingInfo(name: name), ref parameters);
                                   using (var cursor = await DataAccess.ExecuteQueryWithParametersAsync(sql, parameters))
                                   {
                                       if (!Cursor.IsNullOrEmpty(cursor))
                                       {
                                           id = cursor.GetInt(cursor.GetColumnIndex(ArtistTable.IdColumnName));
                                       }
                                       else
                                       {
                                           throw new SQLiteException($"The artist with name '{name}' was not found in the database.");
                                       }
                                   }

                                   parameters.Clear();

                                   // remove all albums where the artist_id == id
                                   sql = artistTable.GetDeleteAlbumsByArtistSql(new IdentifyingInfo(id), ref parameters);
                                   await DataAccess.ExecuteSqlWithParametersAsync(sql, parameters);
                                   parameters.Clear();

                                   sql = Table.GetDeleteSql(new IdentifyingInfo(name: name), ref parameters);
                                   await DataAccess.ExecuteSqlWithParametersAsync(sql, parameters);
                               }
                           });
        }

        /// <summary>
        ///     Gets all the albums by the artist with the given id
        /// </summary>
        /// <param name="id">The id of the item to fetch</param>
        /// <returns>The albums of the artist with the given id</returns>
        // GET: api/<T>/5/albums
        [AcceptVerbs("GET")]
        [HttpGet]
        [Route("{id:int}/albums")]
        public async Task<List<Album>> GetAlbumsAsync(int id)
        {
            return await Task.Run(async () =>
                                  {
                                      List<Album> items = null;

                                      if (Table is ArtistTable artistTable)
                                      {
                                          items = new List<Album>();
                                          var albumController = new AlbumController(DataAccess);
                                          var albumTable = new AlbumTable();
                                          var parameters = new List<SQLiteParameter>();
                                          var sql = artistTable.GetAlbumsByArtistSql(new IdentifyingInfo(id), ref parameters);
                                          using (var cursor = await DataAccess.ExecuteQueryWithParametersAsync(sql, parameters))
                                          {
                                              if (!Cursor.IsNullOrEmpty(cursor))
                                              {
                                                  var indexMap = albumTable.GetColumnNameIndexMap();
                                                  var keys = indexMap.Keys.ToList();
                                                  foreach (var key in keys)
                                                  {
                                                      indexMap[key] = cursor.GetColumnIndex(key);
                                                  }

                                                  do
                                                  {
                                                      items.Add(await albumController.MakeItemAsync(cursor, indexMap));
                                                  } while (cursor.MoveToNextRow());
                                              }
                                          }
                                      }

                                      return items;
                                  });
        }

        /// <summary>
        ///     Gets all the albums by the artist with the given id
        /// </summary>
        /// <param name="name">The name of the item to fetch</param>
        /// <returns>The albums of the artist with the given name</returns>
        // GET: api/<T>/oasis/albums
        [AcceptVerbs("GET")]
        [HttpGet]
        [Route("{name:alpha}/albums")]
        public async Task<List<Album>> GetAlbumsAsync(string name)
        {
            return await Task.Run(async () =>
                                  {
                                      List<Album> items = null;
                                      var parameters = new List<SQLiteParameter>();

                                      if (Table is ArtistTable artistTable)
                                      {
                                          int id;
                                          var sql = artistTable.GetIdFromNameSql(new IdentifyingInfo(name: name), ref parameters);
                                          using (var cursor = await DataAccess.ExecuteQueryWithParametersAsync(sql, parameters))
                                          {
                                              if (!Cursor.IsNullOrEmpty(cursor))
                                              {
                                                  id = cursor.GetInt(cursor.GetColumnIndex(ArtistTable.IdColumnName));
                                              }
                                              else
                                              {
                                                  throw new SQLiteException($"The artist with name '{name}' was not found in the database.");
                                              }
                                          }

                                          items = await GetAlbumsAsync(id);
                                      }

                                      return items;
                                  });
        }

        /// <inheritdoc />
        public override async Task<Artist> MakeItemAsync(ICursor cursor, Dictionary<string, int> indexMap)
        {
            return await Task.Run(() =>
                                  {
                                      var id = cursor.GetInt(indexMap[ArtistTable.IdColumnName]);
                                      var name = cursor.GetString(indexMap[ArtistTable.NameColumnName]);

                                      return new Artist(id, name);
                                  });
        }

        #endregion
    }
}