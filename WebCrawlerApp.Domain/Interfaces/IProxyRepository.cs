using WebCrawlerApp.Domain.Entities;

namespace WebCrawlerApp.Domain.Interfaces
{
    public interface IProxyRepository
    {
        Task AddRangeAsync(IEnumerable<ProxyRecord> records, CancellationToken ct);
    }
}
