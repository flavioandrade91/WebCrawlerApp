using HtmlAgilityPack;
using System.Net.Http;
using WebCrawlerApp.Domain.Interfaces;
using WebCrawlerApp.Infra.Data.Contexts;

namespace WebCrawlerApp.Infra.Data.Gateway
{
    public class ProxyServersGateway : ICrawlerGateway
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public ProxyServersGateway(HttpClient httpClient, string baseUrl)
        {
            _httpClient = httpClient;
            _baseUrl = baseUrl.TrimEnd('/');
        }

        public async Task<int> DetectTotalPagesAsync(CancellationToken ct)
        {
            var html = await FetchHtmlAsync($"{_baseUrl}", ct);
            var doc = Load(html);

            // Ajuste o seletor de paginação conforme o HTML real:
            var nodes = doc.DocumentNode.SelectNodes("//ul[contains(@class,'pagination')]//a");
            if (nodes == null) return 1;

            int max = 1;
            foreach (var a in nodes)
            {
                if (int.TryParse(a.InnerText.Trim(), out var n) && n > max)
                    max = n;
            }
            return max;
        }

        public async Task<PageResult> FetchPageAsync(int pageNumber, CancellationToken ct)
        {
            var url = pageNumber == 1 ? _baseUrl : $"{_baseUrl}/page/{pageNumber}";
            var html = await FetchHtmlAsync(url, ct);
            var doc = Load(html);

            // Ajuste o seletor da tabela e ordem das colunas conforme o site:
            var rows = doc.DocumentNode.SelectNodes("//table[contains(@class,'table')]/tbody/tr");
            var list = new List<ProxyRawRow>();

            if (rows != null)
            {
                foreach (var tr in rows)
                {
                    var tds = tr.SelectNodes("./td");
                    if (tds == null || tds.Count() < 4) continue;

                    var ip = tds[0].InnerText.Trim();
                    var portText = tds[1].InnerText.Trim();
                    var country = tds[2].InnerText.Trim();
                    var protocol = tds[3].InnerText.Trim();

                    if (!int.TryParse(portText, out var port)) continue;

                    list.Add(new ProxyRawRow(ip, port, country, protocol));
                }
            }

            return new PageResult(pageNumber, html, list);
        }

        private async Task<string> FetchHtmlAsync(string url, CancellationToken ct)
        {
            var resp = await _httpClient.GetAsync(url, ct);
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadAsStringAsync(ct);
        }

        private static HtmlDocument Load(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            return doc;
        }
    }
}
