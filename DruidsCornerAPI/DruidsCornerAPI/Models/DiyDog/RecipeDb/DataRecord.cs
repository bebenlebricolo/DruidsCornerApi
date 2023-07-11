using System;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DruidsCornerAPI.Models.DiyDog.RecipeDb
{

    /// <summary>
    /// Encodes which kind or record we are facing (either a local file source or a Cloud Record)
    /// </summary>
    public enum RecordKind
    {
        /// <summary> File record (local database, json based) </summary>
        FileSource,    
        /// <summary> Cloud record (remote distributed database) </summary>
        CloudRecord,     
        /// <summary> Default value </summary>
        Unknown         
    }

    /// <summary>
    /// Duly duplicated from https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/converters-how-to?pivots=dotnet-7-0
    /// -> "Support Polymorphic Deserialization" paragraph
    /// </summary>
    public class DataRecordPolymorphicConverter : JsonConverter<DataRecord>
    {
        /// <summary>
        /// Checks the incoming type can be converted into the DataRecord.
        /// </summary>
        /// <param name="typeToConvert"></param>
        /// <returns></returns>
        public override bool CanConvert(Type typeToConvert)
        {
            return typeof(DataRecord).IsAssignableFrom(typeToConvert);
        }


        /// <summary>
        /// Reads a datarecord from a Json object (custom decoder)
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="typeToConvert"></param>
        /// <param name="options"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Polymorphically encodes the DataRecord based on the actual object type.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="record"></param>
        /// <param name="options"></param>
        public override void Write(Utf8JsonWriter writer, DataRecord record, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            record.WriteToJson(writer, options.PropertyNamingPolicy);
            writer.WriteEndObject();
        }
    }

   
    /// <summary>
    /// Base Data record class, used to provide an interface to the two underlying types
    /// </summary>
    public abstract class DataRecord
    {
        /// <summary>
        /// Data record underlying kind (typed structure)
        /// </summary>
        /// <value></value>
        public RecordKind Kind { get; set; } = RecordKind.Unknown;

        /// <summary>
        /// Reads from an UTF8 Json stream
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="reader"></param>
        /// <param name="namingPolicy"></param>
        public abstract void HandleJsonRead(string propertyName, Utf8JsonReader reader, JsonNamingPolicy? namingPolicy = null);

        
        /// <summary>
        /// Writes Object's data structure to an UTF8 Json Stream
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="namingPolicy"></param>
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

    /// <summary>
    /// File source record (Local database mode).
    /// Points to a file datastructure which is usually stored on disk
    /// </summary>
    public class FileRecord : DataRecord
    {
        /// <summary>
        /// Standard constructor
        /// </summary>
        public FileRecord()
        {
            Kind = RecordKind.FileSource;
        }

        /// <summary>
        /// Where to find the FileRecord on disk
        /// </summary>
        /// <value></value>
        public string Path { get; set; } = "";

        /// <summary>
        /// Parses an UTF8 Json stream and extracts data from it.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="reader"></param>
        /// <param name="namingPolicy"></param>
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

        /// <summary>
        /// Writes the object data structure to a Json UTF8 stream (using the provided writer)
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="namingPolicy"></param>
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

        /// <summary>
        /// Custom comparison operators
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object? obj)
        {
            if(obj is not FileRecord || obj is null)
            {
                return false;
            }
            var other = obj as FileRecord;
            return other!.Path == Path; ;
        }

        /// <summary>
        /// Custom hasher
        /// </summary>
        public override int GetHashCode()
        {
            return Kind.GetHashCode() * 32 + Path.GetHashCode();
        }
    }

    /// <summary>
    /// Represents a Cloud-based data record.
    /// This usually will point to a database document (like Firestore database)
    /// </summary>
    public class CloudRecord : DataRecord
    {
        /// <summary>
        /// Standard constructor
        /// </summary>
        public CloudRecord()
        {
            Kind = RecordKind.CloudRecord;
        }

        /// <summary>
        /// Record unique identifier 
        /// </summary>
        /// <value></value>
        public string Id { get; set; } = "";

        /// <summary>
        /// Record version (unique version)
        /// </summary>
        /// <value></value>
        public string Version { get; set; } = "";

        /// <summary>
        /// Reads from an UTF8 Json Stream
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="reader"></param>
        /// <param name="namingPolicy"></param>
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

        /// <summary>
        /// Writes to an UTF8 Json Stream
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="namingPolicy"></param>
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

        /// <summary>
        /// Custom comparison operator
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Custom hasher
        /// </summary>
        public override int GetHashCode()
        {
            return Id.GetHashCode() * 17 + Kind.GetHashCode();
        }
    }
}
