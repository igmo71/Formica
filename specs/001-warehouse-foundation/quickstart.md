# Quickstart: Warehouse Foundation

**Feature**: `001-warehouse-foundation`  
**Spec**: `specs/001-warehouse-foundation/spec.md`  
**Plan**: `specs/001-warehouse-foundation/plan.md`  
**Research**: `specs/001-warehouse-foundation/research.md`  
**Data Model**: `specs/001-warehouse-foundation/data-model.md`  
**Contracts**: `specs/001-warehouse-foundation/contracts/api-contracts.md`  
**Date**: 2026-05-07

## Purpose

This quickstart defines the expected validation walkthrough for Warehouse Foundation.

It is used to verify that the feature satisfies the specification before executable implementation tasks are considered complete.

This document is not a task list and not implementation code.

## Preconditions

The application should be runnable through the existing Aspire solution.

Expected solution shape:

```text
Formica.slnx
├── Formica.AppHost/
├── Formica.ServiceDefaults/
├── Formica.ApiService/
├── Formica.WebApp/
├── Formica.Web/
└── Formica.Tests/
```

Warehouse Foundation expects PostgreSQL to be orchestrated by Aspire once persistence is implemented.

Manual UI validation should use `Formica.WebApp`, the MudBlazor-based target Blazor UI. `Formica.Web` is a temporary Bootstrap baseline/migration source until required behavior is moved and the project is removed.

Base API route:

```text
/api/warehouse-foundation
```

## Validation Goals

The full Warehouse Foundation quickstart verifies that a user can:

1. create a warehouse;
2. verify default Location Address Rules;
3. create warehouse zones;
4. create storage locations with valid addresses;
5. prevent duplicate warehouse setup data;
6. create basic SKUs with multiple barcodes;
7. prevent duplicate SKU and barcode values;
8. view warehouse layout;
9. deactivate and reactivate foundation records;
10. verify that response models include `isActive` where lifecycle state exists.

M1 validation is intentionally smaller: it covers the `Formica.WebApp` target UI foundation plus Warehouse and Zone management. Storage Locations, SKUs, Warehouse Layout, and full lifecycle consistency are validated in later accepted milestones.

## Scenario 1: Create a Warehouse

### Action

Create a Warehouse:

```http
POST /api/warehouse-foundation/warehouses
```

Request:

```json
{
  "code": "WH-001",
  "name": "Main Warehouse",
  "description": "Primary warehouse"
}
```

### Expected Result

- Response is `201 Created`.
- Response body includes stable `id`.
- Response body includes `isActive: true`.
- Warehouse code is `WH-001`.
- Location header points to the created warehouse resource.

## Scenario 2: Verify Default Location Address Rules

### Action

Get the default Warehouse Foundation Location Address Rules:

```http
GET /api/warehouse-foundation/location-address-rules/default
```

### Expected Result

Response includes default rules:

```json
{
  "maxLength": 50,
  "normalizeToUppercase": true,
  "trimWhitespace": true,
  "zonePrefixRequired": false
}
```

The exact `allowedPattern` may be implementation-defined but must be deterministic and warehouse-safe.

## Scenario 3: Prevent Duplicate Warehouse Code

### Action

Attempt to create another Warehouse with the same code:

```http
POST /api/warehouse-foundation/warehouses
```

Request:

```json
{
  "code": "WH-001",
  "name": "Duplicate Warehouse"
}
```

### Expected Result

- Response is `409 Conflict` or validation-style equivalent defined by implementation.
- Error explains that warehouse code must be unique.
- No duplicate Warehouse is created.

## Scenario 4: Create Zones

### Action

Create a Storage zone:

```http
POST /api/warehouse-foundation/zones
```

Request:

```json
{
  "warehouseId": "{warehouseId}",
  "code": "STORAGE",
  "name": "Main Storage",
  "purpose": "Storage",
  "description": "Main storage area"
}
```

Create a Picking zone:

```json
{
  "warehouseId": "{warehouseId}",
  "code": "PICKING",
  "name": "Picking Area",
  "purpose": "Picking",
  "description": "Picking area"
}
```

