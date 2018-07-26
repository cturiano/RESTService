using System;

namespace Service.Abstract
{
    public abstract class AbstractTypeSafeEnum : IEquatable<AbstractTypeSafeEnum>
    {
        #region Fields

        private readonly string _name;
        private readonly int _value;

        #endregion

        #region Constructors

        /// <summary>
        ///     Parametrized constructor for <see cref="AbstractTypeSafeEnum" />.
        /// </summary>
        protected AbstractTypeSafeEnum(int value, string name)
        {
            _name = name;
            _value = value;
        }

        #endregion

        #region Public Methods

        #region Interface Implementations

        /// <inheritdoc />
        /// <summary>
        ///     Compares this instance with the other instance for equality based on the properties.
        /// </summary>
        public bool Equals(AbstractTypeSafeEnum other) => other != null && _name.Equals(other._name) && _value.Equals(other._value);

        #endregion

        /// <summary>
        ///     Compares this instance with the given object for equality based types and then on the properties.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (!(obj is AbstractTypeSafeEnum other))
            {
                return false;
            }

            return ReferenceEquals(this, other) || Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = _name.GetHashCode();
                hashCode = (hashCode * 397) ^ _value.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString() => _name;

        #endregion
    }
}