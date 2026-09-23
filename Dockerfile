# syntax=docker/dockerfile:1.7
# Stage 1: build the static site with the .NET CLI generator
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Restore first for better layer caching
COPY MahjongDkStats.sln ./
COPY MahjongDkStats.CLI/MahjongDkStats.CLI.csproj MahjongDkStats.CLI/
COPY MahjongDkStatsCalculators/*.csproj MahjongDkStatsCalculators/
RUN dotnet restore

# Copy the rest of the sources and generate the site
COPY . .
RUN dotnet run --project MahjongDkStats.CLI --configuration Release

# Stage 2: serve /dist with caddy file-server
FROM caddy:2-alpine
COPY --from=build /src/dist /srv
CMD ["caddy", "file-server", "--root", "/srv", "--listen", ":80"]
