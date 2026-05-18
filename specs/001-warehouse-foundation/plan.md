# Implementation Plan: Warehouse Foundation

**Branch**: `001-warehouse-foundation` | **Date**: 2026-05-06 | **Spec**: `specs/001-warehouse-foundation/spec.md`  
**Input**: Feature specification from `specs/001-warehouse-foundation/spec.md`

## Summary

Implement Warehouse Foundation as the first Formica Warehouse vertical slice set.

The feature establishes the operational foundation for future warehouse workflows by supporting:

- warehouses;
- zones;
- storage locations;
- configurable location addresses;
- basic SKUs;
- SKU barcodes;
- active/inactive lifecycle;
- basic warehouse layout visibility.

The implementation approach is a pragmatic modular monolith using the existing Aspire Starter App solution:

- `Formica.AppHost` for Aspire orchestration;
- `Formica.ServiceDefaults` for shared service defaults and observability baseline;
- `Formica.ApiService` for the Web API surface and initial backend host;
- `Formica.WebApp` for the MudBlazor-based Blazor UI surface;
- `Formica.Web` as the temporary Bootstrap baseline/legacy UI source until required functionality is migrated and the project is removed;
- `Formica.Tests` for Aspire/xUnit-based tests.

`Formica.ApiService` is the physical host/composition root for the first milestone, not the conceptual owner of Warehouse business logic. Warehouse backend logic must remain organized inside an extractable logical Warehouse boundary.

The feature must not implement full inventory accounting, receiving, putaway, outbound, mobile workflows, 1C synchronization, or optimization. It must prepare stable references that future inventory workflows can use.

Implementation must also follow `specs/001-warehouse-foundation/implementation-guidelines.md`, which defines binding guardrails for internal dependency direction, rich domain model style, identity generation, Location Address Rules, EF Core migration policy, and review branch workflow.

## Technical Context

**Language/Version**: C# / .NET 10  
**Primary Dependencies**: ASP.NET Core, Aspire 13.2.4, Blazor Server, MudBlazor 9.4.0, EF Core, Npgsql EF Core provider, OpenTelemetry baseline through ServiceDefaults
**Storage**: PostgreSQL through EF Core/Npgsql  
**Testing**: `Formica.Tests` using xUnit v3, Microsoft.NET.Test.Sdk, Aspire.Hosting.Testing, coverlet collector  
**Target Platform**: Server-side web application with Web API and Blazor UI, developed and run through Aspire AppHost  
**Project Type**: Modular monolith web application with separate API and UI surfaces  
**Performance Goals**: Foundation administration workflows should remain responsive for ordinary warehouse setup data volumes; no high-throughput operational workflow is introduced by this feature  
**Constraints**: Keep implementation simple; avoid MediatR and unnecessary third-party abstractions; preserve future compatibility with inventory workflows; keep backend business logic extractable from `Formica.ApiService`; keep Domain independent from Features, Endpoints, Persistence, Contracts, ASP.NET Core, and Blazor UI  
**Scale/Scope**: Warehouse Foundation administrative foundation delivered through smaller accepted milestones: M1 covers `Formica.WebApp` foundation plus Warehouse and Zone management; M2 covers Storage Locations; M3 covers SKUs and barcodes; M4 covers Warehouse Layout and lifecycle consistency. No high-throughput operational transaction processing is introduced.

## Existing Solution Baseline

The current solution is `Formica.slnx` and includes:

```text
Formica.slnx
├── Formica.AppHost/Formica.AppHost.csproj
├── Formica.ServiceDefaults/Formica.ServiceDefaults.csproj
├── Formica.ApiService/Formica.ApiService.csproj
├── Formica.WebApp/Formica.WebApp.csproj
├── Formica.Web/Formica.Web.csproj
└── Formica.Tests/Formica.Tests.csproj
```

Observed baseline:

