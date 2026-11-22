namespace UrbanHazardMap.Services
{
    public class EmailAttachment
    {
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } =
            "application/octet-stream";
        public Stream Content { get; set; } = Stream.Null;
    }
}