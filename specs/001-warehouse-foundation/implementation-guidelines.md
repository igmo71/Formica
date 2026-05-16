# Implementation Guidelines: Warehouse Foundation

**Feature**: `001-warehouse-foundation`  
**Purpose**: Architecture guardrails for implementing `tasks.md` without repeating Phase 2 review mistakes.

This document refines the HOW-level implementation context for Warehouse Foundation. It is binding for implementation work in this feature branch and should be read together with `plan.md`, `data-model.md`, `contracts/api-contracts.md`, `quickstart.md`, and `tasks.md`.

## Internal Dependency Direction

Warehouse Foundation uses this internal dependency direction:

```text
Endpoints -> Features -> Domain
Endpoints -> Contracts
Features -> Persistence
Persistence -> Domain
UI ApiClients -> HTTP contracts only
```

Rules:

- `Domain` MUST NOT depend on `Features`.
- `Domain` MUST NOT depend on `Endpoints`.
- `Domain` MUST NOT depend on `Persistence` or EF Core configuration.
- `Domain` MUST NOT depend on `Contracts`, ASP.NET Core, HTTP result types, Blazor UI, or API clients.
- `Features` MAY depend on `Domain` and `Persistence` to implement use cases.
- `Persistence` MAY depend on `Domain` for EF Core mappings.
- `Endpoints` should remain thin and delegate behavior to feature/application code.

## Meaning of `Features`

`Features/` contains application use cases and vertical slices, such as:

- create/update/deactivate/reactivate warehouse;
- list/get warehouse;
- create/update/deactivate/reactivate zone;
- create/update/deactivate/reactivate storage location;
- create/update/deactivate/reactivate SKU;
- get warehouse layout;
- calculate setup readiness.

`Features/Common/` must not become a generic shared-kernel folder. Reusable domain concepts, lifecycle primitives, domain validation results, and address normalization rules belong under `Domain/`.

Accepted `Features/Common/` contents are limited to local Warehouse Foundation feature/application coordination types, such as the accepted feature result pattern described below.

## Domain Model Style

Warehouse Foundation uses self-contained rich domain models without excessive ceremony.

Domain models SHOULD:

- protect invariants through factories and methods;
- avoid public setters except where EF Core requires private setters/backing constructors;
- generate stable technical identities in domain factories using `Guid.CreateVersion7()`;
- use `DateTimeOffset.UtcNow` internally for simple lifecycle timestamps unless a future explicit `TimeProvider` decision is made;
- return simple domain errors/domain validation results from `Domain/Common/` when validation needs to be reported without exceptions;
- keep normalization that belongs to a domain concept inside that domain concept or its domain folder.

Domain models MUST NOT:

- access `DbContext`;
- check database uniqueness directly;
- return ASP.NET Core `IResult`, `ProblemDetails`, or API DTOs;
- depend on `Features`, `Endpoints`, `Persistence`, `Contracts`, or Blazor UI.

Uniqueness checks that require persisted data belong in feature/application code and should be backed by database constraints when the corresponding persisted entities exist.

## Domain Validation Primitives

Domain validation uses minimal domain-level primitives, not an application-wide result framework.

Expected primitives:

- `DomainValidationFailure` describes a domain validation error with a stable `Code`, human-readable `Message`, and optional `Field`.
- `DomainValidationResult` represents either valid state or one or more `DomainValidationFailure` values.

Placement:

```text
Formica.ApiService/Warehouse/WarehouseFoundation/Domain/Common/Validation/
  DomainValidationFailure.cs
  DomainValidationResult.cs
```

Rules:

