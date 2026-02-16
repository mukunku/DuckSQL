namespace DuckSQL.Exceptions
{
    internal class DecimalOverflowException : Exception
    {
        //C# max supported decimal size is: DECIMAL(29,28)
        public const int MAX_DECIMAL_PRECISION = 29;
        public const int MAX_DECIMAL_SCALE = 28;

        public DecimalOverflowException(OverflowException overflowEx) : base(overflowEx.Message, overflowEx)
        {

        }
    }
}
