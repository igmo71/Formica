# Research: Warehouse Foundation

**Feature**: `001-warehouse-foundation`  
**Spec**: `specs/001-warehouse-foundation/spec.md`  
**Plan**: `specs/001-warehouse-foundation/plan.md`  
**Date**: 2026-05-07

## Purpose

This document records Phase 0 technical decisions for Warehouse Foundation.

It resolves implementation-facing questions from `plan.md` before Phase 1 design artifacts are created. It does not define executable tasks or full implementation details.

## Decision Summary

| ID | Decision |
|----|----------|
| R-001 | Use PostgreSQL through EF Core/Npgsql as the initial relational database provider. |
| R-002 | Keep Warehouse persistence at `Formica.ApiService/Warehouse/Persistence` and keep Warehouse Foundation feature code at `Formica.ApiService/Warehouse/WarehouseFoundation`. |
| R-003 | Use Minimal API endpoint groups under `Formica.ApiService/Warehouse/WarehouseFoundation/Endpoints`. |
| R-004 | Use Blazor pages/components/API clients under `Formica.Web/Warehouse/WarehouseFoundation`. |
| R-005 | Use a pragmatic mixed testing approach in `Formica.Tests`. |
| R-006 | Model location address rules as simple configurable warehouse-level validation rules. |
| R-007 | Model capacity attributes as optional simple value-object-style data without capacity calculations. |

## R-001: PostgreSQL through EF Core/Npgsql

### Decision

Use PostgreSQL as the initial relational database provider for Warehouse Foundation, accessed through EF Core and the Npgsql provider.

Aspire should orchestrate PostgreSQL for local development and integration testing once persistence is introduced.

### Rationale

- PostgreSQL aligns well with Aspire and container-based development.
- It is a strong default for a modern .NET portfolio project.
- It avoids unnecessary SQL Server/Windows coupling.
- It has mature EF Core support through Npgsql.
- It supports relational constraints required by Warehouse Foundation.
- It leaves room for future operational data, outbox/inbox, JSONB, and analytical scenarios.

### Alternatives Considered

- **SQL Server**: strong .NET option and familiar in Microsoft-oriented environments, but more Windows/Microsoft-centric than needed for Formica's current direction.
- **SQLite**: useful for small tests or prototypes, but not selected as the primary provider because Warehouse Foundation relies on relational constraints and should stay close to production-like database behavior.

### Consequences

- Add Npgsql EF Core packages during implementation.
- Add Aspire PostgreSQL hosting integration during implementation.
- Integration tests should use PostgreSQL behavior where database behavior matters.
- Data model design should avoid provider-specific features unless justified by future needs.

## R-002: Backend Module and Persistence Placement

### Decision

For this milestone, use `Formica.ApiService` as the initial physical host for backend code, but do not treat `ApiService` as the owner of business logic.

Place Warehouse backend code under a logical Warehouse module boundary:

```text
Formica.ApiService/
└── Warehouse/
    ├── Persistence/
    │   ├── WarehouseDbContext.cs
    │   ├── Configurations/
    │   │   └── WarehouseFoundation/
    │   └── Migrations/
    │
    └── WarehouseFoundation/
        ├── Domain/
        ├── Features/
        └── Endpoints/
```

Do not create a separate `Formica.Modules.Warehouse` project yet.

### Meaning of `ApiService`

`Formica.ApiService` is the ASP.NET Core host, API surface, DI composition point, and initial physical container for backend code.

It must not become the conceptual owner of Warehouse business logic.

Warehouse business logic may physically live inside `Formica.ApiService` during the first milestone, but it must remain organized inside a logical Warehouse boundary so it can be extracted later into a dedicated Warehouse module project if needed.

### Meaning of Persistence

In this feature, `Persistence` means the Warehouse-scoped infrastructure responsible for durable storage and retrieval of Warehouse module data.

It may include:

- EF Core `DbContext`;
- EF Core entity configurations;
- migrations;
- database constraint configuration;
- PostgreSQL/Npgsql-specific setup needed by EF Core;
- persistence helpers needed to implement feature behavior.

It must not become a generic dumping ground for business rules, UI logic, endpoint definitions, or external integration code.

Persistence is infrastructure, but it remains scoped to the logical Warehouse module instead of becoming a global infrastructure layer.

