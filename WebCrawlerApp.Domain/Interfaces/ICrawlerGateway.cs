namespace WebCrawlerApp.Domain.Interfaces
{
    public record PageResult(int PageNumber, string HtmlContent, IReadOnlyList<ProxyRawRow> Rows);
    public record ProxyRawRow(string IpAddress, int Port, string Country, string Protocol);

    public interface ICrawlerGateway
    {
        Task<int> DetectTotalPagesAsync(CancellationToken ct);
        Task<PageResult> FetchPageAsync(int pageNumber, CancellationToken ct);
    }
}
