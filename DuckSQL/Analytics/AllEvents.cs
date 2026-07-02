using System.Collections;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace DuckSQL.Analytics
{
    internal class FileExportEvent : AmplitudeEvent
    {
        private const string EVENT_TYPE = "file.saveas";

        [JsonIgnore]
        public Helpers.FileType FileType { get; set; }
        public string FileTypeName => FileType.ToString();
        public float FileSizeInMB { get; set; }
        public long RowCount { get; set; }
        public int ColumnCount { get; set; }
        public long ExportTimeMS { get; set; }

        public FileExportEvent() : base(EVENT_TYPE)
        {

        }

        public static void FireAndForget(Helpers.FileType fileType, float fileSizeInMB, int rowCount, int columnCount, long exportTimeInMilliseconds)
        {
            var _ = new FileExportEvent
            {
                FileType = fileType,
                FileSizeInMB = fileSizeInMB,
                RowCount = rowCount,
                ColumnCount = columnCount,
                ExportTimeMS = exportTimeInMilliseconds
            }.Record();
        }
    }

    internal class MenuBarClickEvent : AmplitudeEvent
    {
        private const string EVENT_TYPE = "menubar.click";

        [JsonIgnore]
        public ActionId Action { get; set; }

        public string ActionName => Action.ToString();

        public MenuBarClickEvent() : base(EVENT_TYPE)
        {

        }

        public static void FireAndForget(ActionId action)
        {
            var _ = new MenuBarClickEvent { Action = action }.Record();
        }

        public enum ActionId
        {
            None = 0,
            FileOpen,
            FolderOpen,
            QueryOpen,
            QuerySave,
            Exit,
            AboutBox,
            UserGuide,
            DragDrop,
        }
    }

    internal class ExceptionEvent : AmplitudeEvent
    {
        public const string MASK_SENTINEL = "*****";

        private const string EVENT_TYPE = "exception.thrown";

        [JsonIgnore]
        public Exception Exception { get; }

        public string Message
        {
            get
            {
                var message = Exception.Message;

                if (string.IsNullOrWhiteSpace(message))
                    return message;

                //Mask sensitive text that is wrapped with ` `
                message = Regex.Replace(message, "`[^`]+`", MASK_SENTINEL);
                return message;
            }
        }

        public string? StackTrace => Exception.StackTrace?.ToString();
        public string? InnerException => Exception.InnerException?.ToString();
        public string Type => Exception.GetType().Name;

        [JsonExtensionData]
        public IDictionary<string, object> AdditionalData
        {
            get
            {
                var dictionary = new Dictionary<string, object>();
                foreach (DictionaryEntry keyValuePair in this.Exception.Data)
                {
                    if (keyValuePair.Key is string key && keyValuePair.Value is not null)
                    {
                        dictionary.Add(key, keyValuePair.Value);
                    }
                }
                return dictionary;
            }
        }

        public ExceptionEvent(Exception ex, AmplitudeConfiguration? amplitudeConfiguration = null)
            : base(EVENT_TYPE, amplitudeConfiguration)
        {
            this.Exception = ex ?? throw new ArgumentNullException(nameof(ex));
        }

        public static void FireAndForget(Exception ex)
        {
            var _ = new ExceptionEvent(ex).Record();
        }
    }

    internal class QuickPeekEvent : AmplitudeEvent
    {
        private const string EVENT_TYPE = "quickpeek.show";

        [JsonIgnore]
        public DataTypeId DataType { get; set; }

        public string DataTypeName => this.DataType.ToString();

        public QuickPeekEvent() : base(EVENT_TYPE)
        {

        }

        public enum DataTypeId
        {
            Unknown,
            List,
            Map,
            Struct,
            Image,
            Audio,
        }

        public static void FireAndForget(DataTypeId dataType)
        {
            var _ = new QuickPeekEvent { DataType = dataType }.Record();
        }
    }

    internal class ExecuteQueryEvent : AmplitudeEvent
    {
        private const string EVENT_TYPE = "sql.execute";

        public int? RecordCountTotal { get; set; }
        public int RecordCountFiltered { get; set; }
        public int ColumnCount { get; set; }
        public long RunTimeMS { get; set; }
        public long ConvertTimeMS { get; set; }
        public long RenderTimeMS { get; set; }

        public ExecuteQueryEvent() : base(EVENT_TYPE)
        {

        }

        public static void FireAndForget(
            int? recordCountTotal,
            int recordCountFiltered,
            int columnCount,
            long runTimeInMS,
            long convertTimeInMS,
            long renderTimeMS
            )
        {
            var _ = new ExecuteQueryEvent()
            {
                RecordCountTotal = recordCountTotal,
                RecordCountFiltered = recordCountFiltered,
                ColumnCount = columnCount,
                RunTimeMS = runTimeInMS,
                ConvertTimeMS = convertTimeInMS,
                RenderTimeMS = renderTimeMS
            }.Record();
        }
    }

    internal class ColumnFormattedEvent : AmplitudeEvent
    {
        private const string EVENT_TYPE = "column.format";

        public string FormatName { get; }

        public ColumnFormattedEvent(string? formatName = null) : base(EVENT_TYPE)
        {
            FormatName = formatName ?? string.Empty;
        }

        public static void FireAndForget(string? formatName)
        {
            var _ = new ColumnFormattedEvent(formatName).Record();
        }
    }
}