- `Formica.AppHost` targets `net10.0`, uses `Aspire.AppHost.Sdk/13.2.4`, references `Formica.ApiService`, `Formica.WebApp`, and `Formica.Web`, and includes Aspire Redis hosting.
- `Formica.ServiceDefaults` targets `net10.0`, is marked as Aspire shared project, and includes OpenTelemetry packages.
- `Formica.ApiService` targets `net10.0`, references `Formica.ServiceDefaults`, and includes `Microsoft.AspNetCore.OpenApi`.
- `Formica.WebApp` targets `net10.0`, references `Formica.ServiceDefaults`, uses MudBlazor 9.4.0, and includes Aspire Redis output caching.
- `Formica.Web` targets `net10.0`, references `Formica.ServiceDefaults`, includes Aspire Redis output caching, and remains a temporary Bootstrap baseline/legacy source.
- `Formica.Tests` targets `net10.0`, references `Formica.AppHost`, uses xUnit v3 and Aspire.Hosting.Testing.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

### I. Specification-First Development

Status: PASS.

The feature has a reviewed specification at `specs/001-warehouse-foundation/spec.md` with user stories, requirements, success criteria, assumptions, and clarifications. Implementation must not start until this plan, downstream design artifacts, tasks, and analysis are coherent.

### II. WHAT/WHY before HOW

Status: PASS.

The specification defines user-visible behavior and excludes implementation details. This plan introduces implementation approach and structure. Additional implementation guardrails are documented in `implementation-guidelines.md`.

### III. Modular Monolith First

Status: PASS.

The feature will be implemented inside the current Formica modular monolith solution. It will create an explicit logical Formica Warehouse boundary without introducing premature physical project proliferation.

### IV. Clean Architecture without Ceremony

Status: PASS.

Business rules should remain independent from UI, HTTP, and persistence. `Formica.ApiService` is the host/API surface; Warehouse business behavior must stay inside logical module/feature code and remain extractable later. Domain code must not depend on Features, Endpoints, Persistence, Contracts, ASP.NET Core, or Blazor UI.

### V. Vertical Slice Delivery

Status: PASS.

Implementation should be organized around feature capabilities such as warehouse management, zone management, storage location management, SKU management, and warehouse layout viewing. `Features/` means application use cases and orchestration, not a generic shared-kernel folder.

### VI. DDD-Oriented Design

Status: PASS.

DDD concepts are useful for stable identities, value objects, uniqueness rules, lifecycle state, and domain terminology. Full aggregate complexity must be avoided where simple modeling is sufficient. Domain models should be self-contained rich models without excessive ceremony.

### VII. Integration Isolation

Status: PASS.

The feature does not implement 1C, Bitrix24, mobile, scanner, or external integration workflows. SKU barcode support prepares for future integration scenarios but does not couple the domain to external systems.

### VIII. Quality, Observability, and Maintainability

Status: PASS.

Plan includes validation strategy and tests for acceptance criteria. Operational diagnostics should use the existing ServiceDefaults/OpenTelemetry baseline, but this feature does not introduce long-running or high-volume operational processing.

### IX. Minimal Dependencies

Status: PASS.

Use built-in .NET, ASP.NET Core, Blazor, EF Core, Npgsql, Aspire capabilities, and the explicitly approved MudBlazor UI foundation by default. Do not introduce MediatR, external validation frameworks, mapping frameworks, or additional UI libraries unless justified later.

### X. Human-Readable Project Memory

Status: PASS.

This plan references existing project memory and does not promote feature-specific implementation choices to global memory.

## Technical Decisions

### TD-001: Implement inside the existing Aspire solution

Warehouse Foundation will use the current solution baseline:

- API host and composition in `Formica.ApiService`;
- Warehouse backend logic inside a logical `Warehouse` boundary under `Formica.ApiService` for this milestone;
- MudBlazor-based Blazor UI behavior in `Formica.WebApp`;
- temporary Bootstrap UI behavior in `Formica.Web` only as a migration source until removal;
- orchestration in `Formica.AppHost` for PostgreSQL and supporting resources;
- shared telemetry/service defaults through `Formica.ServiceDefaults`;
- tests in `Formica.Tests`.