### DbContext Granularity

A domain or module boundary may own persistence decisions, but it does not automatically require a dedicated `DbContext` for every feature or boundary.

For Warehouse Foundation, avoid placing a broad `WarehouseDbContext` inside the `WarehouseFoundation` folder because the name implies ownership of a wider Warehouse module than the milestone itself.

The initial persistence boundary should be Warehouse-level, not WarehouseFoundation-level:

```text
Formica.ApiService/Warehouse/Persistence/WarehouseDbContext.cs
```

Warehouse Foundation contributes the first set of entities and EF Core configurations to this Warehouse-level persistence boundary.

The Phase 1 data model must still confirm the final DbContext name and ownership before executable tasks are generated.

### Rationale

- The constitution favors modular monolith first and Clean Architecture without ceremony.
- `WarehouseFoundation` is a milestone/feature area, while `Warehouse` is the more stable product/module boundary.
- Keeping `WarehouseDbContext` at the Warehouse level avoids a predictable future move from `WarehouseFoundation/Persistence` to `Warehouse/Persistence`.
- A logical Warehouse boundary is sufficient for the current scope without introducing a separate project.
- Backend logic remains extractable from `Formica.ApiService` later.
- Avoiding a premature DbContext-per-boundary rule prevents unnecessary ceremony and transaction/migration complexity.

### Alternatives Considered

- **Separate `Formica.Modules.Warehouse` project**: may become useful later, but premature for this milestone.
- **`WarehouseFoundation/Persistence/WarehouseFoundationDbContext`**: internally consistent, but likely too narrow if Warehouse Foundation is the first part of a broader Warehouse module.
- **`WarehouseFoundation/Persistence/WarehouseDbContext`**: rejected because the DbContext name would be broader than its folder boundary.
- **Global shared persistence folder**: rejected because it would weaken the Formica Warehouse feature/module boundary.
- **Mandatory DbContext per boundary**: rejected because it would be premature and may create unnecessary transaction, migration, and naming complexity.

### Consequences

- Keep EF Core configurations, migrations, database constraints, and persistence helpers under `Formica.ApiService/Warehouse/Persistence`.
- Keep Warehouse Foundation domain/use-case/API behavior under `Formica.ApiService/Warehouse/WarehouseFoundation`.
- Keep endpoints thin; business behavior must remain inside feature/application/domain code, not endpoint handlers.
- Revisit physical module extraction when Inventory, Inbound, Outbound, or Integrations add real complexity.
- Keep domain behavior outside persistence-specific classes unless it is purely persistence mapping or database constraint configuration.

## R-003: Minimal API Endpoint Grouping

### Decision

Use Minimal API endpoint groups inside:

```text
Formica.ApiService/
└── Warehouse/
    └── WarehouseFoundation/
        └── Endpoints/
```

Use route groups that reflect Warehouse Foundation capabilities.

Expected route groups:

```text
/api/warehouse-foundation/warehouses
/api/warehouse-foundation/zones
/api/warehouse-foundation/storage-locations
/api/warehouse-foundation/skus
/api/warehouse-foundation/warehouses/{warehouseId}/layout
```

`layout` is a read-oriented view of the configured warehouse as warehouse → zone → storage locations. It is not a separate domain entity.

### Rationale

- Minimal API matches the project direction.
- Route groups map to user-visible capabilities from the specification.
- `layout` is clearer than the ambiguous term `structure` for read-only warehouse organization views.
- Keeping endpoints grouped under Warehouse Foundation makes the first feature boundary visible.

### Alternatives Considered

- **Generic `/api/warehouse/...` routes**: reasonable later, but broader than the first milestone.
- **Controller-based API**: not needed for this feature and would move away from the Minimal API direction.
- **Single large endpoint file**: rejected because it would become difficult to maintain as slices grow.

### Consequences

- Endpoint contracts must be designed in `contracts/` before tasks.
- Endpoint grouping should stay capability-oriented, not persistence-table-oriented.
- Endpoint handlers should stay thin and delegate behavior to feature/application/domain code.

## R-004: Blazor Placement

### Decision

Place Blazor UI code under:

```text
Formica.Web/
└── Warehouse/
    └── WarehouseFoundation/
        ├── Pages/
        ├── Components/
        └── ApiClients/
```

