using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace DuckSQL.Types
{
    internal interface IStructValue : IComparable<IStructValue>, IComparable
    {
        public IDataRowLite Data { get; }

        IReadOnlyCollection<string> FieldNames { get; }

        string ToStringTruncated(int desiredLength);

        DataTable ToDataTable();

        string ToJSON(out bool success, int? desiredLength = null);

        /// <summary>
        /// https://huggingface.co/docs/hub/en/datasets-image#parquet-format
        /// </summary>
        /// <returns>True if this is a struct with "bytes" and "path" fields</returns>
        bool IsHuggingFaceFormat([NotNullWhen(true)] out byte[]? data);
    }
}