namespace DuckSQL.Exceptions
{
    /// <summary>
    /// A parquet export that can't be done for a reason we can explain to the user, such as an unknown source query or a grid sort we can't translate to SQL.
    /// </summary>
    internal class ParquetExportException : Exception
    {
        public ParquetExportException(string message, Exception? innerException = null)
           : base(message, innerException)
        {
        }
    }
}
