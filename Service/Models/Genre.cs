using System;

namespace Service.Models
{
    [Serializable]
    public class Genre : BaseModel
    {
        #region Constructors

        public Genre(int id, string name) : base(id, name)
        {
        }

        #endregion
    }
}