- Domain validation primitives MUST NOT reference ASP.NET Core, HTTP, Blazor, EF Core, endpoint contracts, or `Features`.
- Domain validation primitives MAY be used by rich domain models to report expected domain input/invariant errors without throwing exceptions.
- Expected user-input/domain validation failures MUST be returned through `DomainValidationResult`, not thrown as `ArgumentException`, `InvalidOperationException`, or HTTP/API errors.
- Exceptions are reserved for programming errors or impossible states.
- Domain factories that can receive untrusted input SHOULD expose a validation-returning creation method such as `TryCreate(...)` returning `DomainValidationResult` and an `out` domain object.
- Endpoint/application code MAY translate `DomainValidationResult` to HTTP validation responses.
- Endpoint helpers SHOULD use standard ASP.NET Core validation responses, such as `TypedResults.ValidationProblem(...)`, when translating domain validation failures to HTTP.
- Do not introduce a generic application-wide `Result<T>`, CQRS result framework, or third-party validation abstraction in Warehouse Foundation unless explicitly approved later.

Expected shape:

```csharp
public sealed record DomainValidationFailure(
    string Code,
    string Message,
    string? Field = null);

public sealed record DomainValidationResult(
    IReadOnlyList<DomainValidationFailure> Errors)
{
    public bool IsValid => Errors.Count == 0;

    public static DomainValidationResult Valid { get; } = new([]);

    public static DomainValidationResult Invalid(params DomainValidationFailure[] errors)
        => errors.Length == 0 ? Valid : new(errors);
}
```

Use `DomainValidationResult` for expected domain validation failures, such as invalid address format, negative capacity, invalid code, or too-long text. Use exceptions only for programming errors or impossible states, not as the default user-input validation mechanism.

## Domain Create/Update Validation Pattern

Domain models that accept untrusted user input SHOULD expose validation-returning methods for both create and update operations.

Preferred pattern:

- `TryCreate(...)` for creation;
- `TryUpdate(...)` for mutation of editable attributes.

`TryCreate(...)` prevents invalid object creation.

`TryUpdate(...)` validates local invariants before mutating state and MUST NOT mutate state when validation fails.

Domain models handle local invariants and normalization, including required values, maximum lengths, controlled value validation, and lifecycle mechanics.

Feature/application code handles persistence-dependent checks, including not found, database uniqueness, relationship existence, conflict detection, transaction boundaries, and HTTP/API translation.

Do not split expected local domain validation into feature/application code merely to avoid calling a domain method. The feature/application layer may call domain methods such as `TryCreate(...)` and `TryUpdate(...)`, then perform persistence-dependent checks.

## Feature/Application Result Pattern

Warehouse Foundation uses a small local feature/application result pattern for use-case results:

- `FeatureResult<T>`;
- `FeatureResultStatus`;
- `FeatureError`.

Placement:

```text
Formica.ApiService/Warehouse/WarehouseFoundation/Features/Common/
  FeatureResult.cs
  FeatureResultStatus.cs
  FeatureError.cs
```

This is not a generic application-wide Result framework, not a CQRS framework, not a dispatcher abstraction, and not a MediatR replacement. It is a local Warehouse Foundation feature result contract used by feature handlers and endpoint mapping.

Feature handlers SHOULD return `FeatureResult<T>` when they need to report:

- success with a value;
- validation failure;
- not found;
- conflict.

Feature handlers MUST NOT introduce duplicated per-entity result/status types such as:

- `WarehouseFeatureResult`;
- `WarehouseFeatureStatus`;
- `ZoneFeatureResult`;
- `ZoneFeatureStatus`;
- similar entity-specific result/status duplicates.

`FeatureError` represents feature/application errors with a stable `Code`, human-readable `Message`, and optional `Field`.

Domain validation failures MAY be converted into `FeatureError` at the feature/application layer. Domain must keep using `DomainValidationResult` and `DomainValidationFailure`; it must not depend on feature result types.

Endpoints translate `FeatureResult<T>` into HTTP/TypedResults. Feature handlers must not return ASP.NET Core result types directly.

Expected feature result statuses:

```text
Success
ValidationFailed
NotFound
Conflict
```

## Identity Generation and EF Core

Persisted Warehouse Foundation entities should use application/domain-generated GUID v7 identities.

