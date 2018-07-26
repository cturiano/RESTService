namespace Service.Interfaces
{
    public interface ISqlGenerator
    {
        #region Properties

        /// <summary>
        ///     The name of the table on which these SQL statements will act.
        /// </summary>
        string TableName { get; }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Creates a string for deleting an object from the database
        /// </summary>
        string CreateDeleteSql();

        /// <summary>
        ///     Creates a string for inserting an object into the database
        /// </summary>
        string CreateInsertSql();

        /// <summary>
        ///     Creates a string for reading an object from the database
        /// </summary>
        string CreateReadSql();

        /// <summary>
        ///     Creates a string for updating an object in the database
        /// </summary>
        string CreateUpdateSql();

        #endregion
    }
}