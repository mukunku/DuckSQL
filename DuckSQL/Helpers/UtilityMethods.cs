using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;

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

        /// <summary>
        /// Checks the format works for DateTime, and that its stripped down versions work for DateOnly and TimeOnly.
        /// </summary>
        /// <remarks>
        /// Best effort by design. The stripping only understands custom formats, so some valid DateTime formats get rejected,
        /// such as standard (single char) formats like "G" and patterns that leave a lone literal behind. That's okay: the
        /// custom date format dialog won't save a rejected format, and users can always enter an equivalent custom format instead.
        /// </remarks>
        public static bool IsValidDateFormat(string? dateFormat)
        {
            if (string.IsNullOrWhiteSpace(dateFormat))
                return false;

            try
            {
                //Test on DateTime
                _ = DateTime.UtcNow.ToString(dateFormat);

                //Test on DateOnly
                var dateOnlyFormat = StripTimeComponentsFromDateFormat(dateFormat);
                _ = DateOnly.FromDateTime(DateTime.UtcNow).ToString(dateOnlyFormat);

                //Test on TimeOnly
                var timeOnlyFormat = StripDateComponentsFromDateFormat(dateFormat);
                _ = TimeOnly.FromDateTime(DateTime.UtcNow).ToString(timeOnlyFormat);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        //Characters left dangling once components are stripped out. Includes the narrow no-break space (\u202F)
        //and right-to-left mark (\u200F) some cultures use in their date/time patterns.
        private static readonly char[] _dateTimeSeparatorChars = ['/', '-', '.', ' ', ',', '_', ':', '\u202F', '\u200F'];

        /// <summary>
        /// Best effort attempt at stripping time components from a date format string.
        /// </summary>
        /// <param name="dateFormat">Date format with potential time components</param>
        /// <returns>Date format with no time components</returns>
        /// <remarks>If empty string is returned, formatting with it will use the users default culture format</remarks>
        public static string? StripTimeComponentsFromDateFormat(string? dateFormat)
        {
            if (dateFormat is null)
                return dateFormat;

            var timeComponents = new string[] { "H", "h", "m", "s", "f", "F", "t", "z", "K", "'ч'" };
            foreach (var component in timeComponents)
            {
                dateFormat = dateFormat.Replace(component, string.Empty);
            }
            dateFormat = Regex.Replace(dateFormat, @"\s{2,}", " ");
            dateFormat = dateFormat.Trim(_dateTimeSeparatorChars);
            return dateFormat.Trim();
        }

        /// <summary>
        /// Best effort attempt at stripping date components from a date format string.
        /// </summary>
        /// <param name="dateFormat">Date format with potential date components</param>
        /// <returns>Time format with no date components</returns>
        /// <remarks>If empty string is returned, formatting with it will use the users default culture format</remarks>
        public static string? StripDateComponentsFromDateFormat(string? dateFormat)
        {
            if (dateFormat is null)
                return dateFormat;

            var dateComponents = new string[] { "y", "M", "d", "g" };
            foreach (var component in dateComponents)
            {
                dateFormat = dateFormat.Replace(component, string.Empty);
            }
            dateFormat = Regex.Replace(dateFormat, @"\s{2,}", " ");
            dateFormat = dateFormat.Trim(_dateTimeSeparatorChars);
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
        /// <param name="beforeRelaunch">Runs only once the current instance has agreed to exit, right before the new one starts.
        /// Use it for changes that should only stick if the restart actually happens.</param>
        public static void RestartApplication(Action? beforeRelaunch = null)
        {
            //Terminate this instance first. This synchronously runs MainForm.OnFormClosing and then OnFormClosed, which
            //does the final session save and releases the primary instance mutex before the new instance starts.
            var exitArgs = new CancelEventArgs();
            Application.Exit(exitArgs);
            if (exitArgs.Cancel)
                return; //A form refused to close

            beforeRelaunch?.Invoke();

            //Start a new DuckSQL instance. Yes this code will also run even though we exited above :P
            ProcessStartInfo startInfo = new()
            {
                UseShellExecute = true, // Do not wait - make the process standalone
                FileName = Application.ExecutablePath
            };
            Process.Start(startInfo);
        }

        /// <summary>
        /// Formats a value to make it is RFC-4180 compliant
        /// </summary>
        /// <param name="value">Raw string value to be added to a CSV file</param>
        /// <returns>Possibly formatted value</returns>
        public static string CleanCSVValue(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                //Escape quotes with double quotes
                string formattedValue = value.Replace("\"", "\"\"");

                //Enclose value with quotes if it contains commas, line feeds, or other quotes
                foreach (char c in formattedValue)
                {
                    if (c == ',' || c == '\r' || c == '\n' || c == '\"')
                    {
                        formattedValue = string.Concat("\"", formattedValue, "\"");
                        break;
                    }
                }

                return formattedValue;
            }
            else
                return string.Empty;
        }

        /// <summary>
        /// Checks if the path has a .sql extension, ignoring case.
        /// </summary>
        public static bool IsSqlFile([NotNullWhen(true)] string? path)
            => path is not null && Path.GetExtension(path).Equals(".sql", StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Returns a <see cref="FileType"/> if a matching one is found for a given file extension.
        /// </summary>
        /// <param name="extension">File extension in ".xyz" format</param>
        /// <returns>null if no matching file type is found</returns>
        public static FileType? ExtensionToFileType(string extension)
        {
            extension = extension?.ToLowerInvariant() ?? string.Empty;
            foreach (var fileType in Enum.GetValues<FileType>())
            {
                if (fileType.GetExtension().Equals(extension))
                {
                    return fileType;
                }
            }
            return null;
        }
    }
}
