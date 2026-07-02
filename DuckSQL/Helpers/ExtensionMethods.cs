using System.Numerics;

namespace DuckSQL.Helpers
{
    internal static class ExtensionMethods
    {
        public static string Format(this string formatString, params object?[] args)
            => string.Format(formatString, args);

        public static T ToEnum<T>(this int value, T @default) where T : struct, Enum
        {
            if (Enum.IsDefined(typeof(T), value))
            {
                return (T)Enum.ToObject(typeof(T), value);
            }
            return @default;
        }

        public static string GetExtension(this FileType fileType) => fileType switch
        {
            FileType.CSV => ".csv",
            FileType.JSON => ".json",
            FileType.XLS => ".xls",
            FileType.XLSX => ".xlsx",
            FileType.PARQUET => ".parquet",
            _ => throw new ArgumentOutOfRangeException(nameof(fileType), fileType, null)
        };

        public static bool ImplementsInterface<T>(this Type? type)
        {
            if (type is null)
                return false;
            else
                return typeof(T).IsAssignableFrom(type);
        }

        /// <summary>
        /// Returns true if the type is a number type.
        /// </summary>
        public static bool IsNumber(this Type type) =>
            Array.Exists(type.GetInterfaces(), i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(INumber<>));

        public static Size RenderedSize(this PictureBox pictureBox)
        {
            var wfactor = (double)pictureBox.Image!.Width / pictureBox.ClientSize.Width;
            var hfactor = (double)pictureBox.Image.Height / pictureBox.ClientSize.Height;

            var resizeFactor = Math.Max(wfactor, hfactor);
            return new Size((int)(pictureBox.Image.Width / resizeFactor), (int)(pictureBox.Image.Height / resizeFactor));
        }

        /// <summary>
        /// Converts a float to a string without using the scientific notation, if possible
        /// </summary>
        /// <returns><paramref name="floatValue"/> in its decimal representation. `null` if the decimal conversion fails.</returns>
        public static string? ToDecimalString(this float floatValue)
            => floatValue >= (float)decimal.MinValue && floatValue <= (float)decimal.MaxValue ? ToDecimalStringImpl(floatValue) : null;

        /// <summary>
        /// Converts a double to a string without using the scientific notation, if possible
        /// </summary>
        /// <returns><paramref name="doubleValue"/> in its decimal representation. `null` if the decimal conversion fails.</returns>
        public static string? ToDecimalString(this double doubleValue)
            => doubleValue >= (double)decimal.MinValue && doubleValue <= (double)decimal.MaxValue ? ToDecimalStringImpl(doubleValue) : null;

        private static string? ToDecimalStringImpl(object value)
        {
            try
            {
                return Convert.ToDecimal(value).ToString();
            }
            catch
            {
                return null;
            }
        }

        //Source: https://stackoverflow.com/a/7574615/1458738
        public static string Left(this string value, int maxLength, string? truncateSuffix = null)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            maxLength = Math.Abs(maxLength);
            return value.Length <= maxLength ? value : (value.Substring(0, maxLength) + truncateSuffix);
        }

        public static IEnumerable<DataGridViewCell> AsEnumerable(this DataGridViewSelectedCellCollection selectedCells)
        {
            foreach (DataGridViewCell cell in selectedCells)
            {
                yield return cell;
            }
        }

        public static IEnumerable<T> AppendIf<T>(this IEnumerable<T> enumerable, bool append, T value)
        {
            if (append)
                return enumerable.Append(value);
            else
                return enumerable;
        }

        public static long ToMillisecondsSinceEpoch(this DateTime dateTime) => new DateTimeOffset(dateTime).ToUnixTimeMilliseconds();
    }
}
