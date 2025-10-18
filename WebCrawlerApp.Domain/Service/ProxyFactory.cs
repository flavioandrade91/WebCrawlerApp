using WebCrawlerApp.Domain.Entities;

namespace WebCrawlerApp.Domain.Service
{
    public static class ProxyFactory
    {
        public static ProxyRecord Create(string ip, int port, string country, string protocol, int runId)
        {
            // Aqui caberiam validações de domínio (ex: IP válido, portas, protocolo permitido).
            return new ProxyRecord(ip, port, country, protocol, runId);
        }
    }
}
