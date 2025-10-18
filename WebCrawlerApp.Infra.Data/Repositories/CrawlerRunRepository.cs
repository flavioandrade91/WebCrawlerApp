using System;
using WebCrawlerApp.Domain.Entities;
using WebCrawlerApp.Domain.Interfaces;
using WebCrawlerApp.Infra.Data.Contexts;

namespace WebCrawlerApp.Infra.Data.Repositories
{
    public class CrawlerRunRepository : ICrawlerRunRepository
    {
        private readonly DataContext _db;
        public CrawlerRunRepository(DataContext db) => _db = db;

        public async Task<CrawlerRun> AddAsync(CrawlerRun run, CancellationToken ct)
        {
            _db.CrawlerRuns.Add(run);
            await _db.SaveChangesAsync(ct);
            return run;
        }

        public async Task UpdateAsync(CrawlerRun run, CancellationToken ct)
        {
            _db.CrawlerRuns.Update(run);
            await _db.SaveChangesAsync(ct);
        }
    }
}
