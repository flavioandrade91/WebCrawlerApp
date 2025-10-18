using Microsoft.EntityFrameworkCore;
using ProxyCrawler.Application.UseCases;
using System;
using WebCrawlerApp.Application.Config;
using WebCrawlerApp.Infra.Data;
using WebCrawlerApp.Infra.Data.Contexts;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Application layer registrations
var options = new CrawlerOptions
{
    MaxDegreeOfParallelism = builder.Configuration.GetValue<int>("Crawler:MaxDegreeOfParallelism", 3),
    JsonOutputPrefix = builder.Configuration.GetValue<string>("Storage:JsonPrefix") ?? "proxies_"
};
builder.Services.AddSingleton(options);
builder.Services.AddScoped<RunCrawlerUseCase>();

// Infra.Data
builder.Services.AddInfraData(builder.Configuration);

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

// CORS
const string CorsPolicy = "DefaultCorsPolicy";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: CorsPolicy, policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5050",   // se o Swagger UI estiver hospedado aqui
                "http://localhost:5000",   // exemplo de frontend local
                "https://localhost:5050")  // https também, se usar
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); // remova se não usar cookies/autenticação via navegador
    });
});

builder.Services.AddOpenApi();

var app = builder.Build();


// Configure the HTTP request pipeline.
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
