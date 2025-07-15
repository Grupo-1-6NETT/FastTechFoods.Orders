# FASTTECHFOODS.ORDERS
#BASE
FROM mcr.microsoft.com/dotnet/aspnet:8.0-bookworm-slim AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# BUILD
FROM mcr.microsoft.com/dotnet/sdk:8.0-bookworm-slim AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Orders.API/Orders.API.csproj", "src/Orders.API/"]
COPY ["src/Orders.Application/Orders.Application.csproj", "src/Orders.Application/"]
COPY ["src/Orders.Domain/Orders.Domain.csproj", "src/Orders.Domain/"]
COPY ["src/Orders.Infrastructure/Orders.Infrastructure.csproj", "src/Orders.Infrastructure/"]
RUN dotnet restore "./src/Orders.API/Orders.API.csproj"
COPY . .
WORKDIR "/src/src/Orders.API"
RUN dotnet build "./Orders.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# PUBLISH
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Orders.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# RUN
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Orders.API.dll"]