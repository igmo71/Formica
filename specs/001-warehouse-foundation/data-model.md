# Data Model: Warehouse Foundation

**Feature**: `001-warehouse-foundation`  
**Spec**: `specs/001-warehouse-foundation/spec.md`  
**Plan**: `specs/001-warehouse-foundation/plan.md`  
**Research**: `specs/001-warehouse-foundation/research.md`  
**Date**: 2026-05-07

## Purpose

This document defines the Phase 1 data/domain model for Warehouse Foundation.

It describes concepts, relationships, lifecycle rules, uniqueness constraints, validation rules, and the intended persistence boundary. It is not executable code, database DDL, API contract, UI design, or task breakdown.

## Model Scope

### In Scope

- Warehouse
- Zone
- Storage Location
- Location Address
- Location Address Rules
- Storage Location Capacity
- SKU
- SKU Barcode
- Unit of Measure
- lifecycle state
- stable identity
- uniqueness rules
- Warehouse Layout read model
- Warehouse-level persistence boundary

### Out of Scope

- full Product Catalog
- Inventory Balance
- Inventory Movement
- Inventory Adjustment
- Inventory Count
- Receiving
- Putaway
- Handling Unit
- LPN
- Picking
- Packing
- Shipping
- 1C synchronization models
- Bitrix24 integration models
- generated address ranges
- slotting and capacity consumption calculations

## Persistence Boundary

Warehouse Foundation contributes the first set of entities and configurations to the Warehouse-level persistence boundary.

Initial persistence placement:

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

`WarehouseDbContext` is a Warehouse-level persistence boundary, not a Warehouse Foundation domain concept.

Warehouse Foundation entities may be configured under:

```text
Formica.ApiService/Warehouse/Persistence/Configurations/WarehouseFoundation/
```

This keeps the first milestone aligned with the broader Formica Warehouse module without creating a separate project yet.

## Identity and Lifecycle

### Stable Identity

Every persisted reference entity must have a stable technical identity that does not change when display attributes are edited.

Reference entities with stable identity:

- Warehouse
- Zone
- Storage Location
- SKU
- SKU Barcode
- Unit of Measure, if persisted as a reference entity

Display names, codes, addresses, and descriptions are editable attributes subject to validation. They must not be used as the only stable identity for future workflows.

### Lifecycle State

Warehouse Foundation reference data uses this lifecycle:

```text
Create → Update → Deactivate → Reactivate
```

Physical deletion is not the normal user workflow.

If physical deletion is introduced later, it must be limited to records that have never been used by operational workflows or referenced by other records.

### Active/Inactive Semantics

- Active records are available for ordinary operational selection.
- Inactive records remain visible where historical or setup context requires it.
- Reactivation is allowed only when all uniqueness and validation rules still pass.
- Deactivation must not change stable identity.

## Entities

## Warehouse

### Meaning

A physical or logical warehouse where stock is stored, controlled, received, moved, counted, picked, packed, or shipped.

### Suggested Attributes

| Attribute | Required | Notes |
|-----------|----------|-------|
| Id | Yes | Stable technical identity. |
| Code | Yes | Unique warehouse code. |
| Name | Yes | Human-readable display name. |
| Description | No | Optional user-facing description. |
| IsActive | Yes | Lifecycle flag. |
| CreatedAtUtc | Yes | Audit timestamp. |
| UpdatedAtUtc | Yes | Audit timestamp. |

### Relationships

- Warehouse has many Zones.
- Warehouse has many Storage Locations through Zones.
- Warehouse uses the default Warehouse Foundation Location Address Rules policy for this milestone.

### Invariants

- Warehouse Code is required.
- Warehouse Code is unique across Formica Warehouse.
- Warehouse Name is required.
- Warehouse identity remains stable when Code or Name changes.
- Warehouse can be deactivated and reactivated.

## Zone

### Meaning

A named area within a warehouse used to organize storage locations and future operational workflows.

### Suggested Attributes

