# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["MedLabAInsights.Api/MedLabAInsights.Api.csproj", "MedLabAInsights.Api/"]
COPY ["../MedLabAInsights.Data/MedLabAInsights.Data.csproj", "MedLabAInsights.Data/"]
COPY ["../MedLabAInsights.Models/MedLabAInsights.Models.csproj", "MedLabAInsights.Models/"]

RUN dotnet restore "MedLabAInsights.Api/MedLabAInsights.Api.csproj"

COPY . .

WORKDIR "/src/MedLabAInsights.Api"
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 10000

ENTRYPOINT ["dotnet", "MedLabAInsights.Api.dll"]