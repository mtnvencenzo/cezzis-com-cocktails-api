# Cezzis.com Cocktails API

Part of the broader Cezzis.com digital experience for discovering and sharing cocktail recipes with a broad
community of cocktail enthusiasts and aficionados.

[![CI](https://github.com/mtnvencenzo/cezzis-com-cocktails-api/actions/workflows/cezzis-api-cicd.yaml/badge.svg?branch=main)](https://github.com/mtnvencenzo/cezzis-com-cocktails-api/actions/workflows/cezzis-api-cicd.yaml)
[![Release](https://img.shields.io/github/v/release/mtnvencenzo/cezzis-com-cocktails-api?include_prereleases)](https://github.com/mtnvencenzo/cezzis-com-cocktails-api/releases)
[![License](https://img.shields.io/badge/license-Proprietary-lightgrey)](LICENSE)
![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet&logoColor=white)
[![Last commit](https://img.shields.io/github/last-commit/mtnvencenzo/cezzis-com-cocktails-api?branch=main)](https://github.com/mtnvencenzo/cezzis-com-cocktails-api/commits/main)
[![Issues](https://img.shields.io/github/issues/mtnvencenzo/cezzis-com-cocktails-api)](https://github.com/mtnvencenzo/cezzis-com-cocktails-api/issues)
[![Docs](https://img.shields.io/badge/docs-OpenAPI%20Scalar-blue)](https://api.cezzis.com/prd/cocktails/api-docs/v1/scalar/v1)
[![Project](https://img.shields.io/badge/project-Cezzis.com%20Cocktails-181717?logo=github&logoColor=white)](https://github.com/users/mtnvencenzo/projects/2)
[![Website](https://img.shields.io/badge/website-cezzis.com-2ea44f?logo=google-chrome&logoColor=white)](https://www.cezzis.com)

## 🧩 Cezzis.com Project Ecosystem

This backend works alongside several sibling repositories:

- **cocktails-api** (this repo) – ASP.NET Core backend and REST API consumed by the site and integrations
- [cocktails-web](https://github.com/mtnvencenzo/cezzis-com-cocktails-web) – React SPA for the public experience
- [cocktails-mcp](https://github.com/mtnvencenzo/cezzis-com-cocktails-mcp) – Model Context Protocol services that expose cocktail data to AI agents
- [cocktails-common](https://github.com/mtnvencenzo/cezzis-com-cocktails-common) – Shared libraries and utilities reused across frontends, APIs, and tooling
- [cocktails-images](https://github.com/mtnvencenzo/cezzis-com-cocktails-images) (private) – Source of curated cocktail imagery and CDN assets
- [cocktails-shared-infra](https://github.com/mtnvencenzo/cezzis-com-cocktails-shared-infra) – Terraform compositions specific to the cocktails platform
- [shared-infrastructure](https://github.com/mtnvencenzo/shared-infrastructure) – Global Terraform modules that underpin multiple Cezzis.com workloads

![Complete Diagram](./assets/cezzis-com-api-interactions.drawio.svg)

## ☁️ Cloud-Native Footprint (Azure)

Infrastructure is provisioned with Terraform (`/terraform`) and deployed into Azure using shared modules:

- **Azure Container Apps** – Hosts the `cocktails-api` container with HTTPS ingress
- **Azure API Management** – Fronts the API; backend host key required for integration
- **Azure Key Vault** – Holds secrets and configuration (Auth0, APIM keys, connection strings)
- **Azure Cosmos DB** – Primary database via EF Core Cosmos provider
- **Azure AI Search** – Full‑text search over cocktails and ingredients
- **Azure Service Bus** – Pub/Sub via Dapr for event-driven scenarios
- **Azure Blob Storage** – Image/object storage
- **Azure Monitor / Application Insights** – Telemetry collection and dashboards

## 🛠️ Technology Stack

### Core Framework
- **Framework**: .NET 10.0 
- **API Style**: RESTful with OpenAPI (Scalar UI)  
- **Architecture**: Clean Architecture + CQRS (MediatR)  
- **Domain**: Domain-Driven Design (DDD) principles 
- **Implementation**: Minimal APIs with endpoint routing and API versioning (aspnet-api-versioning)

### Data Layer
- **Database**: Azure Cosmos DB (SQL API) via EF Core Cosmos provider
- **Search**: Azure AI Search
- **Storage**: Azure Blob Storage for images
- **Integration**: Dapr (pub/sub) with Azure Service Bus

### Authentication & Security
- **Authentication**: Auth0 (OAuth 2.0 / OIDC) with JWT Bearer
- **Authorization**: Scope-based policies (e.g., `read:owned-account`, `write:owned-account`)
- **Gateway**: Azure API Management (APIM) backend integration
- **Secrets**: Azure Key Vault

### Development Tools
- **IDE**: VS Code / Visual Studio
- **Package Manager**: NuGet
- **Testing**: xUnit, Moq
- **API Documentation**: OpenAPI + Scalar
- **CI/CD**: GitHub Workflows

## 🏗️ Project Structure
```text
src/
├── Cocktails.Api/                 # Main API project
├── Cocktails.Api.Domain/          # Domain layer
└── Cocktails.Api.Infrastructure/  # Infrastructure layer

test/
├── Cocktails.Api.Unit.Tests/
├── Cocktails.Api.Domain.Unit.Tests/
└── Cocktails.Api.Infrastructure.Unit.Tests/

terraform/
└── ...                            # Azure resources (APIM, ACA, AI Search, Key Vault, etc.)
```

## 🚀 Development Setup
 **Public documentation**: [https://api.cezzis.com/prd/cocktails/api-docs/v1/scalar/v1](https://api.cezzis.com/prd/cocktails/api-docs/v1/scalar/v1)
1) Prerequisites
  - .NET SDK 10.0
   - Optional: Dapr CLI (sidecar), Docker (container builds), Azure CLI / Terraform (infrastructure)

2) Install Dependencies
   - Run `dotnet restore` at the repo root

3) Environment Setup
  - See the Environment Setup Guide: [Environment Setup Guide](.readme/env-setup.md)
  - For Auth0 configuration, see: [Auth0 Setup](.readme/readme-auth0.md)
  - Storage emulator (Azurite): [Azurite guide](.readme/readme-azurite.md)
  - Cosmos DB emulator: [Cosmos DB guide](.readme/readme-cosmos.md)
   - Local Auth0 settings are read from `src/Cocktails.Api/appsettings.local.json`:
     ```json
     {
       "Auth0": {
         "Domain": "https://your-tenant.auth0.com",
         "Audience": "https://your-api-identifier",
         "ClientId": "your-client-id"
       },
       "Scalar": {
         "AuthorizationCodeFlow": {
           "ClientId": "your-frontend-client-id",
           "Scopes": [
             "read:owned-account",
             "write:owned-account",
             "admin:cezzi-cocktails"
           ]
         }
       }
     }
     ```

4) Local Development
   - Run the API:
     ```bash
     dotnet run --project src/Cocktails.Api/Cocktails.Api.csproj
     ```
   - Local API docs: [http://localhost:7179/scalar/v1](http://localhost:7179/scalar/v1)
   - Optional Dapr: task available in `.vscode/tasks.json` (sidecar ports and HTTPS configured)

5) Testing
   ```bash
   # Run all tests
   dotnet test

   # Run a specific test project
   dotnet test test/Cocktails.Api.Unit.Tests
   ```

6) Docker
  Create tls certificates for local docker setup
  ```bash
dotnet dev-certs https -ep ./.certs/aspnetapp.pfx -p password
dotnet dev-certs https --trust
  ```

  Build the image
  ```bash
  sudo docker image build \
    -f ./Dockerfile \
    -t cocktails-api:latest \
    --rm \
    --build-arg GH_PACKAGES_PAT_TOKEN_READ=$GH_PACKAGES_PAT_TOKEN_READ \
    .
  ```

  Run the image
  ```bash
  docker run -d \
    --name cocktails-api \
    -p 7177:80 \
    -p 7176:443 \
    -v "$PWD/.certs:/https:ro" \
    -e DAPR_HTTP_PORT=3500 \
    -e DAPR_GRPC_PORT=50001 \
    -e DOTNET_ENVIRONMENT=docker \
    -e ASPNETCORE_Kestrel__Certificates__Default__Path=/https/aspnetapp.pfx \
    cocktails-api:latest
  ```

## 📚 API Documentation

Public documentation: [https://api.cezzis.com/prd/cocktails/api-docs/v1/scalar/v1](https://api.cezzis.com/prd/cocktails/api-docs/v1/scalar/v1)

## 📦 Build & Deployment

- **Build**: `dotnet build` and `dotnet publish -c Release`
- **Container**: Dockerfile provided under `src/Cocktails.Api/`
- **Infra**: Terraform under `/terraform` for APIM, Cosmos, AI Search, Service Bus, Storage, etc.
- **CI/CD**: GitHub Workflows build, test, and publish artifacts/images

## 🔍 Code Quality

- .NET analyzers and unit tests via xUnit
- Consistent formatting via `.editorconfig`
- PR validation through GitHub Workflows

## 🔒 Security Features

- JWT Bearer authentication (Auth0)
- Scope-based authorization (e.g., `read:owned-account`, `write:owned-account`)
- HTTPS enforcement and CORS policy
- API versioning
- Input validation via FluentValidation
- APIM host key protection for backend integration

## 📈 Monitoring

- OpenTelemetry via `Cezzi.OTel` (traces/metrics/logs)
- Azure Monitor / Application Insights
- Health checks

## 🌐 Community & Support

- 🤝 Contributing Guide – see [CONTRIBUTING.md](.github/CONTRIBUTING.md)
- 🤗 Code of Conduct – see [CODE_OF_CONDUCT.md](.github/CODE_OF_CONDUCT.md)
- 🆘 Support Guide – see [SUPPORT.md](.github/SUPPORT.md)
- 🔒 Security Policy – see [SECURITY.md](.github/SECURITY.md)

## 📄 License

This project is proprietary software. All rights reserved. See [LICENSE](LICENSE) for details.