| Attribute | Required | Notes |
|-----------|----------|-------|
| Id | Yes | Stable technical identity. |
| WarehouseId | Yes | Parent warehouse. |
| Code | Yes | Unique within a warehouse. |
| Name | Yes | Human-readable display name. |
| Purpose | Yes | Controlled value. |
| Description | No | Optional user-facing description. |
| IsActive | Yes | Lifecycle flag. |
| CreatedAtUtc | Yes | Audit timestamp. |
| UpdatedAtUtc | Yes | Audit timestamp. |

### Initial Purpose Values

- Storage
- Receiving
- Shipping
- Picking
- Packing
- Staging
- QualityControl
- Quarantine
- Other

### Relationships

- Zone belongs to one Warehouse.
- Zone has many Storage Locations.

### Invariants

- Zone Code is required.
- Zone Code is unique within its Warehouse.
- Zone Name is required.
- Zone Purpose is required.
- Zone identity remains stable when Code, Name, or Purpose changes.
- Zone can be deactivated and reactivated.

## Storage Location

### Meaning

An addressable warehouse place where stock can be stored or processed.

### Suggested Attributes

| Attribute | Required | Notes |
|-----------|----------|-------|
| Id | Yes | Stable technical identity. |
| WarehouseId | Yes | Denormalized parent warehouse reference for uniqueness and queries. |
| ZoneId | Yes | Parent zone. |
| Address | Yes | Human-readable/scannable location address. |
| Name | No | Optional display label. |
| Purpose | Yes | Controlled value. |
| Capacity | No | Optional capacity value object. |
| IsActive | Yes | Lifecycle flag. |
| CreatedAtUtc | Yes | Audit timestamp. |
| UpdatedAtUtc | Yes | Audit timestamp. |

### Initial Purpose Values

- Storage
- Picking
- Staging
- Receiving
- Shipping
- Packing
- QualityControl
- Quarantine
- Other

### Relationships

- Storage Location belongs to one Zone.
- Storage Location belongs to one Warehouse through Zone.
- Storage Location address is validated by Location Address Rules for its Warehouse.

### Invariants

- Storage Location Address is required.
- Storage Location Address is unique within its Warehouse.
- Storage Location Address must satisfy the default Warehouse Foundation Location Address Rules.
- Storage Location Purpose is required.
- Storage Location identity remains stable when Address, Name, Purpose, or Capacity changes.
- Storage Location can be deactivated and reactivated.
- Capacity values, when provided, must be non-negative.

### Notes

`WarehouseId` may be stored directly on Storage Location to enforce uniqueness of `WarehouseId + Address` efficiently and to simplify common queries. The relationship between Zone and Warehouse must still remain consistent.

## Location Address Rules

### Meaning

Default Warehouse Foundation validation rules for user-defined storage location addresses.

For this milestone, Location Address Rules are a foundation-level policy, not a Warehouse-owned child entity. Warehouse-specific address rule overrides are deferred to a future explicit feature decision.

### Suggested Attributes

| Attribute | Required | Notes |
|-----------|----------|-------|
| Id | Yes | Stable technical identity. |
| Code | Yes | Stable rules code, such as `DEFAULT`; unique for the foundation policy. |
| MaxLength | Yes | Maximum accepted address length. |
| AllowedPattern | No | Optional validation pattern. |
| NormalizeToUppercase | Yes | Whether address values are normalized to uppercase. |
| TrimWhitespace | Yes | Whether leading/trailing whitespace is removed. |
| ZonePrefixRequired | No | Optional rule for zone-related prefix conventions. |
| CreatedAtUtc | Yes | Audit timestamp. |
| UpdatedAtUtc | Yes | Audit timestamp. |

### Invariants

- Address rules must be deterministic and user-explainable.
- Address validation must not require a fixed aisle/rack/level/position model.
- If normalization is enabled, uniqueness must be evaluated after normalization.
- Existing addresses must remain valid or be handled by an explicit future migration/validation workflow if rules change.

### Notes

Location Address Rules are not an address generator. Generated ranges, topology-aware address creation, and warehouse-specific overrides are deferred.

### Future Default Policy with Scoped Override

