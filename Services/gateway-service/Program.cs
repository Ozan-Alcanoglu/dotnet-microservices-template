using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ========== 1. OCELOT KONFİGÜRASYONUNU YÜKLEME ==========
// Ocelot.json dosyasını konfigürasyon sistemine ekle
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

// ========== 2. JWT AYARLARI (Gelen Token'ı Doğrulama) ==========
var jwtSettings = builder.Configuration.GetSection("JwtSettings");

// Secret Key, K8s Secret'tan veya appsettings.json'dan okunacak.
// Geliştirme ortamı için geçici bir varsayılan değer kullanılır.
var secret = jwtSettings["Secret"] ?? "BuCokGizliBirJWTAnahTaridirVeEnAzYirmiIkiKarakterOlmalidir"; 

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer("Bearer", options => // "Bearer" anahtarı Ocelot routes içinde kullanılacak
{
    // JWT Token'ın HTTPS üzerinden gelmesini zorunlu kılma (Geliştirmede false olabilir)
    options.RequireHttpsMetadata = false; 
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true, // Issuer (Token'ı üreten) doğrulanacak
        ValidateAudience = true, // Audience (Token'ı kullanan) doğrulanacak
        ValidateLifetime = true, // Süre (Expiry) doğrulanacak
        ValidateIssuerSigningKey = true, // Secret Key doğrulanacak
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret))
    };
});


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

// ========== 3. OCELOT SERVİSİNİ EKLEME ==========
builder.Services.AddOcelot();
builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseRouting();

app.MapPrometheusScrapingEndpoint();

app.MapHealthChecks("/health");
app.MapHealthChecks("/health/ready");
// ========== 4. OCELOT MIDDLEWARE'İNİ KULLANMA ==========
// JWT Doğrulaması Ocelot'tan önce yapılmalı
app.UseAuthentication(); 
app.UseAuthorization();

// Ocelot'u HTTP Pipeline'a ekle (Tüm yönlendirmeleri devralır)
await app.UseOcelot(); 

app.Run();