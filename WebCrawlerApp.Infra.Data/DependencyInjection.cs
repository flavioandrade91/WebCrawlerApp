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

            // Gateways
            //services.AddHttpClient<ICrawlerGateway, ProxyServersGateway>(client =>
            //{
            //    client.Timeout = TimeSpan.FromSeconds(config.GetValue("Crawler:RequestTimeoutSeconds", 30));
            //    var ua = config.GetValue<string>("Crawler:UserAgent") ?? "Mozilla/5.0 (compatible; ProxyCrawler/1.0)";
            //    client.DefaultRequestHeaders.UserAgent.ParseAdd(ua);
            //})
            //    .AddTypedClient((httpClient, sp) =>
            //{
            //    var baseUrl = config.GetValue<string>("Crawler:BaseUrl")
            //     ?? "https://proxyservers.pro/proxy/list/order/updated/order_dir/desc";
            //    return new ProxyServersGateway(httpClient, baseUrl);
            //}); 

            // Garante IHttpClientFactory registrado
            services.AddHttpClient();

            // Registra ICrawlerGateway com factory explícita (injeta o baseUrl manualmente)
            services.AddScoped<ICrawlerGateway>(sp =>
            {
                var cfg = sp.GetRequiredService<IConfiguration>();
                var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
                var httpClient = httpClientFactory.CreateClient();

                // Configura HttpClient a partir do appsettings
                httpClient.Timeout = TimeSpan.FromSeconds(cfg.GetValue<int>("Crawler:RequestTimeoutSeconds", 120));
                var ua = cfg.GetValue<string>("Crawler:UserAgent") ?? "Mozilla/5.0 (compatible; ProxyCrawler/1.0)";
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(ua);

                // Lê o BaseUrl (com fallback)
                var baseUrl = cfg.GetValue<string>("Crawler:BaseUrl")
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
