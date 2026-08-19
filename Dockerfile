# Stage 1: build — big image with the compiler
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY DocQuery.sln .
COPY src/DocQuery.Core/DocQuery.Core.csproj src/DocQuery.Core/
COPY src/DocQuery.Infrastructure/DocQuery.Infrastructure.csproj src/DocQuery.Infrastructure/
COPY src/DocQuery.Api/DocQuery.Api.csproj src/DocQuery.Api/
RUN dotnet restore src/DocQuery.Api/DocQuery.Api.csproj
COPY . .
RUN dotnet publish src/DocQuery.Api/DocQuery.Api.csproj -c Release -o /app

# Stage 2: runtime — small image, no SDK, no source
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "DocQuery.Api.dll"]