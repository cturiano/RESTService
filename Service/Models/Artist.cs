using System;

namespace Service.Models
{
    [Serializable]
    public class Artist : BaseModel
    {
        #region Constructors

        public Artist(int id, string name) : base(id, name)
        {
        }

        #endregion
    }
}