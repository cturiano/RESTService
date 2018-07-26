using System;

namespace Service.DAL
{
    public class IdentifyingInfo : IEquatable<IdentifyingInfo>
    {
        #region Constructors

        internal IdentifyingInfo(IdentifyingInfo copyMe)
        {
            Name = copyMe.Name;
            FilterText = copyMe.FilterText;
            Id = copyMe.Id;
        }

        internal IdentifyingInfo(int? id = null, string name = null, string filterText = null)
        {
            Name = name;
            FilterText = filterText;
            Id = id;
        }

        #endregion

        #region Properties

        internal string FilterText { get; set; }

        internal int? Id { get; set; }

        internal string Name { get; set; }

        #endregion

        #region Public Methods

        #region Interface Implementations

        public bool Equals(IdentifyingInfo other) => other != null && Id.Equals(other.Id) && FilterText.Equals(other.FilterText) && Name.Equals(other.Name);

        #endregion

        public override bool Equals(object obj)
        {
            if (!(obj is IdentifyingInfo other))
            {
                return false;
            }

            return ReferenceEquals(this, obj) || Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Id ?? 0;
                hashCode = (hashCode * 397) ^ FilterText?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ Name?.GetHashCode() ?? 0;
                return hashCode;
            }
        }

        #endregion
    }
}