### Expected Result

- Both zones are created.
- Each zone has stable `id`.
- Each zone has `isActive: true`.
- Zone codes are unique within the Warehouse.

## Scenario 5: Prevent Duplicate Zone Code Within Warehouse

### Action

Attempt to create a second zone with code `STORAGE` in the same Warehouse.

### Expected Result

- Response is `409 Conflict` or validation-style equivalent.
- Error explains that zone code must be unique within a Warehouse.
- No duplicate zone is created.

## Scenario 6: Create Storage Locations

### Action

Create a Storage Location in the Storage zone:

```http
POST /api/warehouse-foundation/storage-locations
```

Request:

```json
{
  "warehouseId": "{warehouseId}",
  "zoneId": "{storageZoneId}",
  "address": "a-01-01",
  "name": "Aisle A location 01",
  "purpose": "Storage",
  "capacity": {
    "maxWeight": 1000.0,
    "volume": 2.5,
    "height": 1.0,
    "width": 1.0,
    "depth": 2.5
  }
}
```

### Expected Result

- Storage Location is created.
- Response includes stable `id`.
- Response includes `isActive: true`.
- Address is accepted if it satisfies Location Address Rules.
- If uppercase normalization is enabled, returned address may be normalized to `A-01-01`.

## Scenario 7: Prevent Duplicate Storage Location Address

### Action

Attempt to create another Storage Location in the same Warehouse with an equivalent address:

```json
{
  "warehouseId": "{warehouseId}",
  "zoneId": "{storageZoneId}",
  "address": " A-01-01 ",
  "name": "Duplicate location",
  "purpose": "Storage"
}
```

### Expected Result

- Response is `409 Conflict` or validation-style equivalent.
- Duplicate check is performed after normalization.
- No duplicate Storage Location is created.

## Scenario 8: Create SKU With Multiple Barcodes

### Action

List seeded Units of Measure:

```http
GET /api/warehouse-foundation/units-of-measure
```

Use the `piece` unit as the SKU base unit.

Create a SKU:

```http
POST /api/warehouse-foundation/skus
```

Request:

```json
{
  "code": "SKU-001",
  "name": "Sample SKU",
  "baseUnitOfMeasureId": "{pieceUnitOfMeasureId}",
  "barcodes": [
    {
      "value": "4600000000011",
      "description": "Default barcode"
    },
    {
      "value": "4600000000012",
      "description": "Alternative barcode"
    }
  ],
  "description": "Sample SKU description"
}
```

### Expected Result

- SKU is created.
- Response includes stable `id`.
- Response includes `isActive: true`.
- Response includes both barcode values.
- Each barcode has a stable identity if barcodes are persisted as entities.

## Scenario 9: Prevent Duplicate SKU Code

### Action

Attempt to create another SKU with code `SKU-001`.

### Expected Result

- Response is `409 Conflict` or validation-style equivalent.
- Error explains that SKU code must be unique.
- No duplicate SKU is created.

## Scenario 10: Prevent Duplicate Barcode Across SKUs

### Action

Attempt to create another SKU with barcode `4600000000011`.

### Expected Result

- Response is `409 Conflict` or validation-style equivalent.
- Error explains that barcode value must be unique across SKUs.
- No duplicate barcode assignment is created.

## Scenario 11: View Warehouse Layout

### Action

Get Warehouse Layout:

```http
GET /api/warehouse-foundation/warehouses/{warehouseId}/layout?includeInactive=true
```

### Expected Result

Response represents:

```text
Warehouse → Zones → Storage Locations
```

Response includes:

- Warehouse code, name, and `isActive`;
- Zone code, name, purpose, and `isActive`;
- Storage Location address, name, purpose, and `isActive`;
- setup readiness indicators.

Expected setup readiness after previous scenarios:

```json
{
  "status": "Ready",
  "hasZones": true,
  "hasStorageLocations": true,
  "hasActiveStorageLocations": true,
  "hasLocationAddressRules": true
}
```

