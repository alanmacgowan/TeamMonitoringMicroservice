#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["TeamMonitoring.LocationReporter.API/TeamMonitoring.LocationReporter.API.csproj", "TeamMonitoring.LocationReporter.API/"]
COPY ["TeamMonitoring.Common/TeamMonitoring.Common.csproj", "TeamMonitoring.Common/"]
COPY ["TeamMonitoring.Events/TeamMonitoring.Events.csproj", "TeamMonitoring.Events/"]
RUN dotnet restore "TeamMonitoring.LocationReporter.API/TeamMonitoring.LocationReporter.API.csproj"
COPY . .
WORKDIR "/src/TeamMonitoring.LocationReporter.API"
RUN dotnet build "TeamMonitoring.LocationReporter.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TeamMonitoring.LocationReporter.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TeamMonitoring.LocationReporter.API.dll"]