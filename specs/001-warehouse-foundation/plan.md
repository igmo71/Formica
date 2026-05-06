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
- basic SKU references;
- SKU barcode references;
- active/inactive lifecycle;
- basic warehouse structure visibility.

The implementation approach is a pragmatic modular monolith using the existing Aspire Starter App solution:

- `Formica.AppHost` for Aspire orchestration;
- `Formica.ServiceDefaults` for shared service defaults and observability baseline;
- `Formica.ApiService` for the Web API surface;
- `Formica.Web` for the Blazor UI surface;
- `Formica.Tests` for Aspire/xUnit-based tests.

The feature must not implement full inventory accounting, receiving, putaway, outbound, mobile workflows, 1C synchronization, or optimization. It must prepare stable references that future inventory workflows can use.

## Technical Context

**Language/Version**: C# / .NET 10  
**Primary Dependencies**: ASP.NET Core, Aspire 13.2.4, Blazor, EF Core to be added for persistence, OpenTelemetry baseline through ServiceDefaults  
**Storage**: Relational persistence through EF Core; concrete provider to be selected during Phase 0 research before implementation tasks  
**Testing**: `Formica.Tests` using xUnit v3, Microsoft.NET.Test.Sdk, Aspire.Hosting.Testing, coverlet collector  
**Target Platform**: Server-side web application with Web API and Blazor UI, developed and run through Aspire AppHost  
**Project Type**: Modular monolith web application with separate API and UI surfaces  
**Performance Goals**: Foundation administration workflows should remain responsive for ordinary warehouse setup data volumes; no high-throughput operational workflow is introduced by this feature  
**Constraints**: Keep implementation simple; avoid MediatR and unnecessary third-party abstractions; preserve future compatibility with inventory workflows  
**Scale/Scope**: First milestone administrative foundation; expected to support multiple warehouses, zones, storage locations, and SKU references, but not high-volume operational transaction processing yet

## Existing Solution Baseline

The current solution is `Formica.slnx` and includes:

```text
Formica.slnx
├── Formica.AppHost/Formica.AppHost.csproj
├── Formica.ServiceDefaults/Formica.ServiceDefaults.csproj
├── Formica.ApiService/Formica.ApiService.csproj
├── Formica.Web/Formica.Web.csproj
└── Formica.Tests/Formica.Tests.csproj
```

Observed baseline:

- `Formica.AppHost` targets `net10.0`, uses `Aspire.AppHost.Sdk/13.2.4`, references `Formica.ApiService` and `Formica.Web`, and includes Aspire Redis hosting.
- `Formica.ServiceDefaults` targets `net10.0`, is marked as Aspire shared project, and includes OpenTelemetry packages.
- `Formica.ApiService` targets `net10.0`, references `Formica.ServiceDefaults`, and includes `Microsoft.AspNetCore.OpenApi`.
- `Formica.Web` targets `net10.0`, references `Formica.ServiceDefaults`, and includes Aspire Redis output caching.
- `Formica.Tests` targets `net10.0`, references `Formica.AppHost`, uses xUnit v3 and Aspire.Hosting.Testing.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

### I. Specification-First Development

Status: PASS.

The feature has a reviewed specification at `specs/001-warehouse-foundation/spec.md` with user stories, requirements, success criteria, assumptions, and clarifications. Implementation must not start until this plan, downstream design artifacts, tasks, and analysis are coherent.

### II. WHAT/WHY before HOW

Status: PASS.

The specification defines user-visible behavior and excludes implementation details. This plan introduces implementation approach and structure.

### III. Modular Monolith First

Status: PASS.

The feature will be implemented inside the current Formica modular monolith solution. It will create an explicit Formica Warehouse boundary without introducing premature physical project proliferation.

### IV. Clean Architecture without Ceremony

Status: PASS.

Business rules should remain independent from UI, HTTP, and persistence. Additional abstractions must be introduced only when they protect a real boundary or reduce concrete complexity.

