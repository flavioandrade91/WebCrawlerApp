using Microsoft.EntityFrameworkCore;
using ProxyCrawler.Application.UseCases;
using System;
using WebCrawlerApp.Application.Config;
using WebCrawlerApp.Infra.Data;
using WebCrawlerApp.Infra.Data.Contexts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var options = new CrawlerOptions
{
    MaxDegreeOfParallelism = builder.Configuration.GetValue<int>("Crawler:MaxDegreeOfParallelism", 3),
    JsonOutputPrefix = builder.Configuration.GetValue<string>("Storage:JsonPrefix") ?? "proxies_"
};
builder.Services.AddSingleton(options);
builder.Services.AddScoped<RunCrawlerUseCase>();

builder.Services.AddInfraData(builder.Configuration);

builder.Services.AddControllers();

const string CorsPolicy = "DefaultCorsPolicy";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: CorsPolicy, policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5050",   
                "http://localhost:5000",   
                "https://localhost:5050")  
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); 
    });
});

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(CorsPolicy);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
