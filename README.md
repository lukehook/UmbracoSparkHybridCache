# UmbracoSparkHybridCache

## Overview

UmbracoSparkHybridCache is a project that demonstrates the use of hybrid caching with distributed and in-memory caching for a Pokémon API. The project consists of three main components:

- **SparkFusionCache**: A web application that uses FusionCache for caching Pokémon data.
- **SparkHybridCache**: A web application that uses HybridCache for caching Pokémon data.
- **SparkShared**: A shared library containing common code used by both web applications.

> [!NOTE]  
> This demo utilises the free and open source [PokeAPI](https://pokeapi.co) - this is a RESTful service that responds with Pokemon data that is used for the purpose of demonstrating the HybridCache functionality. Please use this service responsibly.

## Project Structure

```
.gitattributes
.gitignore
UmbracoSparkHybridCache.sln
.github/
  workflows/
.vs/
  VSWorkspaceState.json
  ProjectEvaluation/
    umbracosparkhybridcache.metadata.v9.bin
    umbracosparkhybridcache.projects.v9.bin
    umbracosparkhybridcache.strings.v9.bin
UmbracoSparkHybridCache/
  config/
  copilot-chat/
  CopilotIndices/
  DesignTimeBuild/
  FileContentIndex/
  RestEditor/
  v17/
Properties/
  launchSettings.json
SparkFusionCache/
  appsettings.Development.json
  appsettings.json
  http-client.env.json
  Program.cs
  SparkFusionCache.csproj
  SparkFusionCache.csproj.user
  SparkFusionCache.http
  bin/
    Debug/
  obj/
    project.assets.json
    project.nuget.cache
  Properties/
    launchSettings.json
  wwwroot/
    css/
      styles.css
    fonts/
      Pokemon Hollow.ttf
      Pokemon Solid.ttf
    index.html
    js/
      script.js
SparkHybridCache/
  appsettings.Development.json
  appsettings.json
  http-client.env.json
  index.html
  Program.cs
  SparkHybridCache.csproj
  SparkHybridCache.csproj.user
  SparkHybridCache.http
  bin/
  obj/
  Properties/
    launchSettings.json
  wwwroot/
    css/
      styles.css
    fonts/
      Pokemon Hollow.ttf
      Pokemon Solid.ttf
    index.html
    js/
      script.js
SparkShared/
  PokemonResponse.cs
  PokemonService.cs
  SparkShared.csproj
  bin/
  obj/
```

## Getting Started

### Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)

> [!NOTE]  
> The projects in this repo are using .NET 9 and will require the .NET 9 runtime to function. However HybridCache and FusionCache are compatible with verions of .NET as far back as .NET Framework 4.7.2

- [Redis](https://redis.io/download)

> [!NOTE]  
> This example uses Redis as an IDistributedCache. You can provide your own connectionString to a Redis resource or swap out the IDistributedCache for a different implementation such as SQLServer

### Setup

1. Clone the repository:
    ```sh
    git clone https://github.com/yourusername/UmbracoSparkHybridCache.git
    cd UmbracoSparkHybridCache
    ```

2. Restore the dependencies:
    ```sh
    dotnet restore
    ```

3. Update the Redis connection string in `appsettings.json` for both `SparkFusionCache` and `SparkHybridCache` projects.

### Running the Applications

#### SparkFusionCache

1. Navigate to the `SparkFusionCache` directory:
    ```sh
    cd SparkFusionCache
    ```

2. Run the application:
    ```sh
    dotnet run
    ```

3. Open your browser and navigate to `http://localhost:7109`.

or

4. Open the ```SparkFusionCache.http``` files and simulate the requests

#### SparkHybridCache

1. Navigate to the `SparkHybridCache` directory:
    ```sh
    cd SparkHybridCache
    ```

2. Run the application:
    ```sh
    dotnet run
    ```

3. Open your browser and navigate to `http://localhost:7010`.

or

4. Open the ```SparkHybridCache.http``` files and simulate the requests

## Usage

- Enter a Pokémon name in the input field and click "Submit" to fetch Pokémon data.
- Use the "Types" button to filter Pokémon by type.
- Use the "Clear Tags" button to clear cached data by tag.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
