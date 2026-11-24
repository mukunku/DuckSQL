using System.Reflection;
using System.Text.Json;

namespace DuckSQL.Helpers
{
    internal static class Env
    {
        private static readonly string _doubleLine = Environment.NewLine + Environment.NewLine;
        public static string DoubleNewLine => _doubleLine;

        private const string DefaultDateTimeFormat = "g";
        public const string ISO8601DateTimeFormat = "yyyy-MM-ddTHH:mm:ss.FFFFFFF";
        public static string DateTimeFormat => AppSettings.DateTimeDisplayFormat switch
        {
            DateFormat.Default => DefaultDateTimeFormat,
            DateFormat.ISO8601 => ISO8601DateTimeFormat,
            DateFormat.Custom => AppSettings.CustomDateFormat ?? DefaultDateTimeFormat,
            _ => string.Empty,
        };

        private const string DefaultDateOnlyFormat = "d";
        public const string ISO8601DateOnlyFormat = "yyyy-MM-dd";
        public static string DateOnlyFormat => AppSettings.DateTimeDisplayFormat switch
        {
            DateFormat.Default => DefaultDateOnlyFormat,
            DateFormat.ISO8601 => ISO8601DateOnlyFormat,
            DateFormat.Custom => UtilityMethods.StripTimeComponentsFromDateFormat(AppSettings.CustomDateFormat) ?? DefaultDateOnlyFormat,
            _ => string.Empty,
        };

        private const string DefaultTimeOnlyFormat = "T";
        public const string ISO8601TimeOnlyFormat = "HH:mm:ss.FFFFFFF";
        public static string TimeOnlyFormat => AppSettings.DateTimeDisplayFormat switch
        {
            DateFormat.Default => DefaultTimeOnlyFormat,
            DateFormat.ISO8601 => ISO8601TimeOnlyFormat,
            DateFormat.Custom => UtilityMethods.StripDateComponentsFromDateFormat(AppSettings.CustomDateFormat) ?? DefaultTimeOnlyFormat,
            _ => string.Empty,
        };

        #region Latest Release Version
        private const string RELEASES_API_URL = "https://api.github.com/repos/mukunku/DuckSQL/releases";
        private static SemanticVersion? _latestReleaseVersion = null;
        private static Uri? _releaseUri = null;
        private static DateTime? _latestReleaseLastCheckedOn = null;
        public static async Task<(SemanticVersion? Version, Uri? Url)> FetchLatestRelease()
        {
            //We cache the http response for 30 minutes (even if it failed) so we don't spam http requests
            if (_latestReleaseLastCheckedOn.HasValue && DateTime.UtcNow.Subtract(_latestReleaseLastCheckedOn.Value) < TimeSpan.FromMinutes(30))
            {
                return (_latestReleaseVersion, _releaseUri);
            }

            //Clear previously cached values
            _latestReleaseVersion = null;
            _releaseUri = null;
            _latestReleaseLastCheckedOn = DateTime.UtcNow;

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("User-Agent", "DuckSQL Query Editor"); //GitHub requires a user agent
            using var response = await httpClient.GetAsync(RELEASES_API_URL);

            response.EnsureSuccessStatusCode();

            using var responseJson = JsonDocument.Parse(response.Content.ReadAsStream());
            foreach (var release in responseJson.RootElement.EnumerateArray())
            {
                Uri? releaseUri = null;
                if (release.TryGetProperty("html_url", out var releaseUrl)
                    && releaseUrl.GetString() is string url)
                {
                    releaseUri = new Uri(url);
                }

                if (release.TryGetProperty("tag_name", out var releaseTag)
                    && releaseTag.GetString() is string tag
                    && SemanticVersion.TryParse(tag, out var semanticVersion))
                {
                    _releaseUri = releaseUri;
                    _latestReleaseVersion = semanticVersion;
                    return (semanticVersion, releaseUri);
                }

                //We only care about the latest release so stop processing now
                break;
            }

            return (null, null);
        }
        #endregion

        #region Assembly Version
        private static SemanticVersion? _assemblyVersion = null;
        public static SemanticVersion AssemblyVersion => _assemblyVersion ??= ParseAssemblyVersion();

        private static SemanticVersion ParseAssemblyVersion()
        {
            var assemblyVersionString = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? string.Empty;
            SemanticVersion.TryParse(assemblyVersionString, out var assemblyVersion);
            return assemblyVersion;
        }
        #endregion

        #region Instance Management
        private static Mutex? _operatingSystemLevelNamedMutex;
        private static bool? _isPrimaryInstance;

        /// <summary>
        /// Claims the primary instance mutex if no other DuckSQL.exe holds it. Call once, on the UI thread, at startup.
        /// The UI thread must also be the one that calls <see cref="TryReleasePrimaryInstance"/> since mutexes are owned by threads.
        /// </summary>
        public static void TryClaimPrimaryInstance()
        {
            if (_isPrimaryInstance is not null)
                throw new InvalidOperationException("The primary instance was already claimed");

            try
            {
                _operatingSystemLevelNamedMutex = new Mutex(false, @"Local\DuckSQL.SessionCache");
                _isPrimaryInstance = _operatingSystemLevelNamedMutex.WaitOne(TimeSpan.Zero);
            }
            catch (AbandonedMutexException)
            {
                _isPrimaryInstance = true; //previous owner crashed; we own it now
            }
            catch (Exception)
            {
                _isPrimaryInstance = false;
            }
        }

        /// <summary>
        /// Whether this instance owns the session cache. Only valid after <see cref="TryClaimPrimaryInstance"/> has been called.
        /// </summary>
        public static bool IsPrimaryInstance() => _isPrimaryInstance
            ?? throw new InvalidOperationException($"{nameof(TryClaimPrimaryInstance)}() hasn't been called yet");

        public static void TryReleasePrimaryInstance()
        {
            if (_isPrimaryInstance == true)
                _operatingSystemLevelNamedMutex?.ReleaseMutex();

            _isPrimaryInstance = false;
        }
        #endregion
    }
}
