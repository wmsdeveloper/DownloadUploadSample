using DownloadUploadSample.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace UnitTestScenario01 {
    public class UnitTestScenario01: IClassFixture<WebApplicationFactory<Program>> {

        #region [ Private Attributes ]

        private string url = "https://localhost:7175/main";
        private readonly WebApplicationFactory<Program> _factory;

        #endregion

        #region [ Constructor ]

        public UnitTestScenario01(WebApplicationFactory<Program> factory) {
            _factory = factory;
        }

        #endregion

        #region [ Test Methods ]

        [Trait("Category", " Unit Tests ")]
        [Fact(DisplayName = "Download a file")]
        public async Task Download() {
            /* Arrange */
            await using var stream = new MemoryStream(GetLargeFile());
            await using var writer = new StreamWriter(stream);
            await writer.WriteLineAsync("FILE CONTENTS");
            await writer.FlushAsync();
            stream.Position = 0;

            using var client = _factory.CreateDefaultClient();

            /* Act */
            using var responsePost =
                await client.PostAsync(
                    $"{url}/upload", 
                        new MultipartFormDataContent { { 
                                new StreamContent(stream), "file", "fileName" } });
            var fs = await DeserializarObjectResponse<FileSample>(responsePost);
            using var responseGet = await client.GetAsync($"{url}/download?name={fs.Name}");

            /* Assert */
            Assert.Equal(HttpStatusCode.OK, responsePost.StatusCode);
            Assert.Equal(HttpStatusCode.OK, responseGet.StatusCode);
        }

        [Trait("Category", " Unit Tests ")]
        [Fact(DisplayName = "Upload a file")]
        public async Task UploadAsync() {
            /* Arrange */
            await using var stream = new MemoryStream(GetLargeFile());
            await using var writer = new StreamWriter(stream);
            await writer.WriteLineAsync("FILE CONTENTS");
            await writer.FlushAsync();
            stream.Position = 0;

            using var client = _factory.CreateDefaultClient();

            /* Act */
            using var response =
                await client.PostAsync(
                    $"{url}/upload",
                    new MultipartFormDataContent
                    {
                        {
                            new StreamContent(stream),
                            "file",
                            "fileName"
                        }
                    });

            /* Assert */
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        #endregion

        #region [ Private Methods ]

        private byte[] GetLargeFile() {
            const string _imagename = "gitimage.png";
            return File.ReadAllBytes(Path.Combine(System.Environment.CurrentDirectory.Split("bin")[0], _imagename));
        }

        protected async Task<T> DeserializarObjectResponse<T>(HttpResponseMessage responseMessage) {
            var options = new JsonSerializerOptions {
                PropertyNameCaseInsensitive = true
            };

            return JsonSerializer.Deserialize<T>(await responseMessage.Content.ReadAsStringAsync(), options);
        }

        #endregion
    }
}