### V. Vertical Slice Delivery

Status: PASS.

Implementation should be organized around feature capabilities such as warehouse management, zone management, storage location management, SKU reference management, and warehouse structure viewing.

### VI. DDD-Oriented Design

Status: PASS.

DDD concepts are useful for stable identities, value objects, uniqueness rules, lifecycle state, and domain terminology. Full aggregate complexity must be avoided where simple modeling is sufficient.

### VII. Integration Isolation

Status: PASS.

The feature does not implement 1C, Bitrix24, mobile, scanner, or external integration workflows. SKU barcode support prepares for future integration scenarios but does not couple the domain to external systems.

### VIII. Quality, Observability, and Maintainability

Status: PASS.

Plan includes validation strategy and tests for acceptance criteria. Operational diagnostics should use the existing ServiceDefaults/OpenTelemetry baseline, but this feature does not introduce long-running or high-volume operational processing.

### IX. Minimal Dependencies

Status: PASS.

Use built-in .NET, ASP.NET Core, Blazor, EF Core, and Aspire capabilities by default. Do not introduce MediatR, external validation frameworks, mapping frameworks, or additional UI libraries unless justified later.

### X. Human-Readable Project Memory

Status: PASS.

This plan references existing project memory and does not promote feature-specific implementation choices to global memory.

## Technical Decisions

### TD-001: Implement inside the existing Aspire solution

Warehouse Foundation will use the current solution baseline:

- API behavior in `Formica.ApiService`;
- Blazor UI behavior in `Formica.Web`;
- orchestration in `Formica.AppHost` if persistence or supporting resources are added;
- shared telemetry/service defaults through `Formica.ServiceDefaults`;
- tests in `Formica.Tests`.

Rationale:

- aligns with the existing Aspire Starter App structure;
- avoids premature new projects;
- keeps the first milestone implementation approachable;
- leaves room for future module extraction if Inventory, Inbound, Outbound, or Integrations grow.

### TD-002: Use a logical Formica Warehouse boundary first

Create an explicit logical feature/module boundary for Formica Warehouse inside the existing projects.

Expected logical grouping:

- `Warehouse/WarehouseFoundation` under API-facing code;
- `Warehouse/WarehouseFoundation` under Blazor UI code;
- shared domain/application/persistence concepts grouped by feature area, not scattered globally.

Rationale:

- keeps Formica Warehouse visible as the first product module;
- supports modular monolith direction;
- avoids extra assemblies before real complexity justifies them.

### TD-003: Use vertical slices for user-visible capabilities

The implementation should be grouped around independently testable capabilities:

- create and maintain warehouses;
- create and maintain zones;
- create and maintain storage locations and addresses;
- create and maintain basic SKU references and barcode values;
- view the warehouse structure.

Rationale:

- maps directly to user stories;
- supports incremental implementation;
- avoids horizontal service-layer ceremony.

### TD-004: Use EF Core for relational persistence

Warehouse Foundation requires durable storage for warehouses, zones, storage locations, address rules, SKU references, and barcode values. EF Core should be used for persistence.

Rationale:

- aligns with the .NET/Aspire project direction;
- supports relational uniqueness constraints;
- keeps persistence implementation conventional and maintainable.

The concrete provider is not present in the current project files and must be selected during Phase 0 research before implementation tasks are finalized.

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

### TD-006: Addressing uses configurable warehouse-level rules

Location addresses should be user-defined codes validated by configurable warehouse-level address rules.

The first implementation should not require a fixed topology such as aisle/rack/level/position.

Rationale:

- supports different warehouse address conventions;
- avoids premature topology modeling;
- satisfies the spec clarification while keeping the first milestone simple.

### TD-007: Use deactivation rather than destructive removal for operational references

Warehouses, zones, storage locations, and SKUs should support active/inactive lifecycle. Reactivation is allowed when uniqueness and validation rules still pass.

