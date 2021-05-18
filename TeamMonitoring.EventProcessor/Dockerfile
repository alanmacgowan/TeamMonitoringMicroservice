#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["TeamMonitoring.EventProcessor/TeamMonitoring.EventProcessor.csproj", "TeamMonitoring.EventProcessor/"]
COPY ["TeamMonitoring.Common/TeamMonitoring.Common.csproj", "TeamMonitoring.Common/"]
COPY ["TeamMonitoring.Events/TeamMonitoring.Events.csproj", "TeamMonitoring.Events/"]
RUN dotnet restore "TeamMonitoring.EventProcessor/TeamMonitoring.EventProcessor.csproj"
COPY . .
WORKDIR "/src/TeamMonitoring.EventProcessor"
RUN dotnet build "TeamMonitoring.EventProcessor.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TeamMonitoring.EventProcessor.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TeamMonitoring.EventProcessor.dll"]