Expected pattern:

- domain factory/constructor assigns `Guid.CreateVersion7()`;
- EF Core configuration uses `ValueGeneratedNever()` for domain-generated identities;
- foreign keys must not be used as primary keys unless the model explicitly justifies that choice;
- placeholder foreign keys to future entities must not be introduced before the referenced entities exist.

## Location Address Rules

For this milestone, Location Address Rules are the default Warehouse Foundation address policy for the current Formica installation. They are not a child entity of Warehouse in Phase 2.

Rules:

- Do not model `WarehouseId` as the primary key of `LocationAddressRules`.
- Do not create a `LocationAddressRules -> Warehouse` foreign key before `Warehouse` exists.
- Use an explicit stable identity, such as `Id`, generated by the domain model.
- If a singleton/default rule set is persisted later, use an explicit code such as `DEFAULT` and a unique constraint on that code.
- Full UI/API management of Location Address Rules remains deferred unless a later task explicitly introduces it.

Storage location addresses remain unique within a warehouse after normalization. The rules for normalization/validation may be shared by all warehouses in this milestone unless a future feature introduces warehouse-specific overrides.

## Storage Location Capacity

`StorageLocationCapacity.Volume` represents optional configured usable volume, not necessarily `Height * Width * Depth`.

If height, width, and depth are supplied, implementation may expose a calculated geometric volume separately. Warehouse Foundation must not calculate capacity consumption, slotting suitability, or location recommendations.

## Controlled Purpose Values

`ZonePurpose` and `StorageLocationPurpose` should use the same numeric ordering where values overlap. Prefer an ordering based on a typical warehouse flow:

```text
Receiving -> QualityControl -> Quarantine -> Storage -> Picking -> Packing -> Staging -> Shipping -> Other
```

Use a reserved value such as `Other = 99` if useful to leave room for future additions.

## Migration Policy

Do not create EF Core migration files automatically during Phase 2.

Phase 2 may prepare:

- `WarehouseDbContext`;
- configuration folders;
- service registration;
- design-time infrastructure if needed;
- test fixtures.

Actual migration creation is allowed only after an explicit instruction and only when there is a coherent persisted entity model. Do not create migrations for incomplete placeholder entities or future foreign keys.

## Aspire Connection Strings

Runtime connection strings should come from Aspire resource references where possible. `appsettings.json` does not need to contain local development connection strings when Aspire supplies them through configuration/environment.

Design-time EF Core infrastructure must not hardcode a local PostgreSQL username/password as the only path. Prefer environment/configuration-based lookup and fail clearly when design-time connection information is not configured.

## Warehouse Foundation UI Workspace Pattern

Warehouse Foundation UI should be organized as a setup workspace around the selected warehouse, not as a growing set of permanently visible CRUD forms.

Child resources such as Zones and Storage Locations should be managed in contextual sections or tabs under the selected warehouse context.

For the current Bootstrap milestone, create/edit forms should not remain permanently visible when they are not being used. Prefer hidden or collapsible editors for create/edit workflows.

The UI structure should remain compatible with a future move to dialog, drawer, tabs, and table/data-grid components if a separate UI foundation decision introduces a component library.

The current milestone remains Bootstrap-only. Do not introduce MudBlazor or another UI component library without a separate explicit UI foundation decision.

## Agent Validation Command Policy

Implementation agents may run:

```text
dotnet build .\Formica.slnx -m:1
```

Implementation agents SHOULD NOT run long full test suites by default. Full test runs are performed manually by the user unless a prompt explicitly allows the agent to run tests.

If an implementation prompt explicitly allows tests, the prompt must define the exact command and expected scope.

## Review Branch Workflow

Experimental implementation attempts should use review branches, for example:

```text
review/001-warehouse-foundation-Phase2-v2
review/001-warehouse-foundation-Phase2-2026-05-13_14-45
```

Only reviewed and accepted phase work should be merged back into `001-warehouse-foundation`.
