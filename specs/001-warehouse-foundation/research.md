# Research: Warehouse Foundation

**Feature**: `001-warehouse-foundation`
**Spec**: `specs/001-warehouse-foundation/spec.md`
**Plan**: `specs/001-warehouse-foundation/plan.md`
**Date**: 2026-05-07

## Purpose

This document records Phase 0 technical decisions. It preserves rationale for choices that are not obvious from `spec.md` or `plan.md`.

## Decision Summary

| ID | Decision |
|----|----------|
| R-001 | Use PostgreSQL through EF Core/Npgsql. |
| R-002 | Keep Warehouse persistence under `Formica.ApiService/Warehouse/Persistence`; keep Warehouse Foundation backend behavior under `Formica.ApiService/Warehouse/WarehouseFoundation`. |
| R-003 | Use Minimal API endpoint groups under `Formica.ApiService/Warehouse/WarehouseFoundation/Endpoints`. |
| R-004 | Use MudBlazor-based Blazor pages/components/API clients under `Formica.WebApp/Warehouse/WarehouseFoundation`. |
| R-005 | Use a pragmatic mixed testing approach in `Formica.Tests`. |
| R-006 | Model location address rules as simple configurable validation rules. |
| R-007 | Model capacity attributes as optional value-object-style data without capacity calculations. |

## R-001: PostgreSQL through EF Core/Npgsql

**Decision**: Use PostgreSQL as the initial relational database provider, accessed through EF Core and Npgsql. Aspire orchestrates PostgreSQL for local development and integration testing.

**Rationale**: PostgreSQL fits Aspire/container-based development, supports relational constraints, avoids unnecessary SQL Server coupling, and leaves room for future operational data patterns.

**Alternatives**: SQL Server was rejected as unnecessarily Microsoft-centric for the current direction. SQLite was rejected as too far from production-like relational constraint behavior.

**Consequences**: Add Npgsql/EF packages, use Aspire PostgreSQL hosting, and validate database-sensitive behavior against PostgreSQL.

## R-002: Backend Module and Persistence Placement

**Decision**: Use `Formica.ApiService` as the first physical host, but keep Warehouse code inside a logical Warehouse boundary. Keep `WarehouseDbContext` and EF configuration under `Formica.ApiService/Warehouse/Persistence`.

**Rationale**: `Warehouse` is the stable module boundary; `WarehouseFoundation` is the first milestone inside it. This avoids premature project proliferation and avoids putting a broad `WarehouseDbContext` inside a narrower feature folder.

**Alternatives**: A separate `Formica.Modules.Warehouse` project may be useful later, but is premature now. A `WarehouseFoundationDbContext` is too narrow for the likely module trajectory.

**Consequences**: Keep persistence infrastructure Warehouse-scoped, keep feature/domain/API behavior under Warehouse Foundation, and revisit physical extraction only after broader Warehouse capabilities justify it.

## R-003: Minimal API Endpoint Grouping

**Decision**: Use capability-oriented Minimal API groups for warehouses, zones, storage locations, SKUs, and warehouse layout.

**Rationale**: Minimal API matches project direction and maps cleanly to vertical slices. `layout` is a read-oriented view, not a separate persisted domain entity.

**Consequences**: Keep endpoints thin, capability-oriented, and backed by contract DTOs and feature handlers.

## R-004: Blazor Placement and UI Foundation

**Decision**: Put target UI code under `Formica.WebApp/Warehouse/WarehouseFoundation` and use MudBlazor 9.4.0. `Formica.Web` remains a temporary Bootstrap migration source only.

**Rationale**: Warehouse Foundation needs administration workspace patterns such as tables, forms, tabs, dialogs/drawers, status indicators, and validation presentation. MudBlazor avoids expanding custom Bootstrap UI code.

**Alternatives**: Continuing Bootstrap-first in `Formica.Web` was rejected because the UI is being replaced by `Formica.WebApp`.

**Consequences**: New Warehouse Foundation UI work goes into `Formica.WebApp`; UI API access remains thin through `ApiClients`; business rules remain in API/domain/application behavior.

## R-005: Testing Strategy

**Decision**: Use a pragmatic mix of domain/rule tests, API behavior tests, and selective Aspire/PostgreSQL integration tests.

**Rationale**: Unit tests alone are insufficient for persistence constraints; full end-to-end coverage for every case would be too slow.

**Consequences**: Tests map to acceptance criteria. Database-sensitive behavior is validated against PostgreSQL where differences matter. UI automation remains out of scope for the first milestones.

## R-006: Location Address Rules

**Decision**: Use configurable address validation rules for user-defined location addresses. Do not require a fixed aisle/rack/level/position topology.

**Rationale**: Warehouses use different address conventions. Simple validation supports consistency without implementing a generator or topology model too early.

**Consequences**: Address validation must be deterministic and user-explainable. Future features may add generated ranges, topology-aware layout, or warehouse-specific overrides.

## R-007: Capacity Attributes

**Decision**: Support optional capacity attributes such as max weight, volume, height, width, and depth as value-object-style data.

**Rationale**: Capacity data is useful foundation data, but capacity consumption, slotting suitability, and optimization are out of scope.

**Consequences**: Capacity values are optional and non-negative when provided. No user story implies automatic placement or capacity availability calculation.

## Deferred Decisions

Deferred to future features:

- Product Catalog modeling;
- inventory balances and movements;
- receiving, putaway, slotting, handling units, and LPN lifecycle;
- barcode scanning workflows;
- 1C synchronization;
- outbox/inbox mechanics;
- generated topology/address ranges;
- physical module extraction.