`ApiClients` are typed wrappers used by Blazor pages/components to call `Formica.ApiService` endpoints.

### Rationale

- Keeps Formica Warehouse visible in the UI project.
- Keeps Warehouse Foundation UI code close together.
- Separates page composition, reusable components, and API access helpers.
- Avoids confusing `ApiClients` with domain services or external integration clients.

### Alternatives Considered

- **Global `Pages/Warehouse` and `Components/Warehouse` folders**: acceptable but less explicit as a feature boundary.
- **`Services` instead of `ApiClients`**: rejected because `Services` is too broad and tends to collect unrelated concerns.
- **Direct `HttpClient` calls in pages**: rejected because typed API clients make UI code clearer and easier to test/refactor.

### Consequences

- UI API access should stay thin.
- Business rules must remain in API/domain/application behavior, not in Blazor `ApiClients`.

## R-005: Testing Strategy

### Decision

Use a pragmatic mixed testing approach in `Formica.Tests`:

- domain/rule-level tests for lifecycle and validation rules;
- API behavior tests for user-visible scenarios;
- selective Aspire integration tests for API + PostgreSQL behavior where database constraints or orchestration matter.

### Rationale

- The existing test project already uses xUnit v3 and Aspire.Hosting.Testing.
- Full end-to-end tests for every case would be slow and unnecessary.
- Pure unit tests are insufficient for relational uniqueness behavior.
- A mixed approach provides confidence without excessive ceremony.

### Alternatives Considered

- **Only domain/unit tests**: too weak for persistence and API behavior.
- **Only Aspire integration tests**: too slow and heavy for all validation rules.
- **UI automation tests**: out of scope for the first milestone.

### Consequences

- Tasks should include tests at the appropriate level for each rule.
- Database-related behavior should be validated against PostgreSQL where differences matter.
- Tests should map to acceptance criteria rather than internal implementation details.

## R-006: Location Address Rules

### Decision

Model location address rules as simple configurable warehouse-level validation rules.

The first milestone should support user-defined address codes validated by rules such as:

- required value;
- maximum length;
- allowed characters or pattern;
- optional normalization such as trimming and uppercase conversion;
- uniqueness within a warehouse;
- optional zone-related prefix convention if useful.

Do not require a fixed topology model such as aisle/rack/level/position.

### Rationale

- Warehouses use different addressing conventions.
- A fixed topology would overfit the first milestone.
- Simple validation supports consistent and scannable addresses without implementing a location generator.
- The design remains compatible with future topology or address-generation features.

### Alternatives Considered

- **Fixed aisle/rack/level/position model**: too specific and premature.
- **Free text only**: too weak for consistency, scanning, and future workflows.
- **Full address generator**: out of scope for Warehouse Foundation.

### Consequences

- `data-model.md` must define a simple `LocationAddressRules` concept.
- Validation must be deterministic and user-explainable.
- Future features may extend rules into generated address ranges or topology-aware layout.

## R-007: Capacity Attributes

### Decision

Support optional capacity attributes as simple value-object-style data for storage locations.

Candidate attributes:

- maximum weight;
- volume;
- height;
- width;
- depth.

Warehouse Foundation must not calculate capacity consumption, slotting suitability, or optimization.

### Rationale

- Capacity information is useful foundation data for future slotting and putaway decisions.
- Making it optional avoids blocking simple warehouse setup.
- Avoiding calculations keeps the first milestone focused.

### Alternatives Considered

- **No capacity attributes**: simpler, but would omit useful future-facing setup data.
- **Full capacity model with consumption calculations**: out of scope and would pull in inventory/slotting concerns too early.
- **Provider-specific computed values**: unnecessary at this stage.

### Consequences

- Capacity data should be optional.
- Validation should ensure values are non-negative when provided.
- No user story should imply automatic placement or capacity availability calculations in this milestone.

## Deferred Decisions

The following decisions are intentionally deferred to future features or Phase 1 design:

- full Product Catalog modeling;
- inventory balances and movements;
- receiving and putaway workflows;
- handling units and LPN lifecycle;
- barcode scanning workflows;
- 1C synchronization mechanics;
- outbox/inbox mechanics;
- generated warehouse topology/address ranges;
- slotting and capacity consumption calculations;
- physical module extraction into separate projects.