Warehouse Foundation currently models Location Address Rules as a foundation-level default policy, not as a Warehouse-owned child entity.

Future warehouse-specific Location Address Rules may follow a **Default Policy with Scoped Override** model: a foundation-level default rule applies to all warehouses unless a warehouse-specific override exists. Warehouse-specific overrides are intentionally deferred and must be introduced by a separate feature/specification.

For the current milestone:

- Location Address Rules do not have `WarehouseId`;
- Warehouse-specific address rule overrides are not implemented;
- Storage Location address validation uses the foundation-level default policy.

## Storage Location Capacity

### Meaning

Optional capacity data for a storage location, intended for future use by putaway, slotting, and optimization features.

### Suggested Attributes

| Attribute | Required | Notes |
|-----------|----------|-------|
| MaxWeight | No | Optional non-negative value. |
| Volume | No | Optional non-negative value. |
| Height | No | Optional non-negative value. |
| Width | No | Optional non-negative value. |
| Depth | No | Optional non-negative value. |

### Invariants

- Capacity is optional.
- Each provided numeric value must be non-negative.
- Warehouse Foundation must not calculate capacity consumption.
- Warehouse Foundation must not calculate slotting suitability.
- Warehouse Foundation must not recommend storage locations based on capacity.

### Notes

Capacity may be modeled as an owned/value-object-style concept rather than a separate independent entity.

## SKU

### Meaning

The primary warehouse-operational identity of stock.

SKU in Warehouse Foundation is intentionally basic. It is not a full Product Catalog.

### Suggested Attributes

| Attribute | Required | Notes |
|-----------|----------|-------|
| Id | Yes | Stable technical identity. |
| Code | Yes | Globally unique within Formica Warehouse. |
| Name | Yes | Human-readable display name. |
| BaseUnitOfMeasureId | Yes | Base unit used by warehouse operations. |
| Description | No | Optional user-facing description. |
| IsActive | Yes | Lifecycle flag. |
| CreatedAtUtc | Yes | Audit timestamp. |
| UpdatedAtUtc | Yes | Audit timestamp. |

### Relationships

- SKU has one Base Unit of Measure.
- SKU has zero or many SKU Barcodes.

### Invariants

- SKU Code is required.
- SKU Code is unique across all SKUs.
- SKU Name is required.
- Base Unit of Measure is required.
- SKU identity remains stable when Code, Name, Base Unit of Measure, or barcode list changes.
- SKU can be deactivated and reactivated.

### Notes

Product and SKU are distinct concepts. This feature models only basic SKUs. Full Product Catalog, Product/SKU hierarchy, Composite SKU/Kit behavior, supplier-specific packaging, and advanced unit conversion are deferred.

## SKU Barcode

### Meaning

A barcode value associated with a SKU.

A SKU may have multiple barcode values, reflecting practical catalog and 1C-style nomenclature scenarios.

### Suggested Attributes

| Attribute | Required | Notes |
|-----------|----------|-------|
| Id | Yes | Stable technical identity if persisted as an entity. |
| SkuId | Yes | Parent SKU. |
| Value | Yes | Barcode value. |
| Description | No | Optional note, such as package or source. |
| IsActive | Yes | Lifecycle flag, if barcode lifecycle is tracked independently. |
| CreatedAtUtc | Yes | Audit timestamp. |
| UpdatedAtUtc | Yes | Audit timestamp. |

### Relationships

- SKU Barcode belongs to one SKU.

### Invariants

- Barcode Value is required.
- Barcode Value is unique across all SKUs unless a future feature explicitly defines barcode reuse or aliasing rules.
- A SKU may have multiple Barcode Values.
- A Barcode Value must not point to multiple active SKUs.

### Notes

Barcode scanning workflow is out of scope. This milestone stores barcode references only.

## Unit of Measure

### Meaning

A quantity unit used for stock operations.

### Suggested Attributes

| Attribute | Required | Notes |
|-----------|----------|-------|
| Id | Yes | Stable technical identity if persisted. |
| Code | Yes | Unique unit code. |
| Name | Yes | Human-readable display name. |
| Symbol | No | Optional short display symbol. |
| IsActive | Yes | Lifecycle flag if maintained by users. |

