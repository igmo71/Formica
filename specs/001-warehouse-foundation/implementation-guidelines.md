# Implementation Guidelines: Warehouse Foundation

**Feature**: `001-warehouse-foundation`
**Purpose**: Binding implementation guardrails for `tasks.md`.

Use this document as the authoritative source for dependency direction, validation/result patterns, persistence rules, UI rules, and agent validation policy.

## Internal Dependency Direction

```text
Endpoints -> Features -> Domain
Endpoints -> Contracts
Features -> Persistence
Persistence -> Domain
UI ApiClients -> HTTP contracts only
```

Rules:

- `Domain` must not depend on Features, Endpoints, Persistence, Contracts, ASP.NET Core, HTTP result types, Blazor/MudBlazor UI, or API clients.
- `Features` may depend on Domain and Persistence to implement use cases.
- `Persistence` may depend on Domain for EF Core mappings.
- `Endpoints` stay thin and delegate behavior to feature/application code.
- UI components and `ApiClients` must not contain business rules.

## Feature and Domain Style

`Features/` contains vertical-slice use cases such as create/update/deactivate/reactivate operations, list/get queries, layout queries, and setup readiness calculations.

`Features/Common/` is limited to local feature/application coordination types. Reusable lifecycle primitives, validation results, and address normalization belong under `Domain/`.

Domain models should protect invariants through factories/methods, generate stable GUID v7 identities, keep domain normalization inside domain concepts, and expose validation-returning create/update methods for untrusted input.

Expected user-input/domain validation failures are returned through domain validation results, not exceptions. Exceptions are reserved for programming errors or impossible states.

## Domain Validation Pattern

Warehouse Foundation uses minimal domain-level validation primitives:

- `DomainValidationFailure`;
- `DomainValidationResult`.

Placement:

```text
Formica.ApiService/Warehouse/WarehouseFoundation/Domain/Common/Validation/
```

Domain validation primitives must remain framework-free. Endpoint/application code may translate them to standard ASP.NET Core validation responses.

## Feature Result Pattern

Feature handlers use the local Warehouse Foundation result pattern:

- `FeatureResult<T>`;
- `FeatureResultStatus`;
- `FeatureError`.

Placement:

```text
Formica.ApiService/Warehouse/WarehouseFoundation/Features/Common/
```

Use this pattern for success, validation failure, not found, and conflict. Do not introduce entity-specific duplicate result/status types, a global dispatcher, MediatR, or a generic application-wide result framework.

## Identity and Persistence

Persisted Warehouse Foundation entities use application/domain-generated GUID v7 identities. EF Core mappings use `ValueGeneratedNever()` for those identities.

Uniqueness checks that require persisted data belong in feature/application code and should be backed by database constraints when the corresponding persisted entities exist.

Foreign keys must not be used as primary keys unless explicitly justified by the model. Do not introduce placeholder foreign keys to future entities.

## Location Address Rules

Location Address Rules are the default Warehouse Foundation address policy for the current installation.

Rules:

- Do not model `WarehouseId` as the primary key of `LocationAddressRules`.
- Do not create a `LocationAddressRules -> Warehouse` foreign key before Warehouse-specific overrides are explicitly introduced.
- Use an explicit stable identity for persisted rule sets.
- Full UI/API management of Location Address Rules remains deferred unless a later task introduces it.

Storage location addresses remain unique within a warehouse after normalization.

## Capacity and Purpose Values

`StorageLocationCapacity.Volume` is optional configured usable volume, not necessarily `Height * Width * Depth`.

Warehouse Foundation must not calculate capacity consumption, slotting suitability, or location recommendations.

`ZonePurpose` and `StorageLocationPurpose` should keep compatible numeric ordering where values overlap. Use `Other = 99` if room for future values is useful.

## Migration Policy

Do not create EF Core migrations automatically during foundational setup. Migrations are allowed only after explicit instruction and only for coherent persisted models.

Design-time EF Core infrastructure must use environment/configuration-based connection lookup and fail clearly when connection information is unavailable. Do not hardcode local PostgreSQL credentials as the only path.

## MudBlazor UI Workspace

Warehouse Foundation UI belongs in `Formica.WebApp`.

Use MudBlazor 9.4.0 for administration UI patterns such as tables, forms, validation presentation, status indicators, dialogs/drawers, tabs, and workspace navigation.

`Formica.Web` is a temporary Bootstrap baseline/migration source only. Do not add new Warehouse Foundation UI functionality there.

Organize the UI as a setup workspace around a selected warehouse. Child resources such as Zones and Storage Locations should appear in contextual sections or tabs. Create/edit flows should use contextual editors, dialogs, drawers, tabs, or expansion panels rather than permanent forms.

Do not introduce another UI component library without a separate explicit UI foundation decision.

## Agent Validation Command Policy

Implementation agents may run:

```text
dotnet build .\Formica.slnx -m:1
```

Implementation agents should not run long full test suites by default. Full test runs are performed manually unless a prompt explicitly allows tests and defines the exact command/scope.

## Review Branch Workflow

Experimental implementation attempts should use review branches, for example:

```text
review/001-warehouse-foundation-Phase2-v2
review/001-warehouse-foundation-Phase2-2026-05-13_14-45
```

Only reviewed and accepted phase work should be merged back into `001-warehouse-foundation`.
