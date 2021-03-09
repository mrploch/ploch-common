#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["TestApps/TestApps.WorkerServiceDaemonApp/TestApps.WorkerServiceDaemonApp.csproj", "TestApps/TestApps.WorkerServiceDaemonApp/"]
RUN dotnet restore "TestApps/TestApps.WorkerServiceDaemonApp/TestApps.WorkerServiceDaemonApp.csproj"
COPY . .
WORKDIR "/src/TestApps/TestApps.WorkerServiceDaemonApp"
RUN dotnet build "TestApps.WorkerServiceDaemonApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TestApps.WorkerServiceDaemonApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TestApps.WorkerServiceDaemonApp.dll"]