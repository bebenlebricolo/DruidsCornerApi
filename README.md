[![DruidsCornerApi build and test](https://github.com/bebenlebricolo/DruidsCornerApi/actions/workflows/dotnet.yml/badge.svg?branch=main)](https://github.com/bebenlebricolo/DruidsCornerApi/actions/workflows/dotnet.yml) [![Docker build and Push](https://github.com/bebenlebricolo/DruidsCornerApi/actions/workflows/docker_image.yml/badge.svg)](https://github.com/bebenlebricolo/DruidsCornerApi/actions/workflows/docker_image.yml)

# Index

- [Index](#index)
- [DruidsCornerApi](#druidscornerapi)
- [Build the api (Command Line Tools)](#build-the-api-command-line-tools)
- [Run the api locally (against localhost:)](#run-the-api-locally-against-localhost)
- [Run the tests](#run-the-tests)
- [Environment variables setup](#environment-variables-setup)
- [Local secrets store](#local-secrets-store)
- [Build and publishing the API remotely](#build-and-publishing-the-api-remotely)
  - [Requirements](#requirements)
  - [Docker build](#docker-build)
    - [Optional : authenticate to Google Cloud services to enable Artifact Registry access](#optional--authenticate-to-google-cloud-services-to-enable-artifact-registry-access)
    - [Full build steps](#full-build-steps)

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
### Optional : authenticate to Google Cloud services to enable Artifact Registry access

```bash
  gcloud init
  gcloud config set projects <google cloud project id>
  gcloud auth login
  # Or via a service account (scenario when image is built and pushed from servers, like from Github or Azure Dev Ops)
  gcloud auth activate-service-account <ACCOUNT> --key-file=<KEY-FILE>
  gcloud auth configure-docker europe-docker.pkg.dev # docker container registry is hosted in europe for now, this can change.
```

***Note : for service account authentication, [google has made a page available for that](https://cloud.google.com/artifact-registry/docs/docker/authentication?hl=fr)***

### Full build steps

```bash
cd DruidsCornerApi

# Both service account and sa key files are required to build this image
# -> This allows docker to download DiyDogExtracted database directly from GS buckets through service account authentication
export BUCKET_DBPATH="gs://bucket path on google"
export SA_KEYFILE="$(cat somefile.json)"
docker build -f Dockerfile.base . -t druidscornerapi-base --build-arg BUCKET_DBPATH=$BUCKET_DBPATH --build-arg SA_KEYFILE=$SA_KEYFILE

# Required in order to build the "deploy" image, or you'll need to login to Artifact registry with Gcloud first and configure Docker to pull from it
docker tag druidscornerapi-base europe-docker.pkg.dev/druids-corner-cloud/druidscornercloud-registry/druidscornerapi-base

# Optional (if docker is configured and authenticated)
docker push europe-docker.pkg.dev/druids-corner-cloud/druidscornercloud-registry/druidscornerapi-base

# Then build the deployment image
docker build -f Dockerfile.deploy . -t druidscornerapi-deploy
# Or build and stop before image stripping down so that the dev environment is still there
docker build -f Dockerfile.deploy --target build . -t druidscornerapi-deploy

# Optional (same as above) : push to Google Artifact Registry
docker tag druidscornerapi-deploy europe-docker.pkg.dev/druids-corner-cloud/druidscornercloud-registry/druidscornerapi-deploy
docker push europe-docker.pkg.dev/druids-corner-cloud/druidscornercloud-registry/druidscornerapi-deploy
```