### Initial Considerations

The first milestone may use a small seed/reference set for common units, such as:

- piece
- box
- kilogram
- liter
- meter
- pallet
- package

### Invariants

- Unit Code is required.
- Unit Code is unique.
- Unit Name is required.
- SKU requires a Base Unit of Measure.

### Notes

Advanced unit conversion, alternative units, supplier packaging, and package hierarchy are out of scope for Warehouse Foundation.

## Read Models

## Warehouse Layout

### Meaning

A read-oriented view that presents the configured warehouse as:

```text
Warehouse → Zone → Storage Location
```

Warehouse Layout is not a separate domain entity and should not be persisted as its own aggregate.

### Suggested Shape

```text
WarehouseLayout
├── Warehouse summary
├── Zones
│   └── Storage Locations
└── Setup readiness indicators
```

### Included Data

- Warehouse code, name, active/inactive state
- Zone code, name, purpose, active/inactive state
- Storage Location address, name, purpose, active/inactive state
- Optional setup readiness indicators

### Setup Readiness Indicators

Setup readiness indicators are read-only signals that help a user understand whether the warehouse foundation is sufficiently configured for future warehouse workflows.

Possible indicators include:

- whether the warehouse has at least one zone;
- whether the warehouse has at least one storage location;
- whether the warehouse has at least one active storage location;
- whether location address rules are configured;
- whether zones exist without storage locations;
- whether inactive zones or storage locations require attention.

These indicators do not introduce a new domain entity or workflow state. They are derived from current Warehouse Foundation data.

### Invariants

- Warehouse Layout must reflect current persisted Warehouse Foundation data.
- Inactive records must be visible when relevant to setup verification.
- A warehouse with no zones or storage locations must be represented clearly as incomplete setup.
- Setup readiness indicators must be derived from persisted Warehouse Foundation data and must not be persisted as a separate aggregate.

## Relationship Summary

```text
Warehouse
└── Zones
    └── StorageLocations

Default LocationAddressRules apply at the Warehouse Foundation level.

SKU
├── BaseUnitOfMeasure
└── SkuBarcodes
```

Storage Location belongs to a Zone and is scoped to a Warehouse for uniqueness of Location Address.

SKU is independent from Warehouse in this milestone. Future inventory features will connect SKU, Warehouse, Storage Location, and stock state.

## Uniqueness Rules

| Rule | Scope |
|------|-------|
| Warehouse.Code | Global within Formica Warehouse |
| Zone.Code | Unique within Warehouse |
| StorageLocation.Address | Unique within Warehouse after normalization |
| SKU.Code | Global within Formica Warehouse |
| SkuBarcode.Value | Global across SKUs |
| UnitOfMeasure.Code | Global within Formica Warehouse |

## Validation Rules

### General

- Required text values must not be blank.
- Codes and addresses should be trimmed before validation.
- Normalization rules must be deterministic.
- Reactivation must re-run uniqueness and validation rules.

### Lifecycle

- Update preserves stable identity.
- Deactivation preserves stable identity.
- Reactivation preserves stable identity.
- Physical deletion is not required for normal workflows.

### Addressing

- Location Address is required.
- Location Address must satisfy the default Warehouse Foundation Location Address Rules.
- Location Address uniqueness must be evaluated within Warehouse scope after normalization.

### Capacity

- Capacity attributes are optional.
- Provided capacity numeric values must be non-negative.
- No capacity consumption calculation is performed.

### Barcode

- A SKU may have multiple barcodes.
- Barcode value is required when a barcode is provided.
- Barcode value must be unique across SKUs.

## Aggregate / Ownership Notes

This milestone should avoid heavy aggregate ceremony.

Suggested ownership boundaries:

- Warehouse owns its setup relationship to Zones conceptually.
- Location Address Rules are a foundation-level policy in this milestone, not owned by Warehouse.
- Zone owns its relationship to Storage Locations conceptually.
- SKU owns its Barcode Values conceptually.
- Unit of Measure may be a simple reference entity or controlled reference set.

