using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Service.Abstract;
using Service.DAL.Tables;
using Service.Interfaces;
using Service.Models;

namespace Service.Controllers
{
    [RoutePrefix("api/genre")]
    public class GenreController : AbstractController<Genre>
    {
        #region Constructors

        public GenreController(IDataAccess dataAccess) : base(dataAccess) => Table = new GenreTable();

        #endregion

        #region Public Methods

        /// <inheritdoc />
        public override async Task<Genre> MakeItemAsync(ICursor cursor, Dictionary<string, int> indexMap)
        {
            return await Task.Run(() =>
                                  {
                                      var id = cursor.GetInt(indexMap[GenreTable.IdColumnName]);
                                      var name = cursor.GetString(indexMap[GenreTable.NameColumnName]);

                                      return new Genre(id, name);
                                  });
        }

        #endregion
    }
}