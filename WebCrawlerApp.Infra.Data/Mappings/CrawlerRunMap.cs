using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebCrawlerApp.Domain.Entities;

namespace WebCrawlerApp.Infra.Data.Mappings
{
    public class CrawlerRunMap : IEntityTypeConfiguration<CrawlerRun>
    {
        public void Configure(EntityTypeBuilder<CrawlerRun> builder)
        {
            builder.ToTable("CrawlerRuns");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.StartedAtUtc)
                   .IsRequired();

            builder.Property(x => x.FinishedAtUtc);

            builder.Property(x => x.PagesProcessed)
                   .IsRequired()
                   .HasDefaultValue(0);

            builder.Property(x => x.TotalRowsExtracted)
                   .IsRequired()
                   .HasDefaultValue(0);

            builder.Property(x => x.JsonFilePath)
                   .HasMaxLength(512);

            builder.Metadata.FindNavigation(nameof(CrawlerRun.Records))?.SetPropertyAccessMode(PropertyAccessMode.Field);
            builder.Metadata.FindNavigation(nameof(CrawlerRun.Pages))?.SetPropertyAccessMode(PropertyAccessMode.Field);

            builder.HasMany(x => x.Pages)
                   .WithOne(p => p.CrawlerRun!)
                   .HasForeignKey(p => p.CrawlerRunId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Records)
                   .WithOne(r => r.CrawlerRun!)
                   .HasForeignKey(r => r.CrawlerRunId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }

}
