using System;

namespace Service.DAL
{
    public class SqlWhereClause : IEquatable<SqlWhereClause>
    {
        #region Constructors

        public SqlWhereClause(SqlWhereClause copyMe)
        {
            Conjunction = copyMe.Conjunction;
            ColumnName = copyMe.ColumnName;
            Operator = copyMe.Operator;
            ColumnValue = copyMe.ColumnValue;
        }

        public SqlWhereClause(Conjunction conjunction, string columnName, Operator @operator, string columnValue)
        {
            Conjunction = conjunction;
            ColumnName = columnName;
            Operator = @operator;
            ColumnValue = columnValue;
        }

        #endregion

        #region Properties

        internal string ColumnName { get; }

        internal string ColumnValue { get; }

        internal Conjunction Conjunction { get; private set; }

        internal Operator Operator { get; }

        #endregion

        #region Public Methods

        #region Interface Implementations

        public bool Equals(SqlWhereClause other) => other != null && Conjunction.Equals(other.Conjunction) && ColumnName.Equals(other.ColumnName) && ColumnValue.Equals(other.ColumnValue) && Operator.Equals(other.Operator);

        #endregion

        public override bool Equals(object obj)
        {
            if (!(obj is SqlWhereClause other))
            {
                return false;
            }

            return ReferenceEquals(this, other) && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Conjunction?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ (ColumnName?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (ColumnValue?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (Operator?.GetHashCode() ?? 0);
                return hashCode;
            }
        }

        public override string ToString()
        {
            var s = Equals(Conjunction, Conjunction.None) ? string.Empty : Conjunction + " ";
            s += $"{ColumnName} {Operator} {ColumnValue}";
            return s;
        }

        #endregion

        #region Internal Methods

        /// <summary>
        ///     Removes the conjunction for the first clause of a where statement
        /// </summary>
        internal void DumpConjunction()
        {
            Conjunction = Conjunction.None;
        }

        #endregion
    }
}