Rationale:

- aligns with the existing Aspire Starter App structure;
- avoids premature new projects;
- keeps the first milestone implementation approachable;
- keeps Warehouse code extractable if a dedicated module project becomes justified later;
- leaves room for future module extraction if Inventory, Inbound, Outbound, or Integrations grow.

### TD-002: Use a logical Formica Warehouse boundary first

Create an explicit logical feature/module boundary for Formica Warehouse inside the existing projects.

Expected logical grouping:

- `Warehouse/Persistence` under API-facing backend code for Warehouse-level persistence;
- `Warehouse/WarehouseFoundation` under API-facing backend code for this feature/milestone;
- `Warehouse/WarehouseFoundation` under Blazor UI code;
- domain/application/persistence concerns grouped by logical boundary, not scattered globally.

Rationale:

- keeps Formica Warehouse visible as the first product module;
- supports modular monolith direction;
- avoids extra assemblies before real complexity justifies them;
- avoids placing a broad `WarehouseDbContext` inside the narrower `WarehouseFoundation` feature folder.

### TD-003: Use vertical slices for user-visible capabilities

The implementation should be grouped around independently testable capabilities:

- create and maintain warehouses;
- create and maintain zones;
- create and maintain storage locations and addresses;
- create and maintain basic SKUs and barcode values;
- view the warehouse layout.

Rationale:

- maps directly to user stories;
- supports incremental implementation;
- avoids horizontal service-layer ceremony.

### TD-004: Use PostgreSQL through EF Core/Npgsql for relational persistence

Warehouse Foundation requires durable storage for warehouses, zones, storage locations, address rules, SKUs, and barcode values. PostgreSQL should be used through EF Core and the Npgsql provider.

Rationale:

- aligns well with Aspire/container-based development;
- provides mature EF Core support;
- supports relational uniqueness constraints needed by the feature;
- avoids unnecessary SQL Server/Windows coupling;
- leaves room for future operational data, outbox/inbox, JSONB, and analytical scenarios.

EF Core migration files must not be generated automatically during foundational setup. Migrations are created only after explicit instruction and only when there is a coherent persisted entity model.

### TD-005: Use explicit domain concepts but avoid over-modeling

The feature should model the following concepts explicitly:

- Warehouse;
- Zone;
- Storage Location;
- Location Address;
- Location Address Rules;
- SKU;
- SKU Barcode;
- Unit of Measure;
- active/inactive lifecycle.

Rationale:

- these concepts are already defined by the spec and glossary;
- stable references are required for future inventory workflows;
- explicit concepts reduce ambiguity.

Avoid introducing full Inventory, Receiving, Putaway, Handling Unit, LPN, Product Catalog, or Slotting models in this feature.

### TD-006: Addressing uses configurable default rules

Location addresses should be user-defined codes validated by configurable default Warehouse Foundation address rules.

For this milestone, Location Address Rules are the default address policy for the current Formica installation. They are not modeled as a Warehouse child entity during Phase 2. Warehouse-specific overrides may be introduced later only by an explicit feature decision.

The first implementation should not require a fixed topology such as aisle/rack/level/position.

Rationale:

- supports different warehouse address conventions;
- avoids premature topology modeling;
- avoids introducing `WarehouseId` as a placeholder primary key before Warehouse exists;
- satisfies the spec clarification while keeping the first milestone simple.

### TD-007: Use deactivation rather than destructive removal for operational references

Warehouses, zones, storage locations, and SKUs should support active/inactive lifecycle. Reactivation is allowed when uniqueness and validation rules still pass.

Rationale:

- preserves stable references for future workflows;
- avoids destructive changes to operational setup;
- aligns with the specification and future auditability needs.

### TD-008: Barcode model allows multiple barcodes per SKU

A SKU may have multiple barcode values. Barcode values must be unique across SKUs unless a future feature defines reuse or aliasing rules.

Rationale:

