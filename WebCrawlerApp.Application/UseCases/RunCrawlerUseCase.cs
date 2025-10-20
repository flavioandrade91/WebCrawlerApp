using System.Text.Json;
using WebCrawlerApp.Application.Config;
using WebCrawlerApp.Domain.Entities;
using WebCrawlerApp.Domain.Interfaces;
using WebCrawlerApp.Domain.Service;

namespace ProxyCrawler.Application.UseCases;

public class RunCrawlerUseCase
{
    private readonly ICrawlerGateway _gateway;
    private readonly IFileStorage _storage;
    private readonly ICrawlerRunRepository _runRepo;
    private readonly IProxyRepository _proxyRepo;
    private readonly ICrawlerPageRepository _pageRepo;
    private readonly CrawlerOptions _options;

    public RunCrawlerUseCase(
        ICrawlerGateway gateway,
        IFileStorage storage,
        ICrawlerRunRepository runRepo,
        IProxyRepository proxyRepo,
        ICrawlerPageRepository pageRepo,
        CrawlerOptions options)
    {
        _gateway = gateway;
        _storage = storage;
        _runRepo = runRepo;
        _proxyRepo = proxyRepo;
        _pageRepo = pageRepo;
        _options = options;
    }

    public async Task<int> ExecuteAsync(CancellationToken ct = default)
    {
        var run = new CrawlerRun(DateTime.UtcNow);
        run = await _runRepo.AddAsync(run, ct);

        var totalPages = await _gateway.DetectTotalPagesAsync(ct);
        if (totalPages < 1) totalPages = 1;

        var semaphore = new SemaphoreSlim(_options.MaxDegreeOfParallelism, _options.MaxDegreeOfParallelism);
        var allRecords = new List<ProxyRecord>();
        var tasks = new List<Task>();

        
        for (int page = 1; page <= totalPages; page++)
        {
            await semaphore.WaitAsync();
            var pageNumber = page;

            tasks.Add(Task.Run(async () =>
            {
                try
                {
                    using var cts = CancellationTokenSource.CreateLinkedTokenSource(CancellationToken.None);
                    cts.CancelAfter(TimeSpan.FromSeconds(30));

                    var pageResult = await _gateway.FetchPageAsync(pageNumber, cts.Token);

                    var htmlPath = await _storage.SaveHtmlAsync($"proxies_page_{pageNumber}.html", pageResult.HtmlContent, CancellationToken.None);

                    var recs = pageResult.Rows
                        .Select(r => ProxyFactory.Create(r.IpAddress, r.Port, r.Country, r.Protocol, run.Id))
                        .ToList();

                    lock (allRecords) { allRecords.AddRange(recs); }

                    var pageEntity = new CrawlerPage(pageNumber, htmlPath, recs.Count, run.Id);
                    await _pageRepo.AddAsync(pageEntity, CancellationToken.None);

                    Console.WriteLine($"Página {pageNumber}: {recs.Count} proxies");
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine($"Página {pageNumber}: timeout/cancel ao buscar HTML.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Falha na página {pageNumber}: {ex.Message}");
                }
                finally
                {
                    semaphore.Release();
                }
            }));
        }

        await Task.WhenAll(tasks);

        var fileName = $"{_options.JsonOutputPrefix}{DateTime.UtcNow:yyyyMMdd_HHmmss}.json";
        var json = JsonSerializer.Serialize(
            allRecords.Select(r => new { r.IpAddress, r.Port, r.Country, r.Protocol }),
            new JsonSerializerOptions { WriteIndented = true });

        var jsonPath = await _storage.SaveJsonAsync(fileName, json, CancellationToken.None);
        await _proxyRepo.AddRangeAsync(allRecords, CancellationToken.None);

        run.MarkFinished(totalPages, allRecords.Count, jsonPath);
        await _runRepo.UpdateAsync(run, CancellationToken.None);

        return run.Id;
    }
}