namespace WebCrawlerApp.Domain.Entities
{
    public class CrawlerPage
    {
        public int Id { get; private set; }
        public int PageNumber { get; private set; }
        public string HtmlFilePath { get; private set; } = string.Empty;
        public int RowsExtracted { get; private set; }
        public int CrawlerRunId { get; private set; }
        public CrawlerRun? CrawlerRun { get; private set; }
        private CrawlerPage() { }
        public CrawlerPage(int pageNumber, string htmlFilePath, int rowsExtracted, int crawlerRunId)
        {
            PageNumber = pageNumber;
            HtmlFilePath = htmlFilePath;
            RowsExtracted = rowsExtracted;
            CrawlerRunId = crawlerRunId;
        }
    }
}
