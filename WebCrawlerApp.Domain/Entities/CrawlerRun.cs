namespace WebCrawlerApp.Domain.Entities
{
    public class CrawlerRun
    {
        public int Id { get; private set; }
        public DateTime StartedAtUtc { get; private set; }
        public DateTime? FinishedAtUtc { get; private set; }
        public int PagesProcessed { get; private set; }
        public int TotalRowsExtracted { get; private set; }
        public string JsonFilePath { get; private set; } = string.Empty;

        private readonly List<ProxyRecord> _records = new();
        public IReadOnlyCollection<ProxyRecord> Records => _records.AsReadOnly();

        private readonly List<CrawlerPage> _pages = new();
        public IReadOnlyCollection<CrawlerPage> Pages => _pages.AsReadOnly();

        private CrawlerRun() { }
        public CrawlerRun(DateTime startedAtUtc) => StartedAtUtc = startedAtUtc;

        public void MarkFinished(int pagesProcessed, int totalRows, string jsonPath)
        {
            PagesProcessed = pagesProcessed;
            TotalRowsExtracted = totalRows;
            JsonFilePath = jsonPath;
            FinishedAtUtc = DateTime.UtcNow;
        }

        public void AddPage(CrawlerPage page) => _pages.Add(page);
        public void AddRecords(IEnumerable<ProxyRecord> recs) => _records.AddRange(recs);
    }
}
