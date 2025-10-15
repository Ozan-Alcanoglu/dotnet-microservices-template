using Microsoft.EntityFrameworkCore;
using AuthService.Data;
using AuthService.Services;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using AuthService.Mapper;
using OpenTelemetry; // ⬅️ BUNU EKLEYİN
using OpenTelemetry.Metrics; // ⬅️ VEYA BUNU
using OpenTelemetry.Trace; 


var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("PostgreSqlConn");

// SERVİS EKLEME
builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseNpgsql(connectionString)); 

builder.Services.AddScoped<IAuthService, AuthService.Services.AuthService>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddOpenTelemetry()
    .WithMetrics(metrics => 
    {
        metrics.AddAspNetCoreInstrumentation();
        metrics.AddHttpClientInstrumentation();
        metrics.AddPrometheusExporter();
    })
    .WithTracing(tracing => 
    {
        tracing.AddAspNetCoreInstrumentation();
        tracing.AddHttpClientInstrumentation();
    });

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();



builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

var app = builder.Build();

// ✅ BU SATIR EKLENDİ - EN ÖNEMLİ KISIM ✅
app.UseRouting();

app.UseOpenTelemetryPrometheusScrapingEndpoint(); // ⬅️ BUNU EKLE

app.MapHealthChecks("/health");
app.MapHealthChecks("/health/ready");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(); 
}

app.UseAuthorization();

// ✅ BU DA ÖNEMLİ - MapControllers UseRouting'den SONRA gelmeli
app.MapControllers();

// ✅ MIGRATION EKLENDİ
using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
        context.Database.Migrate(); // Migration'ları uygula
        Console.WriteLine("✅ Auth Service Database migration successful!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Auth Service Migration failed: {ex.Message}");
    }
}

app.Run();