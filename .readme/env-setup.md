# Cocktails API Environment Setup

This guide is focused on local development for the Cocktails API and the minimum supporting services you may need (storage, database emulator, optional Dapr). Frontend, Cypress, or other projects are out of scope here.

## Table of Contents
- [Scope](#scope)
- [Prerequisites](#prerequisites)
- [Install and verify tooling](#install-and-verify-tooling)
- [Local services (optional)](#local-services-optional)
  - [Azurite (Blob Storage emulator)](#azurite-blob-storage-emulator)
  - [Cosmos DB Emulator (Linux container)](#cosmos-db-emulator-linux-container)
  - [Dapr (optional)](#dapr-optional)
- [Run the API locally](#run-the-api-locally)
- [Troubleshooting](#troubleshooting)
- [Related docs](#related-docs)

## Scope
Local developer setup for the Cocktails API only. Use this when you want to run and test the API on your machine, optionally with local emulators.

## Prerequisites
- Git
- .NET SDK 9.0
- Docker Engine (for emulators/Dapr)
- Azure CLI (optional, for cloud auth/tasks)
- Terraform CLI (optional, for infrastructure workflows)
- jq (optional, for JSON parsing in scripts)
- VS Code or your preferred editor

## Install and verify tooling

Below are Linux-friendly pointers. If you use a different distro, consult the official docs.

- .NET 9 SDK: see Microsoft docs for your distro
  - https://learn.microsoft.com/dotnet/core/install/linux
  - Verify: `dotnet --info`

- Docker Engine: https://docs.docker.com/engine/install/
  - Verify: `docker --version`

- Azure CLI: https://learn.microsoft.com/cli/azure/install-azure-cli
  - Verify: `az version`

- Terraform: https://developer.hashicorp.com/terraform/install
  - Verify: `terraform -version`

- jq: on Debian/Ubuntu: `sudo apt-get update && sudo apt-get install -y jq`
  - Verify: `jq --version`

- Git config (recommended for Linux):
  ```bash
  git config --global core.autocrlf input
  git config --global user.name "Your Name"
  git config --global user.email "you@example.com"
  ```

## Local services (optional)
You can run the API entirely against cloud services, but for local/offline work these emulators help.

### Azurite (Blob Storage emulator)
```bash
docker pull mcr.microsoft.com/azure-storage/azurite
docker run --restart=always -d \
  --name azurite-cocktails \
  -p 10000:10000 \
  -v "$(pwd)/.azurite:/data" \
  mcr.microsoft.com/azure-storage/azurite \
  azurite-blob --blobHost 0.0.0.0 --location /data
```
- See details and troubleshooting: [Azurite guide](./readme-azurite.md)

### Cosmos DB Emulator (Linux container)
```bash
docker pull mcr.microsoft.com/cosmosdb/linux/azure-cosmos-emulator:vnext-preview
docker run --restart=always -d \
  --name cosmos-cocktails \
  -p 8081:8081 -p 1234:1234 \
  mcr.microsoft.com/cosmosdb/linux/azure-cosmos-emulator:vnext-preview \
  --protocol https

docker ps
```
- See details and troubleshooting: [Cosmos Emulator guide](./readme-cosmos.md)

### Dapr (optional)
- Install Dapr CLI: https://docs.dapr.io/getting-started/install-dapr-cli/
- Initialize (requires Docker):
  ```bash
  dapr init
  dapr --version
  docker ps
  ```
- More info: [Dapr guide](./readme-dapr.md)

## Run the API locally
1) Restore and build
```bash
dotnet restore
dotnet build
```

2) Trust HTTPS dev certs (first time on a machine)
```bash
dotnet dev-certs https --trust
```

3) Configure local settings
- See [Auth0 Setup](./readme-auth0.md) for configuring `src/Cocktails.Api/appsettings.local.json`.
- If using emulators, ensure connection strings match the guides above.

4) Run the API
```bash
dotnet run --project src/Cocktails.Api/Cocktails.Api.csproj
```
- Local API docs: https://localhost:7176/scalar/v1

5) Run tests
```bash
dotnet test
```

## Troubleshooting
- Port already in use: stop conflicting processes or change exposed ports.
- HTTPS trust issues: re-run `dotnet dev-certs https --trust` and restart your browser.
- Docker cannot pull images: check network/proxy, then `docker login` if needed.
- Cosmos emulator cert/errors: follow the [Cosmos Emulator guide](./readme-cosmos.md) steps for certs.

## Related docs
- [Auth0 Setup](./readme-auth0.md)
- [Azurite guide](./readme-azurite.md)
- [Cosmos Emulator guide](./readme-cosmos.md)
- [Dapr guide](./readme-dapr.md)