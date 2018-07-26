using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Service.Abstract;
using Service.DAL.Tables;
using Service.Interfaces;
using Service.Models;

namespace Service.Controllers
{
    [RoutePrefix("api/album")]
    public class AlbumController : AbstractController<Album>
    {
        #region Constructors

        public AlbumController(IDataAccess dataAccess) : base(dataAccess) => Table = new AlbumTable();

        #endregion

        #region Public Methods

        /// <inheritdoc />
        public override async Task<Album> MakeItemAsync(ICursor cursor, Dictionary<string, int> indexMap)
        {
            return await Task.Run(() =>
                                  {
                                      var id = cursor.GetInt(indexMap[AlbumTable.IdColumnName]);
                                      var name = cursor.GetString(indexMap[AlbumTable.NameColumnName]);
                                      var aId = cursor.GetInt(indexMap[AlbumTable.ArtistIdColumnName]);
                                      var gId = cursor.GetInt(indexMap[AlbumTable.GenreIdColumnName]);
                                      var year = cursor.GetInt(indexMap[AlbumTable.YearColumnName]);

                                      return new Album(id, aId, gId, name, year);
                                  });
        }

        #endregion
    }
}