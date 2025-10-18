using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebCrawlerApp.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddNavigations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CrawlerRuns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FinishedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PagesProcessed = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    TotalRowsExtracted = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    JsonFilePath = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CrawlerRuns", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CrawlerPages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PageNumber = table.Column<int>(type: "int", nullable: false),
                    HtmlFilePath = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    RowsExtracted = table.Column<int>(type: "int", nullable: false),
                    CrawlerRunId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CrawlerPages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CrawlerPages_CrawlerRuns_CrawlerRunId",
                        column: x => x.CrawlerRunId,
                        principalTable: "CrawlerRuns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProxyRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IpAddress = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Port = table.Column<int>(type: "int", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Protocol = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    CrawlerRunId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProxyRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProxyRecords_CrawlerRuns_CrawlerRunId",
                        column: x => x.CrawlerRunId,
                        principalTable: "CrawlerRuns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CrawlerPages_CrawlerRunId",
                table: "CrawlerPages",
                column: "CrawlerRunId");

            migrationBuilder.CreateIndex(
                name: "IX_Proxy_Unique",
                table: "ProxyRecords",
                columns: new[] { "IpAddress", "Port", "Protocol" });

            migrationBuilder.CreateIndex(
                name: "IX_ProxyRecords_CrawlerRunId",
                table: "ProxyRecords",
                column: "CrawlerRunId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CrawlerPages");

            migrationBuilder.DropTable(
                name: "ProxyRecords");

            migrationBuilder.DropTable(
                name: "CrawlerRuns");
        }
    }
}