Warnings may be empty if setup is complete and no inactive/incomplete records require attention.

## Scenario 12: Deactivate and Reactivate Storage Location

### Action

Deactivate a Storage Location:

```http
POST /api/warehouse-foundation/storage-locations/{storageLocationId}/deactivate
```

Get Storage Locations with inactive records:

```http
GET /api/warehouse-foundation/storage-locations?warehouseId={warehouseId}&includeInactive=true
```

Reactivate the Storage Location:

```http
POST /api/warehouse-foundation/storage-locations/{storageLocationId}/reactivate
```

### Expected Result

- Deactivation does not delete the Storage Location.
- List response with `includeInactive=true` includes the inactive Storage Location.
- Response model includes `isActive: false` while inactive.
- Reactivation restores `isActive: true` if uniqueness and validation rules still pass.
- Stable identity remains unchanged.

## Scenario 13: Deactivate and Reactivate SKU

### Action

Deactivate SKU:

```http
POST /api/warehouse-foundation/skus/{skuId}/deactivate
```

List SKUs with inactive records:

```http
GET /api/warehouse-foundation/skus?includeInactive=true
```

Reactivate SKU:

```http
POST /api/warehouse-foundation/skus/{skuId}/reactivate
```

### Expected Result

- SKU is not physically deleted.
- SKU remains available in responses when `includeInactive=true`.
- Response model includes `isActive`.
- Reactivation preserves stable identity.

## Scenario 14: Update Codes and Addresses Without Changing Identity

### Action

Update Warehouse code or name:

```http
PUT /api/warehouse-foundation/warehouses/{warehouseId}
```

Update Zone code or name:

```http
PUT /api/warehouse-foundation/zones/{zoneId}
```

Update Storage Location address:

```http
PUT /api/warehouse-foundation/storage-locations/{storageLocationId}
```

Update SKU code or name:

```http
PUT /api/warehouse-foundation/skus/{skuId}
```

### Expected Result

- Stable `id` does not change.
- Updated code/address passes uniqueness validation.
- Updated address passes Location Address Rules.
- Response includes updated values and lifecycle state.

## Scenario 15: Verify Read-Only Unit of Measure Behavior

### Action

List Units of Measure:

```http
GET /api/warehouse-foundation/units-of-measure
```

Attempting create/update/delete Unit of Measure is not part of this milestone.

### Expected Result

- Seeded Units of Measure are available.
- API does not require user-managed Unit of Measure workflows.
- SKUs can reference seeded Units of Measure.

## Manual UI Validation

When the `Formica.WebApp` Blazor UI is implemented, it should allow the user to verify the same behavior visually:

- create Warehouse;
- create Zones;
- create Storage Locations;
- create SKUs with multiple barcodes;
- see active/inactive status;
- deactivate/reactivate records;
- view Warehouse Layout;
- see setup readiness indicators;
- understand validation and uniqueness errors.

UI labels may be localized later, but domain/API terminology remains based on the specification and glossary.

## Test Validation Notes

Automated tests should cover at least:

- warehouse code uniqueness;
- zone code uniqueness within Warehouse;
- storage location address uniqueness within Warehouse after normalization;
- SKU code uniqueness;
- multiple barcodes per SKU;
- barcode uniqueness across SKUs;
- active/inactive lifecycle;
- reactivation validation;
- `isActive` in list/detail/layout responses;
- default Location Address Rules availability;
- seeded read-only Unit of Measure availability;
- Warehouse Layout setup readiness indicators.

## Full Completion Criteria

Full Warehouse Foundation is ready for task completion when:

- all P1 user stories from `spec.md` pass through API behavior or UI behavior;
- Warehouse Layout shows configured Warehouse → Zones → Storage Locations;
- uniqueness rules are enforced by validation and database constraints;
- response models expose lifecycle state through `isActive`;
- default Location Address Rules exist for Warehouse Foundation;
- seeded Units of Measure are available for SKUs;
- no out-of-scope workflow is implemented accidentally.
