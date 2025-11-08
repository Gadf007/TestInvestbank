using TestChInvestBank.Services;
using System.Net.Http;
using System.Security.Authentication;
using System.Net;

Console.OutputEncoding = System.Text.Encoding.UTF8;

Console.WriteLine("=== ГАР Обновление / Конвертация ===");



//ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;

var handler = new HttpClientHandler
{
    SslProtocols = SslProtocols.Tls12,
    CheckCertificateRevocationList = true,
    UseProxy = false,
    AllowAutoRedirect = true
};

handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

var httpClient = new HttpClient(handler, disposeHandler: true)
{
    Timeout = TimeSpan.FromSeconds(60)
};

var api = new FiasApiClient(httpClient);

//var httpClient = new HttpClient(handler);
//var api = new FiasApiClient(httpClient);
var downloader = new FileDownloader(httpClient);
var extractor = new ArchiveExtractor();
var converter = new XmlToCsvConverter();

try
{
    Console.WriteLine("> Получение информации о последней версии...");
    var info = await api.GetLastVersionInfoAsync();

    if (info == null || string.IsNullOrEmpty(info.GarXMLDeltaURL))
    {
        Console.WriteLine("Не удалось получить ссылку на gar_delta_xml.zip");
        return;
    }

    var workDir = Path.Combine(Directory.GetCurrentDirectory(), "data");
    var zipPath = Path.Combine(workDir, "gar_delta_xml.zip");
    var extractDir = Path.Combine(workDir, "extracted");
    var csvOutput = Path.Combine(workDir, "RESULT.csv");
    var xsdDir = Path.Combine(workDir, "xsd"); // пользователь должен положить XSD сюда

    Directory.CreateDirectory(workDir);

    await downloader.DownloadFileAsync(info.GarXMLDeltaURL, zipPath);
    extractor.Extract(zipPath, extractDir);

    //ввод наименования XML
    string? xmlName;
    do
    {
        Console.Write($"> Введите наименование любого из XML файлов расположенных по пути:\n{extractDir}\n> ");
        xmlName = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(xmlName))
        {
            Console.WriteLine("Ошибка! Поле не может быть пустым. Попробуйте снова.");
        }
    }
    while (string.IsNullOrWhiteSpace(xmlName));
    


    string xmlFilePath = Directory.GetFiles(extractDir, xmlName, SearchOption.AllDirectories).First(); // путь файла для конвертации
    Console.WriteLine($"> Данный файл расположен по пути: {xmlFilePath}");

    converter.ConvertDynamic(xmlFilePath, xsdDir, csvOutput);


    Console.WriteLine("> === Все операции успешно выполнены! ===");
}
catch (Exception ex)
{
    Console.WriteLine($"Ошибка: {ex.Message}");
}
