namespace DownloadUploadSample.Models {
    public class FileSample {
        public Guid Id { get; private set; }
        public DateTime CreatedTimestamp { get; set; }
        public string Name { get; set; } = string.Empty;
        public byte[]? Data { get; set; } = null;

        public FileSample() {
            Id = Guid.NewGuid();
            CreatedTimestamp = DateTime.Now;
        }
    }
}