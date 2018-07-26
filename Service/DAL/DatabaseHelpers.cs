using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Service.Abstract;
using Service.DAL.Tables;
using Service.Interfaces;
using Service.Models;
using Service.Properties;
using Unity;

namespace Service.DAL
{
    public static class DatabaseHelpers
    {
        #region Public Methods

        public static async Task InitializeDatabase(IUnityContainer container)
        {
            var dataAccess = container.Resolve<IDataAccess>(Resources.DataAccessObjectName);

            await BackgroundTaskScheduler.QueueBackgroundWorkItem(async ct =>
                                                                  {
                                                                      if (DatabaseState.New == await dataAccess.OpenOrCreateDatabaseAsync())
                                                                      {
                                                                          await PopulateDatabase(container, dataAccess);
                                                                      }
                                                                  });
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Gets the id of an item that already exists in the database.
        /// </summary>
        /// <typeparam name="T">The type of the BaseModel we're looking for.</typeparam>
        /// <param name="table">The table to use to create the SQL statements.</param>
        /// <param name="name">The name of the item to look for.</param>
        /// <param name="dataAccess">The access to the database.</param>
        /// <returns></returns>
        private static async Task<int> GetExistingId<T>(AbstractTable<T> table, string name, IDataAccess dataAccess) where T : BaseModel
        {
            var parameters = new List<SQLiteParameter>();
            using (var cursor = await dataAccess.ExecuteQueryWithParametersAsync(table.GetIdFromNameSql(new IdentifyingInfo(name: name), ref parameters), parameters))
            {
                if (!Cursor.IsNullOrEmpty(cursor))
                {
                    return cursor.GetInt(0);
                }

                return -1;
            }
        }

        private static async Task PopulateDatabase(IUnityContainer container, IDataAccess dataAccess)
        {
            await BackgroundTaskScheduler.QueueBackgroundWorkItem(async ct =>
                                                                  {
                                                                      var i = 0;
                                                                      var artistTable = container.Resolve<ArtistTable>();
                                                                      var genreTable = container.Resolve<GenreTable>();
                                                                      var albumTable = container.Resolve<AlbumTable>();

                                                                      var dirName = Path.GetDirectoryName(Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Path));
                                                                      if (dirName != null)
                                                                      {
                                                                          var dataFilePath = Path.Combine(dirName, Resources.DataFile);
                                                                          var lines = File.ReadAllLines(dataFilePath);
                                                                          var lineCount = lines.Length - 1;

                                                                          foreach (var line in lines.Skip(1))
                                                                          {
                                                                              // album name, artist name, genre name, year
                                                                              // album name might have quotes
                                                                              string album;
                                                                              string artist;
                                                                              string genre;
                                                                              int year;

                                                                              var parts = line.Split('"');
                                                                              if (parts.Length > 1)
                                                                              {
                                                                                  album = parts[1];
                                                                                  parts = parts[2].Split(',');
                                                                                  artist = parts[1];
                                                                                  genre = parts[2];
                                                                                  year = int.Parse(parts[3]);
                                                                              }
                                                                              else
                                                                              {
                                                                                  parts = line.Split(',');
                                                                                  album = parts[0];
                                                                                  artist = parts[1];
                                                                                  genre = parts[2];
                                                                                  year = int.Parse(parts[3]);
                                                                              }

                                                                              var artistId = i;
                                                                              var genreId = i;
                                                                              var parameters = new List<SQLiteParameter>();
                                                                              var artistResult = await dataAccess.ExecuteSqlWithParametersAsync(artistTable.GetInsertSql(new Artist(i, artist), ref parameters), parameters);
                                                                              if (artistResult <= 0)
                                                                              {
                                                                                  // the artist already exists
                                                                                  artistId = await GetExistingId(artistTable, artist, dataAccess);
                                                                                  artistResult = artistId > 0 ? 1 : artistId;
                                                                              }

                                                                              parameters.Clear();

                                                                              var genreResult = await dataAccess.ExecuteSqlWithParametersAsync(genreTable.GetInsertSql(new Genre(i, genre), ref parameters), parameters);
                                                                              if (genreResult <= 0)
                                                                              {
                                                                                  // the genre already exists
                                                                                  genreId = await GetExistingId(genreTable, genre, dataAccess);
                                                                                  genreResult = genreId > 0 ? 1 : genreId;
                                                                              }

                                                                              parameters.Clear();

                                                                              var albumResult = await dataAccess.ExecuteSqlWithParametersAsync(albumTable.GetInsertSql(new Album(i, artistId, genreId, album, year), ref parameters), parameters);
                                                                              if (albumResult <= 0)
                                                                              {
                                                                                  // the album already exists
                                                                                  throw new SQLiteException(Resources.AlbumExistsExceptionMessage);
                                                                              }

                                                                              i++;

                                                                              if (artistResult + genreResult + albumResult != 3)
                                                                              {
                                                                                  throw new SQLiteException(Resources.ErrorAddingItemMessage);
                                                                              }
                                                                          }

                                                                          if (i != lineCount)
                                                                          {
                                                                              throw new SQLiteException($"Error adding data to database. There were {lineCount} items, but only {i} were added.");
                                                                          }
                                                                      }
                                                                  });
        }

        #endregion
    }
}