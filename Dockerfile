FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /source

COPY *.sln .
COPY src/Application/*.csproj ./src/Application/
COPY src/Domain/*.csproj ./src/Domain/
COPY src/Infrastructure/*.csproj ./src/Infrastructure/
COPY src/API/*.csproj ./src/API/
COPY tests/UnitTest/*.csproj ./tests/UnitTest/
COPY tests/IntegrationTest/*.csproj ./tests/IntegrationTest/
RUN dotnet restore

COPY src/Application/. ./src/Application/
COPY src/Domain/. ./src/Domain/
COPY src/Infrastructure/. ./src/Infrastructure/
COPY src/API/. ./src/API/
COPY tests/UnitTest/. ./tests/UnitTest/
COPY tests/IntegrationTest/. ./tests/IntegrationTest/
WORKDIR /source/src/API
RUN dotnet publish -c release -o /published --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:6.0
ENV TZ="Asia/Ho_Chi_Minh"
WORKDIR /app
COPY --from=build /published ./
COPY --from=build /source/src/Infrastructure/Data/SeedData ./SeedData
ENTRYPOINT ["dotnet", "API.dll"]