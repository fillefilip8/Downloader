FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Downloader.csproj", "Downloader.csproj"]
RUN dotnet restore "Downloader.csproj"
COPY . .
RUN dotnet build "Downloader.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Downloader.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Downloader.dll"]
