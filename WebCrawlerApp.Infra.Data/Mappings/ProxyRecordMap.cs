using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCrawlerApp.Domain.Entities;

namespace WebCrawlerApp.Infra.Data.Mappings
{
    public class ProxyRecordMap : IEntityTypeConfiguration<ProxyRecord> 
    {
        public void Configure(EntityTypeBuilder<ProxyRecord> builder)
        {
            builder.ToTable("ProxyRecords");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.IpAddress)
                   .HasMaxLength(64)
                   .IsRequired();

            builder.Property(x => x.Port)
                   .IsRequired();

            builder.Property(x => x.Country)
                   .HasMaxLength(128);

            builder.Property(x => x.Protocol)
                   .HasMaxLength(32);

            builder.HasIndex(x => new { x.IpAddress, x.Port, x.Protocol })
                   .HasDatabaseName("IX_Proxy_Unique");

            builder.HasOne(x => x.CrawlerRun)               // navegação
                   .WithMany(r => r.Records)
                   .HasForeignKey(x => x.CrawlerRunId)       // FK escalar
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
