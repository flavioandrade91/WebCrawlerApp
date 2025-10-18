using WebCrawlerApp.Domain.Entities;
using WebCrawlerApp.Domain.Interfaces;
using WebCrawlerApp.Infra.Data.Contexts;

namespace WebCrawlerApp.Infra.Data.Repositories
{
    public class CrawlerPageRepository : ICrawlerPageRepository
    {
        private readonly DataContext _db;
        public CrawlerPageRepository(DataContext db) => _db = db;
        public async Task AddAsync(CrawlerPage page, CancellationToken ct)
        {
            _db.CrawlerPages.Add(page);
            await _db.SaveChangesAsync(ct);
        }
    }
}
