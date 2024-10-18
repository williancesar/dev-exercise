# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the service project
COPY src/ProductService/ProductService.csproj src/
RUN dotnet restore src/ProductService.csproj

# Copy test projects (unit and integration tests)
COPY test/UnitTests/UnitTests.csproj test/UnitTests/
COPY test/IntegrationTests/IntegrationTests.csproj test/IntegrationTests/
RUN dotnet restore test/UnitTests/UnitTests.csproj
RUN dotnet restore test/IntegrationTests/IntegrationTests.csproj

# Copy all the source files for both the service and tests
COPY . .

# Build the service project
WORKDIR /src/src/ProductService
RUN dotnet build ProductService.csproj -c Release -o /app/build

# Build the test projects
WORKDIR /src/test/UnitTests
RUN dotnet build UnitTests.csproj -c Release

WORKDIR /src/test/IntegrationTests
RUN dotnet build IntegrationTests.csproj -c Release

# Stage 2: Publish the application
FROM build AS publish
WORKDIR /src/src/ProductService
RUN dotnet publish ProductService.csproj -c Release -o /app/publish

# Stage 3: Build the final runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProductService.dll"]