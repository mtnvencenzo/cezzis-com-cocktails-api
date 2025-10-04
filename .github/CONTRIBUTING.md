# 🍸 Contributing to Cezzis Cocktails API

Thank you for your interest in contributing to the Cezzis Cocktails API project! We welcome contributions that help improve our backend API, infrastructure, and developer experience.

## 📋 Table of Contents

- [Getting Started](#-getting-started)
- [Development Setup](#-development-setup)
- [Contributing Process](#-contributing-process)
- [Code Standards](#-code-standards)
- [Testing](#-testing)
- [Deployment](#-deployment)
- [Getting Help](#-getting-help)

## 🚀 Getting Started

### 🧰 Prerequisites

Before you begin, ensure you have the following installed:
- .NET SDK (see global.json or solution target, currently .NET 9)
- Dapr CLI (for local sidecar/components)
- Docker (optional, for containerized development)
- Terraform (optional, for IaC under `terraform/`)
- Git

### 🗂️ Project Structure

```text
├── src/
│   ├── Cocktails.Api/                 # ASP.NET Core minimal API
│   ├── Cocktails.Api.Domain/          # Domain model and services
│   └── Cocktails.Api.Infrastructure/  # Data access and integrations
├── test/                              # Unit tests
├── terraform/                         # Infrastructure as Code (Azure)
└── .github/                           # GitHub workflows and templates
```

## 💻 Development Setup

1. **Fork and Clone the Repository**
   ```bash
   git clone https://github.com/mtnvencenzo/cezzis-com-cocktails-api.git
   cd cezzis-com-cocktails-api
   ```

2. **Restore Dependencies**
   ```bash
   dotnet restore Cocktails.Api.sln
   ```

3. **Run locally**
   ```bash
   # Build & test
   dotnet build Cocktails.Api.sln
   dotnet test Cocktails.Api.sln

   # Run API
   dotnet run --project src/Cocktails.Api/Cocktails.Api.csproj

   # Optional: With Dapr sidecar
   # dapr run --app-id cocktails-api --resources-path ./.dapr --app-port 7176 --app-protocol https --dapr-http-port 5295
   ```

4. **Docker (Optional)**
   ```bash
   docker build -f src/Cocktails.Api/Dockerfile -t cocktails-api .
   docker run -p 7176:7176 cocktails-api
   ```

## 🔄 Contributing Process

### 1. 📝 Before You Start

- **Check for existing issues** to avoid duplicate work
- **Create or comment on an issue** to discuss your proposed changes
- **Wait for approval** from maintainers before starting work (required for this repository)

### 2. 🛠️ Making Changes

1. **Create a feature branch**
   ```bash
   git checkout -b feature/your-feature-name
   # or
   git checkout -b fix/your-bug-fix
   ```

2. **Make your changes** following our [code standards](#-code-standards)

3. **Test your changes**
   ```bash
   dotnet test Cocktails.Api.sln
   ```

4. **Commit your changes**
   ```bash
   git add .
   git commit -m "feat(api): add new endpoint for ..."
   ```
   
   Use [conventional commit format](https://www.conventionalcommits.org/):
   - `feat:` for new features
   - `fix:` for bug fixes
   - `docs:` for documentation changes
   - `style:` for formatting changes
   - `refactor:` for code refactoring
   - `test:` for adding tests
   - `chore:` for maintenance tasks

### 3. 📬 Submitting Changes

1. **Push your branch**
   ```bash
   git push origin feature/your-feature-name
   ```

2. **Create a Pull Request**
   - Use our [PR template](pull_request_template.md)
   - Fill out all sections completely
   - Link related issues using `Closes #123` or `Fixes #456`
   - Request review from maintainers

## 📏 Code Standards

### 🧩 API (.NET)

- C#/.NET conventions (nullable enabled)
- Async/await, no sync over async
- Minimal APIs conventions and API versioning
- DI-first, options binding, configuration via `IOptions<T>`
- AuthN/AuthZ via JWT Bearer and scope-based policies

### 🧪 Code Quality

```bash
# Build
dotnet build Cocktails.Api.sln

# Run tests
dotnet test Cocktails.Api.sln

# Code style (if enabled)
dotnet format
```

### 🌱 Infrastructure (Terraform)

- **Terraform**: Use Terraform best practices
- **Variables**: Define all variables in `variables.tf`
- **Documentation**: Document all resources and modules
- **State**: Never commit `.tfstate` files

## 🧪 Testing

### 🧪 Unit Tests
```bash
dotnet test Cocktails.Api.sln
```


### 📏 Test Requirements

- **Unit Tests**: All new features must include unit tests
- **E2E Tests**: Critical user flows should have E2E test coverage
- **Coverage**: Maintain minimum 80% code coverage
- **Test Naming**: Use descriptive test names that explain the behavior

## 🆘 Getting Help

### 📡 Communication Channels

- **Issues**: Use GitHub issues for bugs and feature requests
- **Discussions**: Use GitHub Discussions for questions and ideas
- **Email**: Contact maintainers directly for sensitive issues

### 📄 Issue Templates

Use our issue templates for:
- [Task Stories](./ISSUE_TEMPLATE/task-template.md)
- [User Stories](./ISSUE_TEMPLATE/user-story-template.md)

### ❓ Common Questions

**Q: How do I run the application locally?**
A: Follow the [Development Setup](#-development-setup) section above. Use `dotnet run --project src/Cocktails.Api/Cocktails.Api.csproj`.

**Q: How do I run tests?**
A: Use `dotnet test Cocktails.Api.sln`.

**Q: Can I contribute without approval?**
A: No, all contributors must be approved by maintainers before making changes.

**Q: How do I report a security vulnerability?**
A: Please email the maintainers directly rather than creating a public issue.

## 📜 License

By contributing to this project, you agree that your contributions will be licensed under the same license as the project (see [LICENSE](../LICENSE)).

---

**Happy Contributing! 🍸**

For any questions about this contributing guide, please open an issue or contact the maintainers.
