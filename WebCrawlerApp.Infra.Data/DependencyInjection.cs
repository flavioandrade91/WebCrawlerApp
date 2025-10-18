using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using WebCrawlerApp.Domain.Interfaces;
using WebCrawlerApp.Infra.Data.Contexts;
using WebCrawlerApp.Infra.Data.Gateway;
using WebCrawlerApp.Infra.Data.Repositories;
using WebCrawlerApp.Infra.Data.Storage;

namespace WebCrawlerApp.Infra.Data
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfraData(
        this IServiceCollection services,
        IConfiguration config)
        {
            services.AddDbContext<DataContext>(options =>
                options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

            // Repositórios
            services.AddScoped<ICrawlerRunRepository, CrawlerRunRepository>();
            services.AddScoped<IProxyRepository, ProxyRepository>();
            services.AddScoped<ICrawlerPageRepository, CrawlerPageRepository>();


            services.AddHttpClient<ICrawlerGateway, ProxyServersGateway>(client =>
            {
                client.Timeout = TimeSpan.FromSeconds(config.GetValue("Crawler:RequestTimeoutSeconds", 30));
                var ua = config.GetValue<string>("Crawler:UserAgent") ?? "Mozilla/5.0 (compatible; ProxyCrawler/1.0)";
                client.DefaultRequestHeaders.UserAgent.ParseAdd(ua);
            })
                .AddTypedClient((httpClient, sp) =>
            {
                var baseUrl = config.GetValue<string>("Crawler:BaseUrl")
                 ?? "https://proxyservers.pro/proxy/list/order/updated/order_dir/desc";
                return new ProxyServersGateway(httpClient, baseUrl);
            }); 

            // File storage
            var htmlDir = config.GetSection("Storage")["HtmlOutputDir"] ?? "storage/html";
            var jsonDir = config.GetSection("Storage")["JsonOutputDir"] ?? "storage/json";
            services.AddSingleton<IFileStorage>(new LocalFileStorage(htmlDir, jsonDir));

            return services;
        }
    }
}
