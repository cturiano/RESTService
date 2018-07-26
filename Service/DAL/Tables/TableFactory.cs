using System;
using System.Collections.Generic;
using Service.Abstract;
using Service.Interfaces;
using Service.Models;

namespace Service.DAL.Tables
{
    internal static class TableFactory<T> where T : BaseModel
    {
        #region Static Fields and Constants

        private static readonly Dictionary<Type, ITable<T>> Tables = new Dictionary<Type, ITable<T>>();

        #endregion

        #region Internal Methods

        /// <summary>
        ///     Gets or creates an <see cref="AbstractTable{T}" /> of the requested type.
        /// </summary>
        /// <exception cref="MissingMethodException">
        ///     In the .NET for Windows Store apps or the Portable Class Library, catch the
        ///     base class exception, <see cref="T:System.MissingMemberException" />, instead.The type that is specified for
        ///     TResult does not have a parameterless constructor.
        /// </exception>
        /// <exception cref="ArgumentNullException">Key is null.</exception>
        internal static TResult GetTable<TResult>() where TResult : AbstractTable<T>
        {
            if (Tables.ContainsKey(typeof(TResult)))
            {
                return (TResult)Tables[typeof(TResult)];
            }

            var table = Activator.CreateInstance<TResult>();
            Tables[typeof(TResult)] = table;

            return table;
        }

        #endregion
    }
}