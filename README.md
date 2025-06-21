# Cocktails API

## 🛠️ Technology Stack

### Core Framework
- **Framework**: .NET Core 9.0
- **API Style**: RESTful with OpenAPI/Swagger documentation
- **Architecture**: Clean Architecture with CQRS pattern
- **Domain**: Domain-Driven Design (DDD) principles
- **API Implementation**: Minimal APIs with endpoint routing and versioning

### Data Layer
- **Database**: Azure Cosmos DB (SQL API)
- **State Management**: Dapr
- **Storage**: Azure Blob Storage for images

### Security & Authentication
- **Authentication**: Azure B2C
- **Authorization**: OAuth 2.0 with OpenID Connect
- **API Security**: Azure App Registrations
- **Secrets Management**: Azure Key Vault

### Infrastructure
- **Containerization**: Docker
- **Orchestration**: Azure Container Apps
- **API Gateway**: Azure API Management
- **CDN**: Azure Front Door
- **Service Mesh**: Dapr
- **Monitoring**: Application Insights

### Development Tools
- **IDE**: Visual Studio 2022
- **Package Manager**: NuGet
- **Testing**: xUnit, Moq
- **API Documentation**: Swagger/OpenAPI
- **CI/CD**: GitHub Workflows

## 🏗️ Project Structure

```
src/
├── Cocktails.Api/           # Main API project
├── Cocktails.Application/   # Application layer
├── Cocktails.Domain/        # Domain layer
└── Cocktails.Infrastructure/# Infrastructure layer

tests/
├── Cocktails.Api.Tests/     # API tests
└── Cocktails.Unit.Tests/    # Unit tests
README.md
```

## 🚀 Development Setup

1. **Prerequisites**
   - .NET Core SDK 9.0
   - Visual Studio 2022
   - Docker Desktop
   - Azure CLI
   - Dapr CLI

2. **Environment Setup**
   See [Environment Setup Guide](.readme/env-setup.md) for detailed instructions on configuring your development environment.

3. **Local Development**
   ```bash
   # Restore dependencies
   dotnet restore
   
   # Run the application
   dotnet run --project src/Cocktails.Api
   ```

4. **Testing**
   ```bash
   # Run all tests
   dotnet test
   
   # Run specific test project
   dotnet test tests/Cocktails.Unit.Tests
   ```

## 📚 API Documentation

Full API documentation is available at: [Scalar API Documentation](https://api.cezzis.com/prd/cocktails/api-docs/v1/scalar/v1)

## 🔒 Security Features

- Azure B2C authentication
- HTTPS enforcement
- API versioning
- Rate limiting
- Input validation
- SQL injection prevention
- XSS protection

## 📈 Monitoring

- Application Insights integration
- Health checks
- Performance monitoring
- Error tracking
- Custom metrics

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Test thoroughly
5. Submit a pull request

## 📄 License

This project is proprietary software. All rights reserved. 