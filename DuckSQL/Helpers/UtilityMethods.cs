using DuckSQL.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace DuckSQL.Helpers
{
    internal class UtilityMethods
    {
        public static bool TryParseCultureInfo(string? cultureString, [NotNullWhen(true)] out CultureInfo? cultureInfo)
        {
            if (string.IsNullOrWhiteSpace(cultureString))
                throw new ArgumentNullException(nameof(cultureString));

            try
            {
                cultureInfo = new CultureInfo(cultureString);
                return true;
            }
            catch (CultureNotFoundException)
            {
                cultureInfo = null;
                return false;
            }
        }

        public static bool IsValidDateFormat(string? dateFormat)
        {
            if (string.IsNullOrWhiteSpace(dateFormat))
                return false;

            try
            {
                DateTime.UtcNow.ToString(dateFormat);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Best effort attempt at stripping time components from a date format string.
        /// </summary>
        /// <param name="dateFormat">Date format with potential time components</param>
        /// <returns>Date format with no time components</returns>
        public static string? StripTimeComponentsFromDateFormat(string? dateFormat)
        {
            if (dateFormat is null)
                return dateFormat;

            var timeComponents = new string[] { "H", "h", "m", "s", "f", "F", "t", "z", "K" };
            foreach (var component in timeComponents)
            {
                dateFormat = dateFormat.Replace(component, string.Empty);
            }
            dateFormat = dateFormat.Replace("  ", " ");
            dateFormat = dateFormat.TrimEnd('/', '-', '.', ' ', ',', '_');
            return dateFormat.Trim();
        }

        public static int CompareTo(object? value, object? otherValue)
        {
            value ??= DBNull.Value;
            otherValue ??= DBNull.Value;

            if (otherValue == DBNull.Value && value == DBNull.Value)
                return 0;

            if (otherValue == DBNull.Value)
                return 1;

            if (value == DBNull.Value)
                return -1;

            if (value is IComparable comparableValue && otherValue is IComparable otherComparableValue
                && value.GetType().Equals(otherValue.GetType()))
            {
                return comparableValue.CompareTo(otherComparableValue);
            }
            else
            {
                return value.ToString()!.CompareTo(otherValue.ToString()!);
            }
        }

        public static IEnumerable<(T, R)> PairEnumerables<T, R>(IEnumerable<T> enumerable1, IEnumerable<R> enumerable2)
        {
            ArgumentNullException.ThrowIfNull(enumerable1);
            ArgumentNullException.ThrowIfNull(enumerable2);

            var enumerator1 = enumerable1.GetEnumerator();
            var enumerator2 = enumerable2.GetEnumerator();

            var hasMore1 = enumerator1.MoveNext();
            var hasMore2 = enumerator2.MoveNext();
            while (hasMore1 && hasMore2)
            {
                yield return (enumerator1.Current, enumerator2.Current);
                hasMore1 = enumerator1.MoveNext();
                hasMore2 = enumerator2.MoveNext();
            }

            if (hasMore1 || hasMore2)
            {
                throw new InvalidDataException("Enumerables are of different lengths.");
            }
        }

        /// <summary>
        /// Starts a new DuckSQL instance and terminates the current one.
        /// </summary>
        public static void RestartApplication()
        {
            //Start a new ParquetViewer instance
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.UseShellExecute = true; // Do not wait - make the process standalone
            startInfo.FileName = Application.ExecutablePath;
            Process.Start(startInfo);

            //Terminate this instance
            Application.Exit();
        }
    }
}
