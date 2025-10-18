using WebCrawlerApp.Domain.Interfaces;

namespace WebCrawlerApp.Infra.Data.Storage
{
    public class LocalFileStorage : IFileStorage
    {
        private readonly string _htmlDir;
        private readonly string _jsonDir;

        public LocalFileStorage(string htmlDir, string jsonDir)
        {
            _htmlDir = htmlDir;
            _jsonDir = jsonDir;
            Directory.CreateDirectory(_htmlDir);
            Directory.CreateDirectory(_jsonDir);
        }
        public async Task<string> SaveHtmlAsync(string fileName, string html, CancellationToken ct)
        {
            var path = Path.Combine(_htmlDir, fileName);
            await File.WriteAllTextAsync(path, html, ct);
            return path;
        }

        public async Task<string> SaveJsonAsync(string fileName, string json, CancellationToken ct)
        {
            var path = Path.Combine(_jsonDir, fileName);
            await File.WriteAllTextAsync(path, json, ct);
            return path;
        }
    }
}
