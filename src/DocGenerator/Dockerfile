FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/DocGenerator/DocGenerator.csproj", "src/DocGenerator/"]
RUN dotnet restore "src/DocGenerator/DocGenerator.csproj"
COPY . .
WORKDIR "/src/src/DocGenerator"
RUN dotnet build "DocGenerator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DocGenerator.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DocGenerator.dll"]
