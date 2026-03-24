# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy everything FIRST
COPY . ./

# Restore dependencies
RUN dotnet restore MedLabAInsights.Api/MedLabAInsights.Api.csproj

# Build & publish
RUN dotnet publish MedLabAInsights.Api/MedLabAInsights.Api.csproj -c Release -o /app/out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/out .

# Expose port
EXPOSE 8080

ENTRYPOINT ["dotnet", "MedLabAInsights.Api.dll"]