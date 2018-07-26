using System;
using System.Collections.Generic;
using System.IO;

namespace Service.Interfaces
{
    public interface ICursor : IDisposable
    {
        #region Properties

        bool IsEmpty { get; }

        #endregion

        #region Public Methods

        void Close();

        byte[] GetBlob(int columnIndex);

        long GetBlob(int columnIndex, long dataOffset, ref byte[] buffer, int bufferOffset, int length);

        MemoryStream GetBlobMemoryStream(int columnIndex);

        bool GetBoolean(int columnIndex);

        int GetByteBufferLength();

        int GetColumnCount();

        /// <summary>
        ///     Gets the column index for the specified column name
        /// </summary>
        /// <param name="columnName">The column name to find the index of</param>
        /// <returns>the index of the column name, -1 if not found.</returns>
        int GetColumnIndex(string columnName);

        string GetColumnName(int columnIndex);

        List<string> GetColumnNames();

        DateTime GetDateTimeFromUtcTicksToLocalTime(int columnIndex);

        double GetDouble(int columnIndex);

        int GetFirstFieldIndex();

        float GetFloat(int columnIndex);

        Guid GetGuid(int columnIndex);

        int GetInt(int columnIndex);

        long GetLong(int columnIndex);

        MemoryStream GetNullableBlobMemoryStream(int columnIndex);

        bool? GetNullableBoolean(int columnIndex);

        DateTime? GetNullableDateTimeFromUtcTicksToLocalTime(int columnIndex);

        double? GetNullableDouble(int columnIndex);

        float? GetNullableFloat(int columnIndex);

        int? GetNullableInt(int columnIndex);

        long? GetNullableLong(int columnIndex);

        short? GetNullableShort(int columnIndex);

        string GetNullableString(int columnIndex);

        int GetRowPosition();

        List<object> GetRowValues();

        short GetShort(int columnIndex);

        string GetString(int columnIndex);

        object GetValue(int columnIndex);

        bool IsClosed();

        bool IsFirstRow();

        bool IsNull(int columnIndex);

        /// <returns>Returns True if a new row was successfully loaded and is ready for processing</returns>
        bool MoveToNextRow();

        #endregion
    }
}