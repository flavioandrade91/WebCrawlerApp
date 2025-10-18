namespace WebCrawlerApp.Domain.Entities
{
    public class ProxyRecord
    {
        public int Id { get; private set; }
        public string IpAddress { get; private set; }
        public int Port { get; private set; }
        public string Country { get; private set; }
        public string Protocol { get; private set; }
        public int CrawlerRunId { get; private set; }
        public CrawlerRun? CrawlerRun { get; private set; } // ADICIONE ESTA LINHA

        private ProxyRecord() { } // EF
        public ProxyRecord(string ip, int port, string country, string protocol, int crawlerRunId)
        {
            IpAddress = ip;
            Port = port;
            Country = country;
            Protocol = protocol;
            CrawlerRunId = crawlerRunId;
        }
    }
}
