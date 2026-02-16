using System.Collections;

namespace DuckSQL.Types
{
    internal interface IListValue : IComparable<IListValue>, IComparable, IEnumerable<object>
    {
        public IList Data { get; }
        public Type Type { get; }
    }
}