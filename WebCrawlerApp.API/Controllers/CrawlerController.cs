using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProxyCrawler.Application.UseCases;

namespace WebCrawlerApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CrawlerController : ControllerBase
    {
        private readonly RunCrawlerUseCase _useCase;
        public CrawlerController(RunCrawlerUseCase useCase) => _useCase = useCase;

        [HttpPost("run")]
        public async Task<IActionResult> Run(CancellationToken ct)
        {
            var runId = await _useCase.ExecuteAsync(ct);
            return Ok(new { runId });
        }
    }
}