Rationale:

- preserves stable references for future workflows;
- avoids destructive changes to operational setup;
- aligns with the specification and future auditability needs.

### TD-008: Barcode model allows multiple barcodes per SKU

A SKU may have multiple barcode values. Barcode values must be unique across SKU references unless a future feature defines reuse or aliasing rules.

Rationale:

- supports practical 1C-style nomenclature and catalog scenarios;
- prevents ambiguous barcode-to-SKU resolution;
- avoids implementing full integration-specific barcode semantics in this milestone.

## Project Structure

### Documentation (this feature)

```text
specs/001-warehouse-foundation/
├── spec.md              # Feature specification: WHAT/WHY
├── plan.md              # Implementation plan: HOW
├── research.md          # Phase 0 research decisions
├── data-model.md        # Phase 1 data/domain model design
├── quickstart.md        # Phase 1 validation walkthrough
├── contracts/           # Phase 1 API contracts, if needed
└── tasks.md             # Phase 2 executable tasks; not created by this plan
```

### Source Code (repository root)

Use the existing Aspire solution structure:

```text
Formica.slnx
├── Formica.AppHost/
├── Formica.ServiceDefaults/
├── Formica.ApiService/
├── Formica.Web/
└── Formica.Tests/
```

Expected logical feature placement:

```text
Formica.ApiService/
└── Warehouse/
    └── WarehouseFoundation/
        ├── Domain/
        ├── Persistence/
        ├── Features/
        │   ├── Warehouses/
        │   ├── Zones/
        │   ├── StorageLocations/
        │   ├── SkuReferences/
        │   └── WarehouseStructure/
        └── Endpoints/

Formica.Web/
└── Warehouse/
    └── WarehouseFoundation/
        ├── Pages/
        ├── Components/
        └── Clients/

Formica.Tests/
└── Warehouse/
    └── WarehouseFoundation/
```

`Formica.AppHost` should be updated only if the selected persistence provider or other Aspire-managed resources require orchestration changes.

`Formica.ServiceDefaults` should be reused for telemetry and service defaults; this feature should not introduce feature-specific observability infrastructure unless needed.

**Structure Decision**: Implement Warehouse Foundation as a logical module/feature area inside the existing projects. Do not create `Formica.Modules.Warehouse` or other new projects for this milestone unless Phase 0 research finds a concrete need.

## Phase 0: Research

Research is required for decisions that remain open after this plan.

Required research topics:

1. Select the relational database provider for EF Core in the Aspire solution.
2. Decide whether persistence is introduced directly in `Formica.ApiService` or through a small internal infrastructure grouping under the Warehouse feature area.
3. Confirm Minimal API endpoint grouping conventions in `Formica.ApiService`.
4. Confirm Blazor routing/component placement conventions in `Formica.Web`.
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
- SKU references;
- warehouse structure view.

### Quickstart Scope

`quickstart.md` should describe how to validate the feature manually or through tests:

1. create a warehouse;
2. create zones;
3. create storage locations with addresses;
4. verify duplicate prevention;
5. create SKU references with multiple barcode values;
6. verify duplicate barcode prevention;
7. view warehouse structure;
8. deactivate and reactivate configured references.

## Validation Strategy

The implementation must validate the feature through a pragmatic mix of tests appropriate to the final source structure.

Minimum validation coverage:

- warehouse code uniqueness;
- zone code uniqueness within warehouse;
- location address uniqueness within warehouse;
- SKU code uniqueness;
- multiple barcode values per SKU;
- barcode value uniqueness across SKU references;
- active/inactive lifecycle;
- reactivation validation;
- warehouse structure view behavior;
- location address rule validation.

Testing should focus on user-visible behavior and domain rules, not implementation details.

## Complexity Tracking

No constitution violations are currently expected.

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| N/A | N/A | N/A |
