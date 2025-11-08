using System.Net.Http;

namespace TestChInvestBank.Services
{
    public class FileDownloader
    {
        private readonly HttpClient _client;

        public FileDownloader(HttpClient client)
        {
            _client = client;
        }

        public async Task DownloadFileAsync(string url, string outputPath)
        {
            Console.WriteLine($"> Загрузка файла: {url}");

            using var response = await _client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            await using var stream = await response.Content.ReadAsStreamAsync();
            await using var fileStream = File.Create(outputPath);
            await stream.CopyToAsync(fileStream);

            Console.WriteLine($"> Файл сохранён: {outputPath}");
        }
    }
}
