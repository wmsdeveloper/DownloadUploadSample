using DownloadUploadSample.Models;
using Microsoft.AspNetCore.Mvc;

namespace DownloadUploadSample.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class MainController : ControllerBase {
        private const long MaxFileSize = 10L * 1024L * 1024L * 1024L; // 10GB, adjust to your need
        private static List<FileSample> _files = new();

        public MainController() { }

        [HttpGet("download")]
        public async Task<IActionResult> Download(string name) {
            var file = _files.Find(f => f.Name.Equals(name));
            return Ok(file);
        }

        [HttpPost("upload")]
        [RequestSizeLimit(MaxFileSize)]
        [RequestFormLimits(MultipartBodyLengthLimit = MaxFileSize)]
        public async Task<IActionResult> Upload([FromForm] IFormFile file) {
            byte[] buffer = new byte[file.Length];
            var resultInBytes = ConvertToBytes(file);
            Array.Copy(resultInBytes, buffer, resultInBytes.Length);
            var fs = new FileSample() {
                Name = file.Name,
                Data = resultInBytes
            };
            _files.Add(fs);
            return Ok(fs);
        }

        private byte[] ConvertToBytes(IFormFile image) {
            using (var memoryStream = new MemoryStream()) {
                image.OpenReadStream().CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

    }
}