- supports practical 1C-style nomenclature and catalog scenarios;
- prevents ambiguous barcode-to-SKU resolution;
- avoids implementing full integration-specific barcode semantics in this milestone.

### TD-009: Use lightweight command/query convention inside vertical slices

Use explicit command/query naming inside feature files, without introducing MediatR, a global dispatcher, or generic CQRS framework for this milestone.

Examples:

```text
Features/Warehouses/CreateWarehouse.cs
Features/Warehouses/ListWarehouses.cs
Features/Warehouses/GetWarehouse.cs
Features/WarehouseLayout/GetWarehouseLayout.cs
```

Commands mutate state. Queries read state. Endpoint handlers should call the relevant feature handler directly and remain thin.

Rationale:

- keeps intent clear;
- supports vertical slices;
- avoids premature dispatcher abstractions;
- leaves room for a simple internal dispatcher later if repetition or cross-cutting pipelines justify it.

### TD-010: Use MudBlazor-based Blazor Server UI in Formica.WebApp

Warehouse Foundation should use `Formica.WebApp` as the target Blazor Server UI project and MudBlazor 9.4.0 as the approved component library.

`Formica.Web` remains a temporary Bootstrap baseline and migration source while already implemented UI behavior is moved into `Formica.WebApp`. New Warehouse Foundation UI work should be implemented in `Formica.WebApp`, not split between both UI projects.

MudBlazor is approved for expected administration workspace patterns such as tables, tabs, dialogs or drawers, forms, validation presentation, and stateful management components. This decision does not approve unrelated UI libraries or non-UI frameworks.

Rationale:

- supports richer warehouse administration workflows without hand-rolling Bootstrap-heavy CRUD surfaces;
- keeps the target UI implementation in one project before `Formica.Web` is removed;
- provides a consistent component foundation for tables, forms, status indicators, and workspace navigation;
- keeps business rules in API/domain/application code rather than UI components.

### TD-011: Split Warehouse Foundation into smaller accepted milestones

Warehouse Foundation remains one product feature, but implementation and review should be accepted in smaller milestones:

- M1: `Formica.WebApp` foundation plus Warehouse and Zone management end-to-end;
- M2: Storage Locations and configured address behavior end-to-end;
- M3: SKU, Unit of Measure, and barcode behavior end-to-end;
- M4: Warehouse Layout, lifecycle consistency, and final cross-cutting validation.

Rationale:

- keeps review scope manageable;
- lets the MudBlazor migration land before expanding the UI surface;
- avoids treating Storage Locations and SKUs as blockers for accepting the first working Warehouse/Zone vertical slice;
- keeps the full Warehouse Foundation scope intact while reducing per-pass implementation risk.

## Project Structure

### Documentation (this feature)

```text
specs/001-warehouse-foundation/
├── spec.md                         # Feature specification: WHAT/WHY
├── plan.md                         # Implementation plan: HOW
├── research.md                     # Phase 0 research decisions
├── data-model.md                   # Phase 1 data/domain model design
├── implementation-guidelines.md    # Binding implementation guardrails
├── quickstart.md                   # Phase 1 validation walkthrough
├── contracts/                      # Phase 1 API contracts, if needed
└── tasks.md                        # Executable implementation tasks
```

### Source Code (repository root)

Use the existing Aspire solution structure:

```text
Formica.slnx
├── Formica.AppHost/
├── Formica.ServiceDefaults/
├── Formica.ApiService/
├── Formica.WebApp/
├── Formica.Web/
└── Formica.Tests/
```

Expected logical feature placement:

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
        │   ├── Common/
        │   ├── LocationAddressing/
        │   └── StorageLocations/
        ├── Features/
        │   ├── Warehouses/
        │   ├── Zones/
        │   ├── StorageLocations/
        │   ├── Skus/
        │   └── WarehouseLayout/
        └── Endpoints/

Formica.WebApp/
└── Warehouse/
    └── WarehouseFoundation/
        ├── Pages/
        ├── Components/
        └── ApiClients/

