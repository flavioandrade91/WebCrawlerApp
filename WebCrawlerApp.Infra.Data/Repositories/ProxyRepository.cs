using System;
using WebCrawlerApp.Domain.Entities;
using WebCrawlerApp.Domain.Interfaces;
using WebCrawlerApp.Infra.Data.Contexts;

namespace WebCrawlerApp.Infra.Data.Repositories
{
    public class ProxyRepository : IProxyRepository
    {
        private readonly DataContext _db;
        public ProxyRepository(DataContext db) => _db = db;
        public async Task AddRangeAsync(IEnumerable<ProxyRecord> records, CancellationToken ct)
        {
            _db.ProxyRecords.AddRange(records);
            await _db.SaveChangesAsync(ct);
        }
    }
}
