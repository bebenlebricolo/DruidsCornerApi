#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS base

# Expose ports
# EXPOSE 80
# EXPOSE 443

ARG SA_KEYFILE
ARG BUCKET_DBPATH


ENV ROOT_DIR="DruidsCornerAPI"

# Required in order to get Python3.11 on debian 11 (bullseye)
#RUN echo "deb https://ftp.debian.org/debian/ bookworm contrib main" >> /etc/apt/sources.list

RUN apt-get update -y
RUN apt-get install -y  \
    apt-transport-https \
    ca-certificates     \
    gnupg               \
    curl                \
    python3-dev         \
    python3-setuptools  \
    python3-pip         \
    unzip               \
    vim

# Cannot use the debian-packaged google cloud sdk tools because it comes with a bundle python
# Which makes it very hard to modify. Gcloud sdk version : (437.0.1-0)
# the crcmod lib embedded in this package does not work great with the bundled python installation (for some reason ...)
# And because python installation is embedded with the package, we cannot remove the crcmod with regular python tools (pip uninstall crcmod).
#root@d65e3c9aa695:/tmp# find /usr/lib/google-cloud-sdk/platform/bundledpythonunix/ -name crcmod* -type f
#/usr/lib/google-cloud-sdk/platform/bundledpythonunix/lib/python3.9/site-packages/crcmod/__pycache__/crcmod.cpython-39.pyc
#/usr/lib/google-cloud-sdk/platform/bundledpythonunix/lib/python3.9/site-packages/crcmod/crcmod.py


# From https://cloud.google.com/sdk/docs/install?hl=fr#deb
#RUN echo "deb [signed-by=/usr/share/keyrings/cloud.google.gpg] http://packages.cloud.google.com/apt cloud-sdk main" | tee -a /etc/apt/sources.list.d/google-cloud-sdk.list \
#    && curl https://packages.cloud.google.com/apt/doc/apt-key.gpg | apt-key --keyring /usr/share/keyrings/cloud.google.gpg  add - \
#    && apt-get update -y && apt-get install google-cloud-cli -y

RUN cd /tmp && curl -O https://dl.google.com/dl/cloudsdk/channels/rapid/downloads/google-cloud-cli-437.0.1-linux-x86_64.tar.gz \
    && tar xf google-cloud-cli-437.0.1-linux-x86_64.tar.gz -C /opt \
    && /opt/google-cloud-sdk/install.sh -q
RUN ln -s /opt/google-cloud-sdk/bin/gcloud /usr/bin/gcloud      \
    && ln -s /opt/google-cloud-sdk/bin/gsutil /usr/bin/gsutil

# Removing crcmod manually !!!
RUN /opt/google-cloud-sdk/platform/bundledpythonunix/bin/pip3.9 uninstall --quiet --yes crcmod
#RUN /usr/lib/google-cloud-sdk/platform/bundledpythonunix/bin/pip3.9 uninstall crcmod

RUN rm /tmp/google-cloud-cli-437.0.1-linux-x86_64.tar.gz

RUN printf '%s' "${SA_KEYFILE}" > /tmp/sa_keyfile.json
RUN gcloud auth activate-service-account --key-file=/tmp/sa_keyfile.json

FROM base as build

# Take the whole sources, we also want to embed the tests in order to run them from within the container (as part of CI/CD worfklow)
COPY  . ${ROOT_DIR}
WORKDIR /${ROOT_DIR}

# Build the whole thing in Release mode
RUN dotnet build DruidsCornerAPI.sln -c Release
RUN dotnet publish /${ROOT_DIR}/DruidsCornerAPI/DruidsCornerAPI.csproj -c Release -o /${ROOT_DIR}/app/publish /p:UseAppHost=false

FROM build as dbprep

# Download the database using gsutil now
RUN gsutil cp gs://druidscornercloud-bucket/DiyDogExracted/diydog-db.zip /tmp/diydog-db.zip
RUN mkdir -p /${ROOT_DIR}/app/publish/diydog-db
RUN unzip /tmp/diydog-db.zip -d /${ROOT_DIR}/app/publish/diydog-db

#WORKDIR /app
#COPY --from=publish /app/publish .
#ENTRYPOINT ["dotnet", "DruidsCornerAPI.dll"]