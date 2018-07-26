using Service.Abstract;
using Service.Models;

namespace Service.DAL.Tables
{
    internal class GenreTable : AbstractTable<Genre>
    {
        #region Constructors

        public GenreTable() => TableName = "genres";

        #endregion
    }
}