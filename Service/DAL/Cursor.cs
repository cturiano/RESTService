using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using Service.Interfaces;

#pragma warning disable 1584,1711,1572,1581,1580

namespace Service.DAL
{
    internal class Cursor : ICursor
    {
        #region Constructors

        internal Cursor(SQLiteCommand command)
        {
            Initialize(command, false);
        }

        internal Cursor(SQLiteCommand command, bool singleResult)
        {
            Initialize(command, singleResult);
        }

        #endregion

        #region Properties

        private SQLiteCommand Command { get; set; }

        public bool IsEmpty { get; set; }

        private SQLiteDataReader QueryResult { get; set; }

        public int RowCounter { get; private set; }

        #endregion

        #region Protected Methods

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                using (Command)
                {
                    using (QueryResult)
                    {
                    }
                }
            }
        }

        #endregion

        #region Internal Methods

        internal void Initialize(SQLiteCommand command, bool singleResult)
        {
            Command = command;
            QueryResult = singleResult ? command.ExecuteReader(CommandBehavior.SingleRow) : command.ExecuteReader();
            IsEmpty = !MoveToNextRow();
        }

        internal static bool IsNullOrEmpty(ICursor cursor) => cursor == null || cursor.IsEmpty;

        #endregion

        #region Statics And Constants

        private const int ByteBufferLength = 1024;
        internal const int FirstFieldIndex = 0;

        #endregion

        #region Interface Implementations

        public void Close()
        {
            Dispose();
        }

        /// <exception cref="InvalidCastException">The specified cast is not valid. </exception>
        /// <exception cref="IOException">An I/O error occurs. </exception>
        /// <exception cref="ArgumentNullException"><paramref name="buffer" /> is null. </exception>
        /// <exception cref="NotSupportedException">
        ///     The stream does not support writing. For additional information see
        ///     <see cref="P:System.IO.Stream.CanWrite" />.-or- The current position is closer than <paramref name="count" /> bytes
        ///     to the end of the stream, and the capacity cannot be modified.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <paramref name="offset" /> subtracted from the buffer length is less than
        ///     <paramref name="count" />.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="offset" /> or <paramref name="count" /> are negative. </exception>
        /// <exception cref="ObjectDisposedException">The current stream instance is closed. </exception>
        public byte[] GetBlob(int columnIndex)
        {
            using (var memoryStream = GetBlobMemoryStream(columnIndex))
            {
                return memoryStream.ToArray();
            }
        }

        /// <exception cref="InvalidCastException">The specified cast is not valid. </exception>
        public long GetBlob(int columnIndex, long dataOffset, ref byte[] buffer, int bufferOffset, int length) => QueryResult.GetBytes(columnIndex, dataOffset, buffer, bufferOffset, length);

        /// <exception cref="InvalidCastException">The specified cast is not valid. </exception>
        /// <exception cref="IOException">An I/O error occurs. </exception>
        /// <exception cref="ArgumentNullException"><paramref name="buffer" /> is null. </exception>
        /// <exception cref="NotSupportedException">
        ///     The stream does not support writing. For additional information see
        ///     <see cref="P:System.IO.Stream.CanWrite" />.-or- The current position is closer than <paramref name="count" /> bytes
        ///     to the end of the stream, and the capacity cannot be modified.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <paramref name="offset" /> subtracted from the buffer length is less than
        ///     <paramref name="count" />.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="offset" /> or <paramref name="count" /> are negative. </exception>
        /// <exception cref="ObjectDisposedException">The current stream instance is closed. </exception>
        public MemoryStream GetBlobMemoryStream(int columnIndex)
        {
            var memoryStream = new MemoryStream();
            var buffer = new byte[ByteBufferLength];
            long totalBytesRead = 0;
            int bytesRead;
            while ((bytesRead = (int)GetBlob(columnIndex, totalBytesRead, ref buffer, 0, ByteBufferLength)) > 0)
            {
                totalBytesRead += bytesRead;
                memoryStream.Write(buffer, 0, bytesRead);
            }

            return memoryStream;
        }

        /// <exception cref="InvalidCastException">The specified cast is not valid. </exception>
        public bool GetBoolean(int columnIndex) => QueryResult.GetBoolean(columnIndex);

        public int GetByteBufferLength() => ByteBufferLength;

        public int GetColumnCount() => QueryResult.FieldCount;

        /// <summary>
        ///     Gets the column index for the specified column name
        /// </summary>
        /// <param name="columnName">The column name to find the index of</param>
        /// <returns>the index of the column name, -1 if not found.</returns>
        /// <exception cref="IndexOutOfRangeException">The name specified is not a valid column name.</exception>
        public int GetColumnIndex(string columnName) => QueryResult.GetOrdinal(columnName);

        public string GetColumnName(int columnIndex) => QueryResult.GetName(columnIndex);

        /// <exception cref="NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity" /> is less than 0. </exception>
        public List<string> GetColumnNames()
        {
            var columnNames = new List<string>(QueryResult.FieldCount);
            for (var i = GetFirstFieldIndex(); i < QueryResult.FieldCount; i++)
            {
                columnNames.Add(GetColumnName(i));
            }

            return columnNames;
        }

        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="ticks" /> is less than
        ///     <see cref="F:System.DateTime.MinValue" /> or greater than <see cref="F:System.DateTime.MaxValue" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <paramref name="kind" /> is not one of the <see cref="T:System.DateTimeKind" />
        ///     values.
        /// </exception>
        /// <exception cref="InvalidCastException">The specified cast is not valid. </exception>
        public DateTime GetDateTimeFromUtcTicksToLocalTime(int columnIndex) => new DateTime(GetLong(columnIndex), DateTimeKind.Utc).ToLocalTime();

        /// <exception cref="InvalidCastException">The specified cast is not valid. </exception>
        public double GetDouble(int columnIndex) => QueryResult.GetDouble(columnIndex);

        public int GetFirstFieldIndex() => FirstFieldIndex;

        /// <exception cref="InvalidCastException">The specified cast is not valid. </exception>
        public float GetFloat(int columnIndex) => QueryResult.GetFloat(columnIndex);

        /// <exception cref="InvalidCastException">The specified cast is not valid. </exception>
        public Guid GetGuid(int columnIndex) => QueryResult.GetGuid(columnIndex);

        /// <exception cref="InvalidCastException">The specified cast is not valid. </exception>
        public int GetInt(int columnIndex) => QueryResult.GetInt32(columnIndex);

        /// <exception cref="InvalidCastException">The specified cast is not valid. </exception>
        public long GetLong(int columnIndex) => QueryResult.GetInt64(columnIndex);

        /// <exception cref="InvalidCastException">The specified cast is not valid. </exception>
        /// <exception cref="IOException">An I/O error occurs. </exception>
        /// <exception cref="ObjectDisposedException">The current stream instance is closed. </exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="offset" /> or <paramref name="count" /> are negative. </exception>
        /// <exception cref="ArgumentException">
        ///     <paramref name="offset" /> subtracted from the buffer length is less than
        ///     <paramref name="count" />.
        /// </exception>
        public MemoryStream GetNullableBlobMemoryStream(int columnIndex) => !IsNull(columnIndex) ? GetBlobMemoryStream(columnIndex) : null;

        /// <exception cref="InvalidCastException">The specified cast is not valid. </exception>
        public bool? GetNullableBoolean(int columnIndex) => !IsNull(columnIndex) ? (bool?)GetBoolean(columnIndex) : null;

        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="ticks" /> is less than
        ///     <see cref="F:System.DateTime.MinValue" /> or greater than <see cref="F:System.DateTime.MaxValue" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <paramref name="kind" /> is not one of the <see cref="T:System.DateTimeKind" />
        ///     values.
        /// </exception>
        /// <exception cref="InvalidCastException">The specified cast is not valid. </exception>
        public DateTime? GetNullableDateTimeFromUtcTicksToLocalTime(int columnIndex) => !IsNull(columnIndex) ? (DateTime?)GetDateTimeFromUtcTicksToLocalTime(columnIndex) : null;

        /// <exception cref="InvalidCastException">The specified cast is not valid. </exception>
        public double? GetNullableDouble(int columnIndex) => !IsNull(columnIndex) ? (double?)GetDouble(columnIndex) : null;

        /// <exception cref="InvalidCastException">The specified cast is not valid. </exception>
        public float? GetNullableFloat(int columnIndex) => !IsNull(columnIndex) ? (float?)GetFloat(columnIndex) : null;

        /// <exception cref="InvalidCastException">The specified cast is not valid. </exception>
        public int? GetNullableInt(int columnIndex) => !IsNull(columnIndex) ? (int?)GetInt(columnIndex) : null;

        /// <exception cref="InvalidCastException">The specified cast is not valid. </exception>
        public long? GetNullableLong(int columnIndex) => !IsNull(columnIndex) ? (long?)GetLong(columnIndex) : null;

        /// <exception cref="InvalidCastException">The specified cast is not valid. </exception>
        public short? GetNullableShort(int columnIndex) => !IsNull(columnIndex) ? (short?)GetShort(columnIndex) : null;

        /// <exception cref="InvalidCastException">The specified cast is not valid. </exception>
        public string GetNullableString(int columnIndex) => !IsNull(columnIndex) ? GetString(columnIndex) : null;

        public int GetRowPosition() => QueryResult.StepCount;

        /// <exception cref="NotSupportedException">There is no current connection to an instance of SQL Server. </exception>
        /// <exception cref="ArgumentNullException"><paramref name="collection" /> is null.</exception>
        public List<object> GetRowValues()
        {
            var values = new object[QueryResult.FieldCount];
            QueryResult.GetValues(values);
            return new List<object>(values);
        }

        /// <exception cref="InvalidCastException">The specified cast is not valid. </exception>
        public short GetShort(int columnIndex) => QueryResult.GetInt16(columnIndex);

        /// <exception cref="InvalidCastException">The specified cast is not valid. </exception>
        public string GetString(int columnIndex) => QueryResult.GetString(columnIndex);

        public object GetValue(int columnIndex) => QueryResult.GetValue(columnIndex);

        public bool IsClosed() => QueryResult.IsClosed;

        public bool IsFirstRow() => QueryResult.StepCount == 1;

        public bool IsNull(int columnIndex) => QueryResult.IsDBNull(columnIndex);

        /// <returns>Returns True if a new row was successfully loaded and is ready for processing</returns>
        public bool MoveToNextRow()
        {
            RowCounter++;
            return QueryResult.Read();
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        #endregion
    }
}