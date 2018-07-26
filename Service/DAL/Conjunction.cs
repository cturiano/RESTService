using Service.Abstract;

namespace Service.DAL
{
    public class Conjunction : AbstractTypeSafeEnum
    {
        #region Constructors

        /// <inheritdoc />
        /// <summary>
        ///     Parametrized constructor for <see cref="T:Service.DAL.Conjunction" />.
        /// </summary>
        private Conjunction(int value, string name) : base(value, name)
        {
        }

        #endregion

        #region Statics And Constants

        internal static readonly Conjunction And = new Conjunction(1, "AND");
        internal static readonly Conjunction Between = new Conjunction(2, "BETWEEN");
        internal static readonly Conjunction ConCat = new Conjunction(13, "||");
        internal static readonly Conjunction Exists = new Conjunction(3, "EXISTS");
        internal static readonly Conjunction Glob = new Conjunction(7, "GLOB");
        internal static readonly Conjunction In = new Conjunction(4, "IN");
        internal static readonly Conjunction Is = new Conjunction(11, "IS");
        internal static readonly Conjunction IsNot = new Conjunction(12, "IS NOT");
        internal static readonly Conjunction IsNull = new Conjunction(10, "IS NULL");
        internal static readonly Conjunction Like = new Conjunction(6, "LIKE");
        internal static readonly Conjunction Not = new Conjunction(8, "NOT");
        internal static readonly Conjunction NotIn = new Conjunction(5, "NOT IN");
        internal static readonly Conjunction Or = new Conjunction(9, "OR");
        internal static readonly Conjunction Unique = new Conjunction(14, "UNIQUE");
        internal static readonly Conjunction None = new Conjunction(15, string.Empty);

        #endregion
    }
}