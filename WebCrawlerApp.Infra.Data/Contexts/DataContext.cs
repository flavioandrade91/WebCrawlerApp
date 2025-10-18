using Microsoft.EntityFrameworkCore;
using System;
using WebCrawlerApp.Domain.Entities;
using WebCrawlerApp.Infra.Data.Mappings;

namespace WebCrawlerApp.Infra.Data.Contexts
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<ProxyRecord> ProxyRecords => Set<ProxyRecord>();
        public DbSet<CrawlerRun> CrawlerRuns => Set<CrawlerRun>();
        public DbSet<CrawlerPage> CrawlerPages => Set<CrawlerPage>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new ProxyRecordMap());
            modelBuilder.ApplyConfiguration(new CrawlerRunMap());
            modelBuilder.ApplyConfiguration(new CrawlerPageMap());

        }
    }
}