Formica.Web/
└── Warehouse/
    └── WarehouseFoundation/        # temporary Bootstrap baseline/migration source only

Formica.Tests/
└── Warehouse/
    └── WarehouseFoundation/
```

`WarehouseLayout` is the read-oriented capability that presents the configured warehouse as warehouse → zone → storage locations. It is not a separate domain entity.

`ApiClients` are typed wrappers used by the Blazor UI to call `Formica.ApiService` endpoints. They are UI-facing HTTP access helpers, not domain services, integration clients, or business entities. New Warehouse Foundation UI clients should live under `Formica.WebApp`.

`Formica.AppHost` should orchestrate PostgreSQL for local development and integration testing once persistence is added.

`Formica.ServiceDefaults` should be reused for telemetry and service defaults; this feature should not introduce feature-specific observability infrastructure unless needed.

**Structure Decision**: Implement Warehouse Foundation as a logical Warehouse module/feature area inside the existing projects. Do not create `Formica.Modules.Warehouse` or other new projects for this milestone unless Phase 0 research finds a concrete need.

## Phase 0: Research

Research is required for decisions that remain open after this plan.

Required research topics:

1. Confirm PostgreSQL/Npgsql package and Aspire orchestration setup.
2. Confirm Warehouse-level persistence boundary under `Formica.ApiService/Warehouse/Persistence` and Warehouse Foundation feature boundary under `Formica.ApiService/Warehouse/WarehouseFoundation`.
3. Confirm Minimal API endpoint grouping conventions in `Formica.ApiService`.
4. Confirm Blazor routing/component placement conventions in `Formica.WebApp`.
5. Confirm test style in `Formica.Tests`: integration-first through Aspire, domain tests, API behavior tests, or a pragmatic mix.
6. Confirm initial representation of configurable location address rules.
7. Confirm whether optional capacity attributes are simple scalar fields or value-object-style concepts.

Expected output:

```text
specs/001-warehouse-foundation/research.md
```

## Phase 1: Design

Design must produce technology-aware but implementation-light artifacts before tasks.

Expected outputs:

```text
specs/001-warehouse-foundation/data-model.md
specs/001-warehouse-foundation/contracts/
specs/001-warehouse-foundation/quickstart.md
```

### Data Model Design Scope

`data-model.md` should define the feature-level domain/data model for:

- Warehouse;
- Zone;
- Storage Location;
- Location Address Rules;
- SKU;
- SKU Barcode;
- Unit of Measure;
- lifecycle state;
- uniqueness rules;
- relationships between these concepts.

It must explicitly exclude:

- Inventory Balance;
- Inventory Movement;
- Receiving;
- Putaway;
- Handling Unit;
- LPN;
- Outbound;
- integration synchronization models.

### Contracts Design Scope

Contracts are needed for API-facing behavior.

Contracts should cover user-visible operations from the specification, not persistence schema.

Expected contract groups:

- warehouses;
- zones;
- storage locations;
- SKUs;
- warehouse layout view.

### Quickstart Scope

`quickstart.md` should describe how to validate the feature manually or through tests:

1. create a warehouse;
2. create zones;
3. create storage locations with addresses;
4. verify duplicate prevention;
5. create SKUs with multiple barcode values;
6. verify duplicate barcode prevention;
7. view warehouse layout;
8. deactivate and reactivate configured references.

## Validation Strategy

The implementation must validate the feature through a pragmatic mix of tests appropriate to the final source structure.

Minimum validation coverage:

- warehouse code uniqueness;
- zone code uniqueness within warehouse;
- location address uniqueness within warehouse;
- SKU code uniqueness;
- multiple barcode values per SKU;
- barcode value uniqueness across SKUs;
- active/inactive lifecycle;
- reactivation validation;
- warehouse layout view behavior;
- location address rule validation.

Testing should focus on user-visible behavior and domain rules, not implementation details.

## Complexity Tracking

No constitution violations are currently expected.

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| N/A | N/A | N/A |
