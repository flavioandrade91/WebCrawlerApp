namespace WebCrawlerApp.Domain.Interfaces
{
    public interface IFileStorage
    {
        Task<string> SaveHtmlAsync(string fileName, string html, CancellationToken ct);
        Task<string> SaveJsonAsync(string fileName, string json, CancellationToken ct);
    }
}
