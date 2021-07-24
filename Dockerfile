FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal AS base
WORKDIR /app
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNET_ENVIRONMENT=http://+:80

FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build
WORKDIR /src

COPY ["./Catalog.Core/Catalog.Core.csproj", "./Catalog.Core/"]
COPY ["./Catalog.Persistence.MongoDb/Catalog.Persistence.MongoDb.csproj", "./Catalog.Persistence.MongoDb/"]
COPY ["./Catalog.Api/Catalog.Api.csproj", "./Catalog.Api/"]

RUN dotnet restore "./Catalog.Core/Catalog.Core.csproj"
RUN dotnet restore "./Catalog.Persistence.MongoDb/Catalog.Persistence.MongoDb.csproj"
RUN dotnet restore "./Catalog.Api/Catalog.Api.csproj"

COPY . .

RUN dotnet publish "./Catalog.Api/Catalog.Api.csproj" -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Catalog.Api.dll"]
