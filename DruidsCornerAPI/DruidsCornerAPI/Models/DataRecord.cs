namespace DruidsCornerAPI.Models
{

    public enum RecordKind
    {
        FileSource,
        CloudRecord,
        Unknown
    }

    public record DataRecord
    {
        public RecordKind Kind { get; set; } = RecordKind.Unknown;
    }

    public record FileRecord : DataRecord
    {
        public string Path { get; set; } = "";
    }

    public record CloudRecord : DataRecord
    {
        public string Id { get; set; } = "";

        public string Version { get; set; } = "";
    }
}
