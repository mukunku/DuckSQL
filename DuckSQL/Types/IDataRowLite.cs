namespace DuckSQL.Types
{
    internal interface IDataRowLite
    {
        IReadOnlyCollection<string> ColumnNames { get; }
        object[] Row { get; }
        object GetValue(string columnName);
    }
}
