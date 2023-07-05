[![DruidsCornerApi build and test](https://github.com/bebenlebricolo/DruidsCornerApi/actions/workflows/dotnet.yml/badge.svg?branch=main)](https://github.com/bebenlebricolo/DruidsCornerApi/actions/workflows/dotnet.yml)

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

# Build and publishing the API remotely

## Requirements 
* Having Docker installed
* Gcloud cli tools for api publishing
  * Install gcloud tools from Google download areas (tried with linux distro managed packages but they prevent further gcloud plugin installation so better resort to the original installers in gcloud...)

## Docker build
```bash
cd DruidsCornerApi

# Both service account and sa key files are required to build this image
# -> This allows docker to download DiyDogExtracted database directly from GS buckets through service account authentication
export SERVICE_ACCOUNT="Myservice_account"
export SA_KEYFILE="$(cat somefile.json)"
docker build . -t druidscornerapi-build --build-arg SERVICE_ACCOUNT=$SERVICE_ACCOUNT --build-arg SA_KEYFILE=$SA_KEYFILE
```

## Publish docker image to container registry 
### Base configuration
```bash
  gcloud init
  gcloud config set projects <google cloud project id>
  gcloud auth login
  # Or via a service account (scenario when image is built and pushed from servers, like from Github or Azure Dev Ops)
  gcloud auth activate-service-account <ACCOUNT> --key-file=<KEY-FILE>
  gcloud auth configure-docker europe-docker.pkg.dev # docker container registry is hosted in europe for now, this can change.
```

***Note : for service account authentication, [google has made a page available for that](https://cloud.google.com/artifact-registry/docs/docker/authentication?hl=fr)***

### Publishing
```bash
  docker tag [IMAGE] gcr.io/[PROJECT-ID]/[IMAGE]
  docker push gcr.io/[PROJECT-ID]/[IMAGE]
  
```