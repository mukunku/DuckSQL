namespace DuckSQL.Types
{
    internal class QueryResultDataRow : IDataRowLite
    {
        public IReadOnlyCollection<string> ColumnNames { get; }
        public object[] Row { get; }

        public QueryResultDataRow(List<string> columnNames, object[] data)
        {
            this.ColumnNames = columnNames.AsReadOnly();
            this.Row = data;
        }

        public object GetValue(string columnName)
        {
            var result = this.ColumnNames.Index().Where(pair => pair.Item == columnName).FirstOrDefault();
            if (result == default)
                throw new ArgumentOutOfRangeException($"Column `{columnName}` doesn't exist");
            return this.Row[result.Index];
        }
    }
}
