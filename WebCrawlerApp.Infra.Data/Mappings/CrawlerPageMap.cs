using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using WebCrawlerApp.Domain.Entities;

namespace WebCrawlerApp.Infra.Data.Mappings
{
    public class CrawlerPageMap : IEntityTypeConfiguration<CrawlerPage>
    {
        public void Configure(EntityTypeBuilder<CrawlerPage> builder)
        {
            builder.ToTable("CrawlerPages");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.PageNumber)
                   .IsRequired();

            builder.Property(x => x.HtmlFilePath)
                   .HasMaxLength(512)
                   .IsRequired();

            builder.Property(x => x.RowsExtracted)
                   .IsRequired();

            builder.HasOne(x => x.CrawlerRun)
                   .WithMany(r => r.Pages)
                   .HasForeignKey(x => x.CrawlerRunId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
