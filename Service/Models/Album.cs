using System;

namespace Service.Models
{
    [Serializable]
    public class Album : BaseModel
    {
        #region Constructors

        public Album(int id, int artistId, int genreId, string name, int year) : base(id, name)
        {
            ArtistId = artistId;
            GenreId = genreId;
            Year = year;
        }

        #endregion

        #region Properties

        public int ArtistId { get; }

        public int GenreId { get; }

        public int Year { get; }

        #endregion

        #region Public Methods

        /// <inheritdoc />
        public override string ToString() => base.ToString() + $"ArtistId: {ArtistId}, GenreId: {GenreId}, Year: {Year}";

        #endregion
    }
}