using Microsoft.EntityFrameworkCore;
using ProductService.Data;
using ProductService.Services;
using System.Text.Json.Serialization;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using MassTransit;
using CSMVCK8S.Shared.Events;
using ProductService.Consumers;  // ⬅️ BUNU EKLE
using Minio;                     // ⬅️ BUNU EKLE
// OPEN TELEMETRY USING'LERİNİ EKLE:
using OpenTelemetry; // ⬅️ BU DA GEREKLİ
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("PostgreSqlConn");

// ========== SERVİS EKLEME BAŞLANGIÇ ==========

// 1. DbContext ve PostgreSQL Ayarı
builder.Services.AddDbContext<ProductDbContext>(options =>
    options.UseNpgsql(connectionString)); 

// 2. Servis Enjeksiyonu
builder.Services.AddScoped<IProductService, ProductService.Services.ProductService>();

// 3. AutoMapper Ayarı
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// 4. OpenTelemetry - DÜZGÜN VERSİYON
// 4. OpenTelemetry - DÜZGÜN VERSİYON
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService("ProductService"))
    .WithMetrics(metrics => metrics
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddRuntimeInstrumentation()
        .AddPrometheusExporter() // ⬅️ BUNU EKLEYİN!
    )
    .WithTracing(tracing => tracing
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
    );

// 5. MinIO Client
// MinIO Client - INTERFACE olarak kaydet
builder.Services.AddSingleton<IMinioClient>(provider =>
{
    return new MinioClient()
        .WithEndpoint("minio-service:9000")
        .WithCredentials("admin", "password123")
        .WithSSL(false)
        .Build();
});

// 6. MassTransit - RABBITMQ
builder.Services.AddMassTransit(x =>
{
    // Tüketici sınıflarını kaydet
    x.AddConsumer<UserCreatedConsumer>();
    x.AddConsumer<PhotoValidationConsumer>();    // ⬅️ BUNU EKLE
    x.AddConsumer<PhotoUploadConsumer>();        // ⬅️ BUNU EKLE

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq-service", h =>
        {
            h.Username("guest"); 
            h.Password("guest");
        });

        // User Created Event kuyruğu
        cfg.ReceiveEndpoint("product-service-user-created", e =>
        {
            e.ConfigureConsumer<UserCreatedConsumer>(context);
        });

        // Photo Validation kuyruğu    ⬅️ BUNU EKLE
        cfg.ReceiveEndpoint("photo-validation-queue", e =>
        {
            e.ConfigureConsumer<PhotoValidationConsumer>(context);
        });

        // Photo Upload kuyruğu        ⬅️ BUNU EKLE
        cfg.ReceiveEndpoint("photo-upload-queue", e =>
        {
            e.ConfigureConsumer<PhotoUploadConsumer>(context);
        });
    });
});

builder.Services.AddControllers();

// 7. Swagger/OpenAPI Desteği
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();
// ========== SERVİS EKLEME BİTİŞ ==========

var app = builder.Build();

app.UseOpenTelemetryPrometheusScrapingEndpoint(); // ⬅️ BUNU EKLE

app.UseRouting();

app.MapHealthChecks("/health");
app.MapHealthChecks("/health/ready");

// Geliştirme ortamında Swagger UI'ı etkinleştir
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(); 
}

app.UseAuthorization();
app.MapControllers();

// ✅ MIGRATION EKLENDİ
using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<ProductDbContext>();
        context.Database.Migrate(); // Migration'ları uygula
        Console.WriteLine("✅ Product Service Database migration successful!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Product Service Migration failed: {ex.Message}");
    }
}

app.Run();