#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["TeamMonitoring.TeamService.API/TeamMonitoring.TeamService.API.csproj", "TeamMonitoring.TeamService.API/"]
RUN dotnet restore "TeamMonitoring.TeamService.API/TeamMonitoring.TeamService.API.csproj"
COPY . .
WORKDIR "/src/TeamMonitoring.TeamService.API"
RUN dotnet build "TeamMonitoring.TeamService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TeamMonitoring.TeamService.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TeamMonitoring.TeamService.API.dll"]