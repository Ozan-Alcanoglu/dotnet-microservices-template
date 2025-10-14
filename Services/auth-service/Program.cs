using Microsoft.EntityFrameworkCore;
using AuthService.Data;
using AuthService.Services;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using AuthService.Mapper;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("PostgreSqlConn");

// SERVİS EKLEME
builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseNpgsql(connectionString)); 

builder.Services.AddScoped<IAuthService, AuthService.Services.AuthService>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService("auth-service"))
    .WithMetrics(metrics => 
    {
        // METRICS instrumentation'ları burada
        metrics.AddAspNetCoreInstrumentation();
        metrics.AddHttpClientInstrumentation();
        metrics.AddConsoleExporter();
    })
    .WithTracing(tracing => 
    {
        // TRACING instrumentation'ları burada  
        tracing.AddAspNetCoreInstrumentation();
        tracing.AddHttpClientInstrumentation();
        tracing.AddConsoleExporter();
    });



builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

var app = builder.Build();

// ✅ BU SATIR EKLENDİ - EN ÖNEMLİ KISIM ✅
app.UseRouting();



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