FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# DÜZELTİLMİŞ YOLLAR
COPY ["CSMVCK8S.Shared/Shared.csproj", "CSMVCK8S.Shared/"]
COPY ["Services/auth-service/auth-service.csproj", "Services/auth-service/"]

RUN dotnet restore "CSMVCK8S.Shared/Shared.csproj"
RUN dotnet restore "Services/auth-service/auth-service.csproj"

COPY . .
WORKDIR "/src/Services/auth-service"
RUN dotnet build "auth-service.csproj" -c Release -o /app/build
RUN dotnet publish "auth-service.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "auth-service.dll"]