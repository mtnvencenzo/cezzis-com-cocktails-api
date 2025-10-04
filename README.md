# Cocktails API

## 🛠️ Technology Stack

### Core Framework
- **Framework**: .NET 9.0
- **API Style**: RESTful with OpenAPI (Scalar UI)
- **Architecture**: Clean Architecture + CQRS (MediatR)
- **Domain**: Domain-Driven Design (DDD) principles
- **API Implementation**: Minimal APIs with endpoint routing and API versioning (aspnet-api-versioning)

### Data Layer
- **Database**: Azure Cosmos DB (SQL API) via EF Core Cosmos provider
- **Search**: Azure AI Search
- **Storage**: Azure Blob Storage for images
- **Integration**: Dapr (pub/sub) for messaging via Azure Service Bus

### Security & Authentication
- **Authentication**: Auth0 (OAuth 2.0 / OpenID Connect) with JWT Bearer
- **Authorization**: Scope-based policies (e.g., `read:owned-account`, `write:owned-account`)
- **Gateway**: Azure API Management (APIM) backend integration
- **Secrets Management**: Azure Key Vault

### Infrastructure
- **Containerization**: Docker
- **Orchestration**: Azure Container Apps
- **API Gateway**: Azure API Management
- **CDN**: Azure Front Door
- **Sidecar**: Dapr
- **Observability**: OpenTelemetry + Application Insights

### Development Tools
- **IDE**: VS Code / Visual Studio
- **Package Manager**: NuGet
- **Testing**: xUnit, Moq
- **API Documentation**: OpenAPI + Scalar
- **CI/CD**: GitHub Workflows

## 🏗️ Project Structure
```
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

1. **Prerequisites**
  - .NET SDK 9.0
  - VS Code or Visual Studio (optional)
  - Dapr CLI (optional, for sidecar)
  - Docker (optional, for container builds)
  - Azure CLI / Terraform (optional, for infra)

2. **Environment Setup**
   See [Environment Setup Guide](.readme/env-setup.md) for detailed instructions on configuring your development environment.
   
   For Auth0 configuration, see [Auth0 Setup Guide](.readme/readme-auth0.md).

3. **Auth0 Configuration**
   Configure your Auth0 settings in `src/Cocktails.Api/appsettings.local.json`:
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
           "write:owned-account"
         ]
       }
     }
   }
   ```

4. **Local Development**
   ```bash
   # Restore dependencies
   dotnet restore
   
   # Run the application
  dotnet run --project src/Cocktails.Api/Cocktails.Api.csproj
   ```

  - Local API docs: https://localhost:7176/scalar/v1
  - Optional Dapr (task available in .vscode/tasks.json): `dapr run --app-id cocktails-api --resources-path ./.dapr --app-port 7176 --app-protocol https --dapr-http-port 5295`

5. **Testing**
   ```bash
   # Run all tests
   dotnet test
   
   # Run specific test project
  dotnet test test/Cocktails.Api.Unit.Tests
   ```

## 📚 API Documentation

Full API documentation is available at: [Scalar API Documentation](https://api.cezzis.com/prd/cocktails/api-docs/v1/scalar/v1)

## 🔒 Security Features

- JWT Bearer authentication (Auth0)
- Scope-based authorization (e.g., `read:owned-account`, `write:owned-account`)
- HTTPS enforcement
- CORS policy
- API versioning
- Input validation via FluentValidation
- APIM host key protection for backend integration

## 📈 Observability

- OpenTelemetry via `Cezzi.OTel` (traces/metrics/logs)
- Azure Monitor / Application Insights
- Health checks

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Test thoroughly
5. Submit a pull request

## 📄 License

This project is proprietary software. All rights reserved. 