Implementation may use direct EF Core relationships and feature-level validation rather than complex aggregate roots if that keeps the first milestone simpler.

Business invariants must be enforced in feature/application/domain behavior and backed by database constraints where appropriate.

## Database Constraint Expectations

PostgreSQL/EF Core persistence should support at least these database-level constraints:

- unique index on Warehouse Code;
- unique index on Zone WarehouseId + Code;
- unique index on Storage Location WarehouseId + normalized Address;
- unique index on SKU Code;
- unique index on SKU Barcode Value;
- unique index on Unit of Measure Code, if persisted;
- foreign key from Zone to Warehouse;
- foreign key from Storage Location to Zone;
- foreign key from Storage Location to Warehouse, if WarehouseId is stored directly;
- foreign key from SKU to Unit of Measure, if Unit of Measure is persisted;
- foreign key from SKU Barcode to SKU.

Database constraints do not replace user-friendly validation and error messages.

## Resolved Open Questions for Contracts

### OQ-001: Endpoint grouping

Use separate capability-oriented endpoint groups instead of a single combined management endpoint.

Expected contract groups:

- warehouses;
- zones;
- storage locations;
- SKUs;
- warehouse layout.

Rationale: this keeps contracts aligned with vertical slices and avoids a broad CRUD-style management endpoint.

### OQ-002: Updating codes and addresses

Warehouse Foundation allows updating codes and addresses while preserving stable identity.

Allowed updates include:

- Warehouse Code, if the new code is globally unique;
- Zone Code, if the new code is unique within the Warehouse;
- Storage Location Address, if the new address satisfies Location Address Rules and is unique within the Warehouse after normalization;
- SKU Code, if the new code is globally unique.

Rationale: full operational history does not exist in this milestone yet, so forbidding code/address edits would be premature. Future workflow features may introduce stricter change policies if needed.

### OQ-003: Inactive records and response models

List endpoints return active records by default and support an explicit `includeInactive=true` filter.

All list, detail, and layout response models must include `IsActive` where the underlying model has lifecycle state. This allows the UI to show active and inactive records explicitly, even when a response includes both.

Warehouse Layout must include activity state for Warehouse, Zones, and Storage Locations.

Recommended behavior:

- ordinary list endpoints default to active records only;
- ordinary list endpoints support `includeInactive=true`;
- detail endpoints return the requested record if it exists, including its `IsActive` value;
- Warehouse Layout may include inactive records by default or through an explicit parameter because it is used for setup verification.

Rationale: filtering controls which records are included, while `IsActive` communicates the lifecycle state of each returned record.

### OQ-004: Location Address Rules management

Ensure default Location Address Rules exist when Warehouse Foundation is initialized by the first warehouse creation path.

Initial defaults:

- MaxLength: 50;
- TrimWhitespace: true;
- NormalizeToUppercase: true;
- AllowedPattern: simple warehouse-safe address pattern;
- ZonePrefixRequired: false.

Full UI management and warehouse-specific ownership for Location Address Rules are deferred.

Rationale: address rules are needed to keep Storage Location addresses consistent, but building a full rule-management UI would unnecessarily expand Warehouse Foundation.

### OQ-005: Unit of Measure management

Use a seeded read-only Unit of Measure set for the first milestone.

Initial seed candidates:

- piece;
- box;
- kilogram;
- liter;
- meter;
- pallet;
- package.

User-managed units, unit conversion, alternative units, supplier packaging, and package hierarchy are deferred.

Rationale: Unit of Measure is required by SKU, but full UoM management would pull Product Catalog and packaging concerns into Warehouse Foundation too early.

## Deferred Model Decisions

The following model decisions are intentionally deferred:

- whether SKU Barcode has independent lifecycle or follows SKU lifecycle;
- exact numeric types and units for capacity values;
- exact location address pattern syntax;
- whether normalized address is stored as a separate field;
- whether future Inventory uses the same WarehouseDbContext or introduces a separate context;
- physical extraction of Warehouse into a separate project.
