using System;
using Service.DAL;
using Service.Extensions;
using Service.Models;

namespace Service.Tests
{
    internal static class HelperObjectFactory
    {
        #region Static Fields and Constants

        private static readonly Random Random = new Random();

        #endregion

        #region Internal Methods

        internal static int GetRandomInt(int? min = null, int? max = null) => Random.Next(min ?? int.MinValue, max ?? int.MaxValue);

        internal static Album MakeAlbum(int? id = null, int? artistId = null, int? genreId = null, string name = null, int? year = null) => new Album(id ?? Random.Next(100), artistId ?? Random.Next(100), genreId ?? Random.Next(100), name ?? StringExtensions.GetRandomStringAsync(Random.Next(100)).Result, year ?? Random.Next(1900, DateTime.Now.Year));

        internal static Artist MakeArtist(int? id = null, string name = null) => new Artist(id ?? Random.Next(100), name ?? StringExtensions.GetRandomStringAsync(Random.Next(100)).Result);

        internal static Genre MakeGenre(int? id = null, string name = null) => new Genre(id ?? Random.Next(100), name ?? StringExtensions.GetRandomStringAsync(Random.Next(100)).Result);

        internal static IdentifyingInfo MakeIdentifyingInfo(int? id = null, string filterText = null, string name = null) => new IdentifyingInfo(id ?? Random.Next(), name ?? StringExtensions.GetRandomStringAsync(32).Result, filterText ?? StringExtensions.GetRandomStringAsync(32).Result);

        internal static SqlWhereClause MakeSqlWhereClause(Conjunction conjunction = null, string columnName = null, Operator @operator = null, string columnValue = null)
        {
            var c = conjunction ?? Conjunction.And;
            var cn = columnName ?? "id";
            var o = @operator ?? Operator.Equal;
            var cv = columnValue ?? "@id";

            return new SqlWhereClause(c, cn, o, cv);
        }

        #endregion
    }
}