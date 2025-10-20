using HtmlAgilityPack;
using System.Globalization;
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

        private static int? DecodeHexPort(string? hex)
        {
            if (string.IsNullOrWhiteSpace(hex)) return null;
            hex = hex.Trim();

            if (hex.Length % 2 == 1) hex = "0" + hex;

            if (hex.Length > 4)
                hex = hex.Substring(hex.Length - 4);

            if (int.TryParse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var value))
            {
                return value;
            }
            return null;
        }

        public async Task<PageResult> FetchPageAsync(int pageNumber, CancellationToken ct)
        {
        var url = pageNumber == 1 ? _baseUrl : $"{_baseUrl}/page/{pageNumber}";
        var html = await FetchHtmlAsync(url, ct);
        var doc = Load(html);

        var list = new List<ProxyRawRow>();

        var rows = doc.DocumentNode.SelectNodes("//table[.//th[contains(.,'IP Address')]]//tr[td]");
        if (rows != null)
        {
            foreach (var tr in rows)
            {
                var tds = tr.SelectNodes("./td");
                if (tds == null || tds.Count < 7) continue;

                var ip = tds[1].SelectSingleNode(".//a")?.InnerText?.Trim()
                         ?? tds[1].InnerText?.Trim();
                if (string.IsNullOrWhiteSpace(ip)) continue;

                var portHex = tds[2].SelectSingleNode(".//span[@class='port']")?.GetAttributeValue("data-port", null);
                var port = DecodeHexPort(portHex);
                if (port == null || port <= 0) continue;

                var country = HtmlEntity.DeEntitize(tds[3].InnerText).Trim();

                var protocol = HtmlEntity.DeEntitize(tds[6].InnerText).Trim();
                if (string.IsNullOrWhiteSpace(protocol)) protocol = "HTTP";

                list.Add(new ProxyRawRow(ip, port.Value, country, protocol));
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
