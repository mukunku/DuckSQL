using DuckSQL.Types;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace DuckSQL.Helpers
{
    internal class CustomJsonWriter : IDisposable
    {
        public int ApproximateStringLengthSoFar { get; private set; }

        private readonly Utf8JsonWriter _writer;

        public CustomJsonWriter(Stream stream)
        {
            _writer = new Utf8JsonWriter(stream, new JsonWriterOptions()
            {
                //Without 'UnsafeRelaxedJsonEscaping' JSON reserved chars get escaped using unicode instead of backslash (E.g. \u0022 instead of \")
                //https://stackoverflow.com/questions/70849792/system-text-json-utf8jsonwriter-how-to-prevent-breaking-unicode-characters-int
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });
            ApproximateStringLengthSoFar = 0;
        }

        public void WriteStartObject()
        {
            _writer.WriteStartObject();
            ApproximateStringLengthSoFar++; // '{'
        }

        public void WriteEndObject()
        {
            _writer.WriteEndObject();
            ApproximateStringLengthSoFar++; // '}'
        }

        public void WritePropertyName(string name)
        {
            ArgumentNullException.ThrowIfNull(name);
            _writer.WritePropertyName(name);
            ApproximateStringLengthSoFar += name.Length + 1; // +1 for the colon ':'
        }

        private void WriteNullValue()
        {
            _writer.WriteNullValue();
            ApproximateStringLengthSoFar += 4; // 'null'
            IncrementForComma();
        }

        private void WriteStringValue(string value)
        {
            _writer.WriteStringValue(value);
            ApproximateStringLengthSoFar += value.Length;
            IncrementForComma();
        }

        private void WriteBooleanValue(bool value)
        {
            _writer.WriteBooleanValue(value);
            if (value)
                ApproximateStringLengthSoFar += 4; // 'true'
            else
                ApproximateStringLengthSoFar += 5; // 'false'

            IncrementForComma();
        }

        private void WriteNumberValue(decimal value)
        {
            _writer.WriteNumberValue(value);
            ApproximateStringLengthSoFar += value.ToString().Length;
        }

        private void WriteRawValue(string json)
        {
            _writer.WriteRawValue(json);
            ApproximateStringLengthSoFar += json.Length;
        }

        private void WriteStartArray()
        {
            _writer.WriteStartArray();
            ApproximateStringLengthSoFar += 1; // '['
        }

        private void WriteEndArray()
        {
            _writer.WriteEndArray();
            ApproximateStringLengthSoFar += 1; // ']'
        }

        //We don't keep track of element count in each nested structure, so we can't tell
        //if a comma would be added to the json element or not. So we just always assume there is one :shrug:
        //E.g. { elem1: value, elem2: value } vs { elem1: value }
        private void IncrementForComma() => ApproximateStringLengthSoFar++;

        public void Dispose()
        {
            _writer.Dispose();
        }

        public void WriteValue(object value, bool truncateForDisplay)
        {
            if (value is null)
            {
                //Value should never be null as we should be replacing all those with DBNull.Value
                throw new ArgumentNullException(nameof(value));
            }
            else if (value == DBNull.Value)
            {
                WriteNullValue();
            }
            else if (value is string str)
            {
                WriteStringValue(str);
            }
            else if (value is bool @bool)
            {
                WriteBooleanValue(@bool);
            }
            else if (value.GetType().IsNumber())
            {
                WriteNumberValue(Convert.ToDecimal(value));
            }
            else if (value is IStructValue @struct)
            {
                var json = @struct.ToJSON(out var success);
                if (success)
                    WriteRawValue(json);
                else
                    WriteStringValue(json);
            }
            else if (value is IMapValue map)
            {
                WriteStartArray();
                foreach ((object mapKey, object mapValue) in map)
                {
                    WriteStartObject();
                    WritePropertyName("key");
                    WriteValue(mapKey, truncateForDisplay);
                    WritePropertyName("value");
                    WriteValue(mapValue, truncateForDisplay);
                    WriteEndObject();
                }
                WriteEndArray();
            }
            else if (value is IListValue list)
            {
                WriteStartArray();
                foreach (var item in list)
                {
                    WriteValue(item, truncateForDisplay);
                }
                WriteEndArray();
            }
            else if (value is IByteArrayValue byteArray /*&& truncateForDisplay //should use the entire byte array if 
                                                        * we're not truncating for display? Seems kind of unreasonable 
                                                        * for users to rely on binary data within a Struct value preview.*/)
            {
                const int byteArrayMaxStringLength = 24; //arbitrary number that I think looks good
                var byteArrayAsString = byteArray.ToStringTruncated(byteArrayMaxStringLength);
                WriteStringValue(byteArrayAsString);
            }
            else if (value is DateTime dt)
            {
                WriteStringValue(dt.ToString(Env.DateTimeFormat));
            }
            else if (value is DateOnly dateOnly)
            {
                WriteStringValue(dateOnly.ToString(Env.DateOnlyFormat));
            }
            else
            {
                //Everything else just try to write it as string
                WriteStringValue(value.ToString()!);
            }
        }
    }
}
