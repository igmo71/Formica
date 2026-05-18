# Implementation Plan: Warehouse Foundation

**Branch**: `001-warehouse-foundation` | **Date**: 2026-05-06 | **Spec**: `specs/001-warehouse-foundation/spec.md`

## Summary

Warehouse Foundation is the first Formica Warehouse vertical-slice set. It establishes warehouses, zones, storage locations, configurable location addresses, basic SKUs, SKU barcodes, active/inactive lifecycle, and basic warehouse layout visibility.

The implementation remains a pragmatic modular monolith:

- `Formica.AppHost`: Aspire orchestration.
- `Formica.ServiceDefaults`: shared service defaults and observability baseline.
- `Formica.ApiService`: Web API host/composition root and first physical home for Warehouse backend code.
- `Formica.WebApp`: target MudBlazor-based Blazor Server UI.
- `Formica.Web`: temporary Bootstrap baseline/migration source only.
- `Formica.Tests`: xUnit/Aspire tests.

`Formica.ApiService` is not the conceptual owner of Warehouse business logic. Warehouse behavior must stay inside an extractable logical Warehouse boundary.

## Technical Context

**Language/Version**: C# / .NET 10
**Primary Dependencies**: ASP.NET Core, Aspire 13.2.4, Blazor Server, MudBlazor 9.4.0, EF Core, Npgsql EF Core provider, OpenTelemetry through ServiceDefaults
**Storage**: PostgreSQL through EF Core/Npgsql
**Testing**: xUnit v3, Microsoft.NET.Test.Sdk, Aspire.Hosting.Testing, coverlet collector
**Target Platform**: Server-side web application with Web API and Blazor UI, run through Aspire AppHost
**Constraints**: Keep implementation simple; avoid MediatR, generic dispatchers, mapping frameworks, and unnecessary abstractions; keep Domain independent from UI, HTTP, persistence, endpoint contracts, and Blazor/MudBlazor.
**Scale/Scope**: Delivered through smaller accepted milestones: M1 `Formica.WebApp` + Warehouse/Zone management; M2 Storage Locations; M3 SKUs/barcodes; M4 Warehouse Layout and lifecycle consistency.

## Constitution Check

All constitution gates are currently **PASS**.

- `spec.md` holds WHAT/WHY.
- This plan holds durable HOW decisions.
- `implementation-guidelines.md` holds implementation guardrails.
- `tasks.md` holds executable work.
- No known constitution violation or complexity exception is currently required.

MudBlazor is a justified local UI foundation decision for Warehouse Foundation and does not approve unrelated UI libraries or non-UI frameworks.

## Technical Decisions

### TD-001: Existing Aspire Solution

Use the existing Aspire solution. Keep backend host/composition in `Formica.ApiService`, orchestration in `Formica.AppHost`, shared defaults in `Formica.ServiceDefaults`, target UI in `Formica.WebApp`, and tests in `Formica.Tests`.

### TD-002: Logical Warehouse Boundary

Keep Warehouse persistence at `Formica.ApiService/Warehouse/Persistence` and Warehouse Foundation backend behavior at `Formica.ApiService/Warehouse/WarehouseFoundation`.

Do not create `Formica.Modules.Warehouse` yet. Revisit physical extraction only when Inventory, Inbound, Outbound, or Integrations add enough complexity to justify a separate project.

### TD-003: Vertical Slices

Implement around user-visible capabilities:

- warehouse management;
- zone management;
- storage location management;
- SKU/barcode management;
- warehouse layout view.

Use explicit command/query naming inside feature files without introducing MediatR or a dispatcher framework.

### TD-004: PostgreSQL Persistence

Use PostgreSQL through EF Core/Npgsql. Warehouse Foundation needs relational uniqueness constraints for warehouse codes, zone codes, normalized location addresses, SKU codes, and barcode values.

EF migrations are created only after an explicit task/instruction and only for coherent persisted models.

### TD-005: Domain Concepts

Model these concepts explicitly without over-modeling full inventory workflows:

- Warehouse;
- Zone;
- Storage Location;
- Location Address and Location Address Rules;
- SKU and SKU Barcode;
- Unit of Measure;
- active/inactive lifecycle.

Do not introduce Inventory Balance, Inventory Movement, Receiving, Putaway, Handling Unit, LPN, Product Catalog, Slotting, 1C synchronization, scanners, or mobile workflows in this feature.

### TD-006: Configurable Address Rules

Location addresses are user-defined codes validated by default Warehouse Foundation address rules. Do not require fixed aisle/rack/level/position topology. Warehouse-specific rule overrides are deferred.

### TD-007: Stable References and Lifecycle

Warehouses, zones, storage locations, and SKUs use stable IDs and active/inactive lifecycle. Normal workflows use deactivation/reactivation instead of physical deletion.

### TD-008: SKU Barcodes

A SKU may have multiple barcode values. Barcode values are unique across SKUs unless a future feature explicitly introduces reuse or aliasing.

### TD-009: MudBlazor UI in Formica.WebApp

`Formica.WebApp` is the target Blazor Server UI project. Use MudBlazor 9.4.0 for administration workspace patterns such as tables, forms, validation display, tabs, dialogs/drawers, and status indicators.

`Formica.Web` remains a temporary Bootstrap baseline/migration source. New Warehouse Foundation UI work goes into `Formica.WebApp`.

### TD-010: Accepted Milestones

Warehouse Foundation remains one product feature but is accepted in smaller milestones:

- **M1**: `Formica.WebApp` foundation plus Warehouse and Zone management end-to-end.
- **M2**: Storage Locations and configured address behavior end-to-end.
- **M3**: SKU, Unit of Measure, and barcode behavior end-to-end.
- **M4**: Warehouse Layout, lifecycle consistency, and final cross-cutting validation.

## Project Structure

Expected source placement:

```text
Formica.ApiService/
└── Warehouse/
    ├── Persistence/
    └── WarehouseFoundation/
        ├── Domain/
        ├── Features/
        ├── Contracts/
        └── Endpoints/

Formica.WebApp/
└── Warehouse/
    └── WarehouseFoundation/
        ├── Pages/
        ├── Components/
        └── ApiClients/

Formica.Web/
└── Warehouse/
    └── WarehouseFoundation/        # temporary migration source only

Formica.Tests/
└── Warehouse/
    └── WarehouseFoundation/
```

`ApiClients` are thin UI-facing HTTP wrappers. They are not domain services, integration clients, or business rule containers.

## Design Outputs

This feature uses the following supporting artifacts:

- `research.md`: decision rationale.
- `data-model.md`: domain/data model reference.
- `contracts/api-contracts.md`: API contract reference.
- `implementation-guidelines.md`: binding implementation guardrails.
- `quickstart.md`: manual validation walkthrough.
- `tasks.md`: executable implementation work.

## Validation Strategy

Use a pragmatic mix of:

- domain/rule tests for lifecycle and validation;
- API behavior tests for user-visible scenarios;
- PostgreSQL-backed persistence tests where relational constraints matter;
- targeted manual UI validation through `Formica.WebApp`.

Minimum behavior coverage: uniqueness rules, address normalization, SKU/barcode uniqueness, active/inactive lifecycle, reactivation validation, layout view, and default Location Address Rules.
