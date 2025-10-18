using WebCrawlerApp.Domain.Entities;

namespace WebCrawlerApp.Domain.Interfaces
{
    public interface ICrawlerPageRepository
    {
        Task AddAsync(CrawlerPage page, CancellationToken ct);
    }
}
