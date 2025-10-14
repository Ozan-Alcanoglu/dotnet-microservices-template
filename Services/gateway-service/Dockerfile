FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Proje dosyasını kopyala
COPY ["Services/gateway-service/gateway-service.csproj", "Services/gateway-service/"]

# Restore işlemi
RUN dotnet restore "Services/gateway-service/gateway-service.csproj"

# Tüm kodu kopyala (ocelot.json için)
COPY . .

# Build ve publish
WORKDIR "/src/Services/gateway-service"
RUN dotnet build "gateway-service.csproj" -c Release -o /app/build
RUN dotnet publish "gateway-service.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "gateway-service.dll"]