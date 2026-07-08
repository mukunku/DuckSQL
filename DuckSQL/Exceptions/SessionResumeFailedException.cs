namespace DuckSQL.Exceptions
{
    internal class SessionResumeFailedException : Exception
    {
        public SessionResumeFailedException(string? message, Exception? innerException) : base(message, innerException)
        {

        }
    }
}
