using System;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DruidsCornerAPI.Models.DiyDog
{

    public enum RecordKind
    {
        FileSource,
        CloudRecord,
        Unknown
    }

    /// <summary>
    /// Duly duplicated from https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/converters-how-to?pivots=dotnet-7-0
    /// -> "Support Polymorphic Deserialization" paragraph
    /// </summary>
    public class DataRecordPolymorphicConverter : JsonConverter<DataRecord>
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeof(DataRecord).IsAssignableFrom(typeToConvert);
        }


        public override DataRecord Read( ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            reader.Read();
            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException();
            }

            // Reading the Kind of data record we are facing
            var encodedPropName = (options.PropertyNamingPolicy ?? JsonNamingPolicy.CamelCase).ConvertName(nameof(DataRecord.Kind));
            string? propertyName = reader.GetString();
            if (propertyName != encodedPropName)
            {
                throw new JsonException();
            }

            // Onto the next value, Python presents the value as a string
            reader.Read();
            if (reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException();
            }

            // Decypher Record kind
            if(false == Enum.TryParse<RecordKind>(reader.GetString(), out RecordKind kind))
            {
                throw new JsonException($"Could not parse {typeof(RecordKind)} from Json object");
            }

            DataRecord record;
            switch(kind)
            {
                case RecordKind.FileSource:
                    record = new FileRecord();
                    break;
                
                case RecordKind.CloudRecord:
                    record = new CloudRecord();
                    break;

                default:
                    throw new JsonException();
            }

            // Custom properties deserialization
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return record;
                }

                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    propertyName = reader.GetString();
                    if(null == propertyName) 
                    {
                        throw new JsonException();
                    }
                    reader.Read();

                    // Delegate the task of translating to derived classes
                    record.HandleJsonRead(propertyName, reader, options.PropertyNamingPolicy);
                }
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, DataRecord record, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            record.WriteToJson(writer, options.PropertyNamingPolicy);
            writer.WriteEndObject();
        }
    }

   
    public abstract class DataRecord
    {
        public RecordKind Kind { get; set; } = RecordKind.Unknown;

        public abstract void HandleJsonRead(string propertyName, Utf8JsonReader reader, JsonNamingPolicy? namingPolicy = null);

        public virtual void WriteToJson(Utf8JsonWriter writer, JsonNamingPolicy? namingPolicy = null)
        {
            if(namingPolicy == null)
            {
                namingPolicy = JsonNamingPolicy.CamelCase;
            }
            var encodedPropName = namingPolicy.ConvertName(nameof(Kind));
            writer.WriteString(encodedPropName, Kind.ToString());
        }

    }

    public class FileRecord : DataRecord
    {
        public FileRecord()
        {
            Kind = RecordKind.FileSource;
        }

        public string Path { get; set; } = "";

        public override void HandleJsonRead(string propertyName, Utf8JsonReader reader, JsonNamingPolicy? namingPolicy = null ) 
        {
            if(namingPolicy == null)
            {
                namingPolicy = JsonNamingPolicy.CamelCase;
            }

            if(propertyName != namingPolicy.ConvertName(nameof(Path)))
            {
                throw new JsonException($"Cannot read property {propertyName}");
            }
            Path = reader.GetString() ?? "";
        }

        public override void WriteToJson(Utf8JsonWriter writer, JsonNamingPolicy? namingPolicy = null)
        {
            if (namingPolicy == null)
            {
                namingPolicy = JsonNamingPolicy.CamelCase;
            }
            base.WriteToJson(writer, namingPolicy);
            var encodedPathProp = namingPolicy.ConvertName(nameof(Path));
            writer.WriteString(encodedPathProp, Path);
        }

        public override bool Equals(object? obj)
        {
            if(obj is not FileRecord || obj is null)
            {
                return false;
            }
            var other = obj as FileRecord;
            return other!.Path == Path; ;
        }
    }

    public class CloudRecord : DataRecord
    {
        public CloudRecord()
        {
            Kind = RecordKind.CloudRecord;
        }

        public string Id { get; set; } = "";

        public string Version { get; set; } = "";

        public override void HandleJsonRead(string propertyName, Utf8JsonReader reader, JsonNamingPolicy? namingPolicy = null)
        {
            if (namingPolicy == null)
            {
                namingPolicy = JsonNamingPolicy.CamelCase;
            }

            var encodedProps = new List<string>
            {
                namingPolicy.ConvertName(nameof(Id)),
                namingPolicy.ConvertName(nameof(Version))
            };


            // Reject foreign keys
            if(!encodedProps.Contains(propertyName))
            {
                throw new JsonException($"Property name {propertyName} is not recognized");
            }

            if(propertyName == namingPolicy.ConvertName(nameof(Id)))
            {
                Id = reader.GetString() ?? "";
            }

            if (propertyName == namingPolicy.ConvertName(nameof(Version)))
            {
               Version = reader.GetString() ?? "";
            }
        }

        public override void WriteToJson(Utf8JsonWriter writer, JsonNamingPolicy? namingPolicy = null)
        {
            if (namingPolicy == null)
            {
                namingPolicy = JsonNamingPolicy.CamelCase;
            }
            base.WriteToJson(writer, namingPolicy);
            var encodedIdProp = namingPolicy.ConvertName(nameof(Id));
            var encodedVersionProp = namingPolicy.ConvertName(nameof(Version));
            writer.WriteString(encodedIdProp, Id);
            writer.WriteString(encodedVersionProp, Version);
        }

        public override bool Equals(object? obj)
        {
            if (obj is not CloudRecord || obj is null)
            {
                return false;
            }
            var other = obj as CloudRecord;
            var same = other!.Id == Id;
            same &= other!.Version == Version;
            return same ;
        }
    }
}
