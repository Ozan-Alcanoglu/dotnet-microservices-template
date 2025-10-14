FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Proje dosyalarını kopyala
COPY ["CSMVCK8S.Shared/Shared.csproj", "CSMVCK8S.Shared/"]
COPY ["Services/product-service/product-service.csproj", "Services/product-service/"]

# Restore işlemi
RUN dotnet restore "CSMVCK8S.Shared/Shared.csproj"
RUN dotnet restore "Services/product-service/product-service.csproj"

# Tüm kodu kopyala
COPY . .

# Build ve publish
WORKDIR "/src/Services/product-service"
RUN dotnet build "product-service.csproj" -c Release -o /app/build
RUN dotnet publish "product-service.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "product-service.dll"]