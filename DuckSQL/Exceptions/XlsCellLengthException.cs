namespace DuckSQL.Exceptions
{
    internal class XlsCellLengthException : Exception
    {
        public int MaxLength { get; }

        public XlsCellLengthException(int maxLength, string? message = null, Exception? innerException = null)
           : base(message, innerException)
        {
            this.MaxLength = maxLength;
        }
    }
}
