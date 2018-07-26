using System.Data;
using System.Data.SQLite;

namespace Service.Extensions
{
    internal static class SqliteExtensions
    {
        #region Internal Methods

        /// <summary>
        ///     Makes a new <see cref="SQLiteParameter" /> using the given parameters
        /// </summary>
        /// <param name="parameterName">The name of the </param>
        /// <param name="type">The data type of the object.</param>
        /// <param name="value">The value to insert into the database.</param>
        /// <returns></returns>
        internal static SQLiteParameter Create(string parameterName, DbType type, object value)
        {
            if (!parameterName.StartsWith("@"))
            {
                parameterName = "@" + parameterName;
            }

            return new SQLiteParameter(parameterName, type)
                   {
                       Value = value
                   };
        }

        #endregion
    }
}