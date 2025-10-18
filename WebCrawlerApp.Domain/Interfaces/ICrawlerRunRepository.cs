using WebCrawlerApp.Domain.Entities;

namespace WebCrawlerApp.Domain.Interfaces
{
    public interface ICrawlerRunRepository
    {
        Task<CrawlerRun> AddAsync(CrawlerRun run, CancellationToken ct);
        Task UpdateAsync(CrawlerRun run, CancellationToken ct);
    }
}
