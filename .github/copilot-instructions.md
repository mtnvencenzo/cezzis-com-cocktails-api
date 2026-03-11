# Copilot Instructions

## Build, Test & Run

```bash
# Restore, build, test
dotnet restore
dotnet build
dotnet test

# Run a single test project
dotnet test test/Cocktails.Api.Unit.Tests
dotnet test test/Cocktails.Api.Domain.Unit.Tests
dotnet test test/Cocktails.Api.Infrastructure.Unit.Tests

# Run a single test by name filter
dotnet test --filter "FullyQualifiedName~MyTestClass.MyTestMethod"

# Run the API locally
dotnet run --project src/Cocktails.Api/Cocktails.Api.csproj
# Local Scalar UI: http://localhost:7179/scalar/v1

# Run with Dapr sidecar (VS Code task or manual)
dapr run --app-id cocktails-api-dapr \
  --app-port 7179 \
  --dapr-http-port 5295 \
  --dapr-grpc-port 50003 \
  --resources-path .iac/dapr/k8s-loc/ \
  --config .iac/dapr/k8s-loc/dapr-config.yaml \
  -- dotnet run --project src/Cocktails.Api/Cocktails.Api.csproj
```

`TreatWarningsAsErrors=true` is set globally in `Directory.Build.props` — all compiler warnings fail the build.

## Architecture

**Layer order:** `Cocktails.Api` → `Cocktails.Api.Domain` → `Cocktails.Api.Infrastructure`

- **`Cocktails.Api`** — Minimal API endpoints, MediatR pipeline behaviors, startup extensions, DI wiring
- **`Cocktails.Api.Domain`** — Aggregates (entities, value objects, repository interfaces), domain events, config models
- **`Cocktails.Api.Infrastructure`** — EF Core Cosmos DB context, repository implementations, Dapr event bus, data stores (seeding from embedded resources)

**Request flow:** HTTP request → Minimal API endpoint → MediatR `Send()` dispatches a Command or Query → Pipeline behaviors (validation, exception handling, auth) → Handler → Repository → Cosmos DB or Azure AI Search

**Domain events** are collected on entities (implementing `IEventEmitter<IEvent>`), dispatched inside `CocktailDbContext.SaveEntitiesAsync()` via MediatR before returning.

**Seeding** is triggered by a Dapr scheduled job (`initialize-app`) that sends `InitializeAppCommand` → `SeedCocktailsCommand` / `SeedIngredientsCommand`. Data is loaded from embedded `.md` resource files in `Cocktails.Api.Infrastructure/Resources/`.

## Key Conventions

### Minimal API Endpoints

Each feature area under `Apis/` has three files:
- `*Api.cs` — endpoint route definitions (`IEndpointRouteBuilder` extensions)
- `*Services.cs` — `[AsParameters]` service aggregator injected into handlers
- `*Validators.cs` — FluentValidation validators for request models

Endpoint groups use `RequireAuthorization(ApimHostKeyRequirement.PolicyName)` at the group level. Endpoints needing OAuth scopes add `.RequireAuthorization("scope:admin:cezzi-cocktails")`.

```csharp
// Endpoint pattern
app.MapGet("/cocktails/{id}", async (
    [AsParameters] CocktailsServices services,
    string id,
    CancellationToken ct) =>
{
    var result = await services.Mediator.Send(new GetCocktailQuery(id), ct);
    return TypedResults.Ok(result);
})
.RequireAuthorization(ApimHostKeyRequirement.PolicyName);
```

### MediatR Commands vs Queries

- **Commands** live in `Application/Concerns/{Feature}/Commands/` — named `{Action}{Feature}Command` + `{Action}{Feature}CommandHandler`
- **Queries** live in `Application/Concerns/{Feature}/Queries/` — interface `I{Feature}Queries` implemented by `{Feature}Queries`
- Queries are called directly from endpoints (not via MediatR `Send`); commands are dispatched via `Mediator.Send()`

### Authorization

Three authorization policies:
- `ApimHostKeyRequirement.PolicyName` — validates API Management backend host key header; applied to all main route groups
- `"scope:{scope}"` (dynamic, e.g. `"scope:admin:cezzi-cocktails"`) — OAuth scope from JWT; applied to individual protected endpoints
- `DaprAppTokenRequirement.PolicyName` — validates Dapr-to-app tokens; applied to Dapr job/event endpoints

Auth scopes are defined as constants on `AuthScopes` class. New scopes added there are automatically registered as policies via reflection in `AuthorizationExtensions`.

### Repository Pattern

Repositories implement both `IRepository<T>` and `IReadonlyRepository<T>`. `ICocktailRepository` exposes:
- `Items` — `IQueryable<Cocktail>` with `AsNoTracking()` direct Cosmos access
- `CachedItems` — thread-safe static in-memory cache (Lock pattern); used for read-heavy filter/sitemap queries

### FluentValidation Pipeline

`ValidatorBehavior<TRequest, TResponse>` runs as a MediatR pipeline behavior. All validators registered from `typeof(Program).Assembly` via `AddValidatorsFromAssembly`. Supports both sync and async validators — checked via `IsAsync()` interface detection.

### Testing Patterns

Tests inherit from `ServiceTestBase` (in `Cocktails.Api.Unit.Tests`):

```csharp
public abstract class ServiceTestBase : IAsyncLifetime
```

- Builds a `WebApplication` with `"unittest"` environment
- Provides `SetupEnvironment(Action<IServiceCollection>)` for per-test DI overrides
- Use `AutoBogus` (via `AutoFaker<T>`) for random model generation
- Use `FluentAssertions` (`.Should()`) for assertions — not plain `Assert.*`
- Use `Moq` for mocking; `Moq.EntityFrameworkCore` for `DbSet<T>` mocking

Custom HTTP mocks (`MockHttpContext`, `MockHttpRequest`, `MockHttpResponse`) are in `test/Cocktails.Api.Unit.Tests/Mocks/`.

### Cosmos DB Context

Development uses connection string (emulator, gateway mode, no cert validation). Production uses `DefaultAzureCredential` with direct mode TCP connection pooling. Configured in `CosmosExtensions.cs`.

### Dapr Integration

- Pub/sub publishing via `DaprEventBus` (implements `IEventBus`) using `DaprClient.PublishEventAsync`
- Component configs for local development: `.iac/dapr/k8s-loc/` (Kubernetes-style) and `.iac/dapr/docker-loc/` (Docker-style)
- Dapr jobs client (`DaprJobsClient`) schedules `initialize-app` with exponential retry (5 attempts, 20s base delay)

### Global Settings (`Directory.Build.props`)

- `LangVersion=latest`
- `TreatWarningsAsErrors=true`
- `Nullable=disable` — nullable reference types are **off** project-wide
- `ImplicitUsings=enable`
- `EnforceCodeStyleInBuild=True` — `.editorconfig` rules enforced at compile time

### NuGet Source

Private NuGet feed (GitHub Packages) configured in `Nuget.Config`. Requires `GH_PACKAGES_PAT_TOKEN_READ` environment variable set for Docker builds and CI. The `Cezzi.*` packages (`Cezzi.Applications`, `Cezzi.OTel`, `Cezzi.Security`, etc.) come from this private feed.
