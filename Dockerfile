FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
RUN apt-get update && apt-get install -y libgssapi-krb5-2 && rm -rf /var/lib/apt/lists/*

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["src/CatalogoProductos.API/CatalogoProductos.API.csproj", "src/CatalogoProductos.API/"]
COPY ["src/CatalogoProductos.Application/CatalogoProductos.Application.csproj", "src/CatalogoProductos.Application/"]
COPY ["src/CatalogoProductos.Domain/CatalogoProductos.Domain.csproj", "src/CatalogoProductos.Domain/"]
COPY ["src/CatalogoProductos.Infrastructure/CatalogoProductos.Infrastructure.csproj", "src/CatalogoProductos.Infrastructure/"]
RUN dotnet restore "src/CatalogoProductos.API/CatalogoProductos.API.csproj"
COPY . .
WORKDIR "/src/src/CatalogoProductos.API"
RUN dotnet build "CatalogoProductos.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CatalogoProductos.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CatalogoProductos.API.dll"]
