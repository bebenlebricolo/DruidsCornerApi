[![DruidsCornerApi build and test](https://github.com/bebenlebricolo/DruidsCornerApi/actions/workflows/dotnet.yml/badge.svg?branch=main)](https://github.com/bebenlebricolo/DruidsCornerApi/actions/workflows/dotnet.yml) [![Docker build and Push](https://github.com/bebenlebricolo/DruidsCornerApi/actions/workflows/docker_image.yml/badge.svg)](https://github.com/bebenlebricolo/DruidsCornerApi/actions/workflows/docker_image.yml)

# Index

- [Index](#index)
- [DruidsCornerApi](#druidscornerapi)
- [Build the api (Command Line Tools)](#build-the-api-command-line-tools)
- [Run the api locally (against localhost:)](#run-the-api-locally-against-localhost)
- [Run the tests](#run-the-tests)
- [Environment variables setup](#environment-variables-setup)
- [Local Database downloading](#local-database-downloading)
- [General project architecture](#general-project-architecture)
  - [DiyDog database generation](#diydog-database-generation)
  - [WebApi deployment](#webapi-deployment)
  - [Client device/service](#client-deviceservice)
- [Manual test and run workflow](#manual-test-and-run-workflow)
  - [Use the gcloudauth python tool](#use-the-gcloudauth-python-tool)
  - [Use the generated token (valid for 1 hour) to connect to the WebApi](#use-the-generated-token-valid-for-1-hour-to-connect-to-the-webapi)
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
| `DRUIDSCORNERAPI_DIR` |`"C:\\YourName\\DruidsCornerApi\diydog-db"` | Tells the runtime where to find repo's base location. Used for testing purposes (with local database especially) |
| `DRUIDSCORNERAPI_DBMODE` | `Local` or `Cloud` | Tells the runtime where to look for a database. This overrides the Database default selection mode |

# Local Database downloading
In order to be able to use the **Local** database mode, you first need to download the database locally.
This can be achieved using **gsutil** :
```bash
  gsutil cp gs://<path to db> <Temp folder>
  unzip <Temp folder>/diydog-db.zip -d ${worskpaceFolder}/diydog-db
  mv ${worskpaceFolder}/diydog-db/deployed/* ${worskpaceFolder}/diydog-db/*
  rm -r ${worskpaceFolder}/diydog-db/deployed
```

# General project architecture
This project (Druids Corner Cloud) has many moving parts and requires security at various levels for everything to work properly.

## DiyDog database generation
For instance, the database is extracted by the repository [DiyDogExtractor](https://github.com/bebenlebricolo/DiyDogExtractor) and published with CI/CD pipelines to **Google Storage Buckets** (requires authentication).

## WebApi deployment
This .Net api (DruidsCornerApi) is also configured to be built and pushed automatically with CI/CD pipelines.
Base docker image automatically downloads the latest **diydog-db** from Google Storage Buckets and is published in Google Artifact Registry.
The Deployment image is also automatically built and deployed.
Then the Cloud Run service is based on top of the deployed image; and the WebApi runs in Cloud Run infrastructure (requires authentication)

## Client device/service
Yet to be developped : End user application (mobile app), and Websites

# Manual test and run workflow
First, generate a token using the OAuth2 Client.
## Use the gcloudauth python tool
A python tool is provided under [Tools/GoogleCloudTokenGen](Tools/GoogleCloudTokenGen/gcloudauth.py).
This tool requires you to provide valid configuration files under [Tools/GoogleCloudTokenGen/Config](Tools/GoogleCloudTokenGen/Config) where files such as `client_secrets.json` needs to be provided.
Their content must reflect an OAuth2 client, registered in the Google Cloud Project (download credentials from there, files are ignored by git and are not embedded in the repository.)

Then run the tool like this : 
```bash
  # Optional : Virtual environment and dependencies installation (to be done only once)
  python -m venv .venv
  source .venv/bin/activate
  cd Tools/GoogleCloudTokenGen
  pip install -r requirements.txt

  # Run the script
  python gcloudauth.py
  # -> Will open the browser and ask you to allow the client to connect to your google account
  # Prints a jwt token to stdout
```

## Use the generated token (valid for 1 hour) to connect to the WebApi
The token can now be used for every call that goes to the hosted WebApi (Google's Servers) and locally as well.
There are 2 security layers which consist of a first authentication/authorization check from Google infrastructure, then the WebApi performs another one on top of it.

Simply add the token to any `HTTPS` request headers :
```bash
  curl -H "Authorization: Bearer <token>" https://<hostname>/<controller>/<endpoint>
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

export CLIENT_ID="<The web client id used>"
# Then build the deployment image
docker build -f Dockerfile.deploy . -t druidscornerapi-deploy --build-arg CLIENT_ID=$CLIENT_ID
# Or build and stop before image stripping down so that the dev environment is still there
docker build -f Dockerfile.deploy --target build . -t druidscornerapi-deploy --build-arg CLIENT_ID=$CLIENT_ID

# Optional (same as above) : push to Google Artifact Registry
docker tag druidscornerapi-deploy europe-docker.pkg.dev/druids-corner-cloud/druidscornercloud-registry/druidscornerapi-deploy
docker push europe-docker.pkg.dev/druids-corner-cloud/druidscornercloud-registry/druidscornerapi-deploy
```