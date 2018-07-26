using Service.Abstract;

namespace Service.DAL
{
    public sealed class Operator : AbstractTypeSafeEnum
    {
        #region Constructors

        /// <inheritdoc />
        /// <summary>
        ///     Parametrized constructor for <see cref="T:Service.DAL.Operator" />.
        /// </summary>
        private Operator(int value, string name) : base(value, name)
        {
        }

        #endregion

        #region Statics And Constants

        internal static readonly Operator Equal = new Operator(1, "=");
        internal static readonly Operator Equalequal = new Operator(2, "==");
        internal static readonly Operator GreaterOrLesser = new Operator(3, "<>");
        internal static readonly Operator GreaterThan = new Operator(4, ">");
        internal static readonly Operator GreaterThanOrEqual = new Operator(5, ">=");
        internal static readonly Operator LessThan = new Operator(6, "<");
        internal static readonly Operator LessThanOrEqual = new Operator(7, "<=");
        internal static readonly Operator NotEqual = new Operator(8, "!=");
        internal static readonly Operator NotGreaterThan = new Operator(9, "!>");
        internal static readonly Operator NotLessThan = new Operator(10, "!<");

        #endregion
    }
}