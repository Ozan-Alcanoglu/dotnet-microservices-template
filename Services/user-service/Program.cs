using Microsoft.EntityFrameworkCore;
using UserService.Data;
using UserService.Services;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using System;
using UserService.Clients;
using Polly;
using System.Net;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;
using MassTransit;
using UserService.Consumers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// 1. DbContext ve PostgreSQL Ayarı
builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSqlConn"))); 

// 2. Servis Enjeksiyonu
builder.Services.AddScoped<IUserService, UserService.Services.UserService>();

// 3. AutoMapper Ayarı (DTO <-> Model dönüşümü)
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// ========== OPEN TELEMETRY OBSERVABILITY ==========
builder.Services.AddOpenTelemetry()
    .WithMetrics(metrics => metrics
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddRuntimeInstrumentation() // BU PAKETİ EKLEDİK
    )
    .WithTracing(tracing => tracing
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
    );

// ========== IHttpClientFactory ve Polly Entegrasyonu ==========

// 4. AUTH CLIENT: Polly ile Korunan İstemci
builder.Services.AddHttpClient<IAuthClient, AuthClient>(client =>
{
    client.BaseAddress = new Uri("http://auth-service-api:8080"); 
})
.AddTransientHttpErrorPolicy(policy => 
    policy.WaitAndRetryAsync(3, retryAttempt => 
        TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
    )
);

// 5. PRODUCT CLIENT: Polly ile Korunan İstemci
builder.Services.AddHttpClient<IProductClient, ProductClient>(client =>
{
    client.BaseAddress = new Uri("http://product-service-api:8080"); 
})
.AddTransientHttpErrorPolicy(policy => 
    policy.WaitAndRetryAsync(3, retryAttempt => 
        TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
    )
);

// MassTransit configuration
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<PhotoStatusConsumer>(); // ⬅️ YENİ
    
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq-service", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });
        
        // Status güncelleme kuyruğu
        cfg.ReceiveEndpoint("photo-status-update-queue", e =>
        {
            e.ConfigureConsumer<PhotoStatusConsumer>(context);
        });
    });
});

// ========== KONTROLLER ve SWAGGER ==========

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseRouting();

// ========== PROMETHEUS METRICS ENDPOINT ==========
//app.MapPrometheusScrapingEndpoint(); // ← /metrics endpoint'ini açar

// Health Checks
app.MapHealthChecks("/health");
app.MapHealthChecks("/health/ready");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

// ✅ MIGRATION UYGULAMA
using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<UserDbContext>();
        context.Database.Migrate();
        Console.WriteLine("✅ User Service Database migration successful!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ User Service Migration failed: {ex.Message}");
    }
}

app.Run();