[![DruidsCornerApi Dotnet build and run Tests](https://github.com/bebenlebricolo/DruidsCornerApi/actions/workflows/dotnet.yml/badge.svg)](https://github.com/bebenlebricolo/DruidsCornerApi/actions/workflows/dotnet.yml)

# DruidsCornerApi
Druid's Corner API

# Build the api (Command Line Tools)
In order to build the api you'll need :
* dotnet CLI tools
* .Net 7.0+ installed
-> Installation depends on your OS distibution

Then run the commands :
```bash
  dotnet build DruidsCornerAPI
  dotnet build DruidsCornerAPI -c <Release/Debug>
  dotnet build DruidsCornerAPI/DruidsCornerAPI.sln -c <Release/Debug>  # Builds the whole solution
```

# Run the api locally (against localhost:<port number>)
For debugging purposes/development process, you might need to run the API locally :

```bash
  dotnet run --project DruidsCornerAPI/DruidsCornerAPI
```

# Run the tests

After building the whole solution (or the individual test project), we can run the tests now
```bash
  dotnet test DruidsCornerAPI/DruidsCornerUnitTests 
  dotnet test DruidsCornerAPI/DruidsCornerUnitTests -l:junit # Outputs TestResults.xml in the DruidsCornerUnitTests/TestResults/ folder
```

# Environment variables setup
Some environment variables are used to configure (in development process) how the API works.
Here is a list of the env vars and what they do :

|   env var name        |  example/values       |   description         |
|-----------------------|-----------------------|-----------------------|
| `DRUIDSCORNERAPI_DIR` |`"C:\\YourName\\DruidsCornerApi\TestDatabase"` | Tells the runtime where to find repo's base location. Used for testing purposes (with local database especially) |
| `DRUIDSCORNERAPI_DBMODE` | `Local` or `Cloud` | Tells the runtime where to look for a database. This overrides the Database default selection mode |

# Local secrets store
For now (dev) this Api relies on dotnet user secrets stores.
When cloning the project, the secret store is enabled for the **DruidsCornerAPI project** but no entries are saved.
So first, we need to set the secrets :
```bash
dotnet user-secrets set "Authentication:Google:ClientId" "<ClientID>" -p DruidsCornerAPI/DruidsCornerAPI
```
