using System;
using Service.Interfaces;

namespace Service.Models
{
    [Serializable]
    public class BaseModel : IHasId, IHasName, IEquatable<BaseModel>
    {
        #region Static Fields and Constants

        private const int MaxNameLength = 50;
        private const int MinNameLength = 1;

        #endregion

        #region Constructors

        public BaseModel(int id, string name)
        {
            if (ValidateParameters(name))
            {
                Id = id;
                Name = name;
            }
        }

        #endregion

        #region Properties

        /// <inheritdoc />
        public int Id { get; }

        /// <inheritdoc />
        public string Name { get; }

        #endregion

        #region Public Methods

        /// <inheritdoc />
        public bool Equals(BaseModel other)
        {
            if (other == null)
            {
                return false;
            }

            return ReferenceEquals(this, other) || Id == other.Id && string.Equals(Name, other.Name);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            var other = obj as BaseModel;
            if (other == null)
            {
                return false;
            }

            return ReferenceEquals(this, other) || Equals(other);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                return (Id * 397) ^ (Name != null ? Name.GetHashCode() : 0);
            }
        }

        public static bool operator ==(BaseModel left, BaseModel right) => Equals(left, right);

        public static bool operator !=(BaseModel left, BaseModel right) => !Equals(left, right);

        /// <inheritdoc />
        public override string ToString()
        {
            return $"ID: {Id}, Name: {Name}";
        }

        #endregion

        #region Private Methods

        private bool ValidateParameters(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name), "The name parameter must not be null or empty.");
            }

            var nameLength = name.Length;
            if (nameLength > MaxNameLength || nameLength < MinNameLength)
            {
                throw new ArgumentException(nameof(name), $"The name parameter must be less than {MaxNameLength + 1} and greater than {MinNameLength - 1}.");
            }

            return true;
        }

        #endregion
    }
}