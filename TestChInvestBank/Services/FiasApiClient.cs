using System.Net.Http;
using System.Text.Json;
using TestChInvestBank.Models;

namespace TestChInvestBank.Services
{
    public class FiasApiClient
    {
        private readonly HttpClient _client;

        public FiasApiClient(HttpClient client)
        {
            _client = client;
        }



        // Получаем JSON с метаданными о версии
        public async Task<DownloadFileInfo?> GetLastVersionInfoAsync()
        {
            int retries = 3;

            for (int attempt = 1; attempt <= retries; attempt++)
            {
                try
                {
                    var url = "https://fias.nalog.ru/WebServices/Public/GetLastDownloadFileInfo";
                    var response = await _client.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<DownloadFileInfo>(json,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
                catch (HttpRequestException ex) when (ex.InnerException is IOException)
                {
                    Console.WriteLine($"Попытка {attempt}: ошибка SSL — {ex.Message}");
                    if (attempt == retries)
                        throw;
                    await Task.Delay(1000 * attempt); // экспоненциальная пауза
                }
            }

            return null;

        }



        // Скачиваем архив по ссылке
        public async Task DownloadGarDeltaAsync(string url, string savePath)
        {
            Console.WriteLine($"> Загрузка файла с {url}");

            using var response = await _client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            await using var inputStream = await response.Content.ReadAsStreamAsync();
            await using var outputStream = File.Create(savePath);
            await inputStream.CopyToAsync(outputStream);

            Console.WriteLine($"> Файл сохранён в {savePath}");
        }

    }
}
