namespace DuckSQL.Exceptions
{
    internal class QueryExecuteException : Exception
    {
        public string Title { get; private set; }

        public QueryExecuteException(string title, string message) : base(message)
        {
            this.Title = title;
        }
    }
}
