using DuckSQL.Helpers;
using System.Text.Json;

namespace DuckSQL.Controls
{
    /// <summary>
    /// Persists the open query tabs to disk so they can be restored the next time the app starts.
    /// </summary>
    internal class SessionCache
    {
        private const string CACHE_FILE_NAME = "DUCKSQL_QUERY_CACHE.json";
        private const string CACHE_BACKUP_FILE_NAME = "DUCKSQL_QUERY_BACKUP_CACHE.json";
        private static string CacheFilePath => Path.Combine(Path.GetTempPath(), CACHE_FILE_NAME);
        private static string CacheBackupFilePath => Path.Combine(Path.GetTempPath(), CACHE_BACKUP_FILE_NAME);

        private int _lastCacheHashCode = 0;
        private string _cachePath = CacheFilePath;

        public record RestoredSession(CustomTabPage[] Tabs, CustomTabPage? SelectedTab, int NewQueryCount);

        /// <summary>
        /// Reads the most recently written cache file.
        /// </summary>
        /// <returns>null if there's nothing worth restoring</returns>
        public RestoredSession? Load()
        {
            var mainCacheFileLastWriteTime = File.Exists(CacheFilePath) ? File.GetLastWriteTime(CacheFilePath) : DateTime.MinValue;
            var backupCacheFileLastWriteTime = File.Exists(CacheBackupFilePath) ? File.GetLastWriteTime(CacheBackupFilePath) : DateTime.MinValue;

            if (mainCacheFileLastWriteTime == DateTime.MinValue && backupCacheFileLastWriteTime == DateTime.MinValue)
                return null; //nothing to restore

            var cacheFileToUsePath = mainCacheFileLastWriteTime > backupCacheFileLastWriteTime ? CacheFilePath : CacheBackupFilePath;
            var cacheDetails = JsonSerializer.Deserialize<TabControlCacheRecord>(File.ReadAllText(cacheFileToUsePath));
            if (cacheDetails is null)
                return null;

            var recordsToRestore = cacheDetails.Records.Where(record => !record.IsEmptyQueryTab).ToArray();
            if (recordsToRestore.Length == 0)
                return null; //Only empty query tabs, nothing to restore

            var tabs = new CustomTabPage[recordsToRestore.Length];
            CustomTabPage? selectedTab = null;
            for (var i = 0; i < recordsToRestore.Length; i++)
            {
                tabs[i] = recordsToRestore[i].ToTabPage();
                if (recordsToRestore[i].IsSelectedTab)
                    selectedTab = tabs[i];
            }

            return new RestoredSession(tabs, selectedTab, cacheDetails.NewQueryCount);
        }

        /// <summary>
        /// Writes the tabs to the cache file. Never throws.
        /// </summary>
        /// <param name="force">Write even if nothing changed since the last save</param>
        /// <remarks>This is synchronous on purpose to guarantee completion on application shutdown.</remarks>
        public void Save(IEnumerable<CustomTabPage> tabs, CustomTabPage? selectedTab, int newQueryCount, bool force)
        {
            try
            {
                var cacheRecords = new List<TabPageCacheRecord>();
                foreach (var tab in tabs)
                {
                    var text = tab.CurrentText;
                    if (!tab.IsFileBackedTab && string.IsNullOrWhiteSpace(text))
                        continue; //nothing worth restoring in empty scratch tabs

                    //Clean file backed tabs get reloaded from disk, so only keep their text when there are unsaved edits
                    var textToCache = tab.IsFileBackedTab && !tab.IsDirty ? null : text;
                    cacheRecords.Add(new(tab.Kind, tab.FilePath, tab.ScratchNumber, textToCache, tab == selectedTab));
                }

                //Check if anything changed, including the selected tab since records track that too
                var tabCacheHash = new HashCode();
                foreach (var cacheRecord in cacheRecords)
                {
                    tabCacheHash.Add(cacheRecord);
                }
                var tabCacheHashCode = tabCacheHash.ToHashCode();

                if (force || _lastCacheHashCode != tabCacheHashCode)
                {
                    var cacheDetails = new TabControlCacheRecord(newQueryCount, cacheRecords.ToArray());

                    //Write to a temp file first and then swap it in, so the cache file is never left half written
                    var tempPath = _cachePath + ".tmp";
                    File.WriteAllText(tempPath, JsonSerializer.Serialize(cacheDetails));
                    File.Move(tempPath, _cachePath, overwrite: true);
                    _lastCacheHashCode = tabCacheHashCode; //only after a successful write, so failed writes get retried
                }
            }
            catch
            {
                //Alternate between the main cache file and the backup one to help
                //deal with any kind of weirdness and reduce the chance of data loss
                if (_cachePath.Equals(CacheFilePath))
                    _cachePath = CacheBackupFilePath;
                else
                    _cachePath = CacheFilePath;
            }
        }

        private record TabControlCacheRecord(int NewQueryCount, TabPageCacheRecord[] Records);
        private record TabPageCacheRecord(CustomTabPage.TabKind Kind, string? FilePath, int ScratchNumber, string? Text, bool IsSelectedTab,
            string? Tag = null, string? Title = null) //Tag & Title are only used to read caches from older versions
        {
            //Legacy records stored file paths in Tag, which are never blank, so a blank Tag means an empty query tab
            public bool IsEmptyQueryTab => Tag is not null
                ? string.IsNullOrWhiteSpace(Tag)
                : Kind == CustomTabPage.TabKind.Scratch && string.IsNullOrWhiteSpace(Text);

            public CustomTabPage ToTabPage()
            {
                if (Tag is not null && Title is not null)
                    return FromLegacyRecord(Tag, Title);

                return Kind == CustomTabPage.TabKind.SqlFile && FilePath is not null
                    ? CustomTabPage.ForSqlFile(FilePath, Text)
                    : CustomTabPage.ForScratch(ScratchNumber, Text);
            }

            //Old caches stored either a file path or query text in Tag, and the tab title in Title
            private static CustomTabPage FromLegacyRecord(string tag, string title)
            {
                if (File.Exists(tag) && UtilityMethods.IsSqlFile(tag))
                    return CustomTabPage.ForSqlFile(tag);

                var scratchNumber = int.TryParse(title.Trim(' ', '*').Split(' ').Last(), out var number) ? number : 1;
                return File.Exists(tag) || Directory.Exists(tag)
                    ? CustomTabPage.ForDataSource(scratchNumber, tag)
                    : CustomTabPage.ForScratch(scratchNumber, tag);
            }
        }
    }
}
