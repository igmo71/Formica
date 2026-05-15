# API Contracts: Warehouse Foundation

**Feature**: `001-warehouse-foundation`  
**Spec**: `../spec.md`  
**Plan**: `../plan.md`  
**Research**: `../research.md`  
**Data Model**: `../data-model.md`  
**Date**: 2026-05-07

## Purpose

This document defines the API-facing contracts for Warehouse Foundation.

It describes route groups, request shapes, response shapes, lifecycle actions, filters, and expected error semantics. It is not implementation code, generated OpenAPI output, database schema, or task breakdown.

## General Contract Rules

### Base Route

All Warehouse Foundation endpoints use this base route:

```text
/api/warehouse-foundation
```

### Identity

All persisted resources use stable technical identifiers in API routes and responses.

Codes, names, addresses, and descriptions are editable attributes and must not be treated as stable identity.

### Lifecycle

Reference resources use this lifecycle:

```text
Create → Update → Deactivate → Reactivate
```

Physical deletion is not a normal Warehouse Foundation API operation.

Deactivate/reactivate endpoints use `POST` because they represent explicit lifecycle commands, not generic partial updates to the `isActive` field.

### Activity State

All list, detail, and layout responses must include `isActive` where the underlying model has lifecycle state.

List endpoints return active records by default and support:

```text
includeInactive=true
```

### Timestamps

Responses may include audit timestamps when useful:

```text
createdAtUtc
updatedAtUtc
```

Timestamps must represent UTC values.

### Error Shape

API errors should use a consistent problem response shape compatible with ASP.NET Core `ProblemDetails`.

Suggested fields:

```json
{
  "type": "https://formica/problems/validation-error",
  "title": "Validation error",
  "status": 400,
  "detail": "One or more validation errors occurred.",
  "errors": {
    "code": ["Warehouse code is required."]
  }
}
```

Common status codes:

| Status | Meaning |
|--------|---------|
| 400 | Validation error or invalid request. |
| 404 | Requested resource does not exist. |
| 409 | Uniqueness conflict or lifecycle conflict. |
| 500 | Unexpected server error. |

### Naming

JSON uses camelCase.

Domain terms used in contracts:

- warehouse
- zone
- storageLocation
- locationAddressRules
- sku
- skuBarcode
- unitOfMeasure
- warehouseLayout

## Warehouses

### List Warehouses

```http
GET /api/warehouse-foundation/warehouses?includeInactive=false
```

Default behavior:

- returns active warehouses only;
- `includeInactive=true` returns active and inactive warehouses.

Response:

```json
[
  {
    "id": "00000000-0000-0000-0000-000000000000",
    "code": "WH-001",
    "name": "Main Warehouse",
    "description": "Primary warehouse",
    "isActive": true,
    "createdAtUtc": "2026-05-07T10:00:00Z",
    "updatedAtUtc": "2026-05-07T10:00:00Z"
  }
]
```

### Get Warehouse

```http
GET /api/warehouse-foundation/warehouses/{warehouseId}
```

Response:

```json
{
  "id": "00000000-0000-0000-0000-000000000000",
  "code": "WH-001",
  "name": "Main Warehouse",
  "description": "Primary warehouse",
  "isActive": true,
  "createdAtUtc": "2026-05-07T10:00:00Z",
  "updatedAtUtc": "2026-05-07T10:00:00Z"
}
```

### Create Warehouse

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

Behavior:

- creates a Warehouse;
- ensures default Warehouse Foundation Location Address Rules exist;
- rejects duplicate warehouse code.

Response:

```http
201 Created
Location: /api/warehouse-foundation/warehouses/{warehouseId}
```

Body:

```json
{
  "id": "00000000-0000-0000-0000-000000000000",
  "code": "WH-001",
  "name": "Main Warehouse",
  "description": "Primary warehouse",
  "isActive": true,
  "createdAtUtc": "2026-05-07T10:00:00Z",
  "updatedAtUtc": "2026-05-07T10:00:00Z"
}
```

### Update Warehouse

```http
PUT /api/warehouse-foundation/warehouses/{warehouseId}
```

Request:

```json
{
  "code": "WH-001",
  "name": "Main Warehouse",
  "description": "Updated description"
}
```

Behavior:

- preserves stable identity;
- allows changing code if the new code is globally unique;
- does not change active/inactive state.

### Deactivate Warehouse

```http
POST /api/warehouse-foundation/warehouses/{warehouseId}/deactivate
```

Behavior:

- marks the Warehouse inactive;
- preserves stable identity;
- does not physically delete the Warehouse.

### Reactivate Warehouse

```http
POST /api/warehouse-foundation/warehouses/{warehouseId}/reactivate
```

Behavior:

- marks the Warehouse active if uniqueness and validation rules still pass.

## Zones

### List Zones

```http
GET /api/warehouse-foundation/zones?warehouseId={warehouseId}&includeInactive=false
```

Default behavior:

- `warehouseId` is required in this milestone;
- returns active zones for the selected Warehouse only;
- `includeInactive=true` returns active and inactive zones for the selected Warehouse.

Response:

```json
[
  {
    "id": "00000000-0000-0000-0000-000000000000",
    "warehouseId": "00000000-0000-0000-0000-000000000000",
    "code": "STORAGE",
    "name": "Main Storage",
    "purpose": "Storage",
    "description": "Main storage area",
    "isActive": true,
    "createdAtUtc": "2026-05-07T10:00:00Z",
    "updatedAtUtc": "2026-05-07T10:00:00Z"
  }
]
```

### Get Zone

```http
GET /api/warehouse-foundation/zones/{zoneId}
```

### Create Zone

```http
POST /api/warehouse-foundation/zones
```

Request:

```json
{
  "warehouseId": "00000000-0000-0000-0000-000000000000",
  "code": "STORAGE",
  "name": "Main Storage",
  "purpose": "Storage",
  "description": "Main storage area"
}
```

Behavior:

- rejects duplicate zone code within the same Warehouse;
- allows the same zone code in different Warehouses.

### Update Zone

```http
PUT /api/warehouse-foundation/zones/{zoneId}
```

Request:

```json
{
  "code": "STORAGE",
  "name": "Main Storage",
  "purpose": "Storage",
  "description": "Updated description"
}
```

Behavior:

- preserves stable identity;
- allows changing code if the new code is unique within the same Warehouse;
- does not move the Zone to another Warehouse.

### Deactivate Zone

```http
POST /api/warehouse-foundation/zones/{zoneId}/deactivate
```

Behavior:

- marks the Zone inactive;
- preserves stable identity and Warehouse assignment;
- does not physically delete the Zone.

### Reactivate Zone

```http
POST /api/warehouse-foundation/zones/{zoneId}/reactivate
```

Behavior:

- marks the Zone active if uniqueness and validation rules still pass;
- preserves stable identity and Warehouse assignment.

## Storage Locations

### List Storage Locations

```http
GET /api/warehouse-foundation/storage-locations?warehouseId={warehouseId}&zoneId={zoneId}&includeInactive=false
```

Default behavior:

- returns active storage locations only;
- supports filtering by Warehouse and/or Zone.

Response:

```json
[
  {
    "id": "00000000-0000-0000-0000-000000000000",
    "warehouseId": "00000000-0000-0000-0000-000000000000",
    "zoneId": "00000000-0000-0000-0000-000000000000",
    "address": "A-01-01",
    "name": "Aisle A location 01",
    "purpose": "Storage",
    "capacity": {
      "maxWeight": 1000.0,
      "volume": 2.5,
      "height": 1.0,
      "width": 1.0,
      "depth": 2.5
    },
    "isActive": true,
    "createdAtUtc": "2026-05-07T10:00:00Z",
    "updatedAtUtc": "2026-05-07T10:00:00Z"
  }
]
```

### Get Storage Location

```http
GET /api/warehouse-foundation/storage-locations/{storageLocationId}
```

### Create Storage Location

```http
POST /api/warehouse-foundation/storage-locations
```

Request:

```json
{
  "warehouseId": "00000000-0000-0000-0000-000000000000",
  "zoneId": "00000000-0000-0000-0000-000000000000",
  "address": "A-01-01",
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

Behavior:

- validates address against the default Warehouse Foundation Location Address Rules;
- normalizes address according to the default Warehouse Foundation Location Address Rules;
- rejects duplicate normalized address within the same Warehouse;
- validates that `zoneId` belongs to the same Warehouse as `warehouseId`.

### Update Storage Location

```http
PUT /api/warehouse-foundation/storage-locations/{storageLocationId}
```

Request:

```json
{
  "zoneId": "00000000-0000-0000-0000-000000000000",
  "address": "A-01-02",
  "name": "Aisle A location 02",
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

Behavior:

- preserves stable identity;
- allows changing address if the new address is valid and unique within the Warehouse after normalization;
- allows moving to another Zone only within the same Warehouse;
- does not move the Storage Location to another Warehouse.

### Deactivate Storage Location

```http
POST /api/warehouse-foundation/storage-locations/{storageLocationId}/deactivate
```

### Reactivate Storage Location

```http
POST /api/warehouse-foundation/storage-locations/{storageLocationId}/reactivate
```

## SKUs

### List SKUs

```http
GET /api/warehouse-foundation/skus?includeInactive=false
```

Default behavior:

- returns active SKUs only;
- `includeInactive=true` returns active and inactive SKUs.

Response:

```json
[
  {
    "id": "00000000-0000-0000-0000-000000000000",
    "code": "SKU-001",
    "name": "Sample SKU",
    "baseUnitOfMeasureId": "00000000-0000-0000-0000-000000000000",
    "baseUnitOfMeasureCode": "piece",
    "barcodes": [
      {
        "id": "00000000-0000-0000-0000-000000000000",
        "value": "4600000000011",
        "description": "Default barcode",
        "isActive": true
      }
    ],
    "description": "Sample SKU description",
    "isActive": true,
    "createdAtUtc": "2026-05-07T10:00:00Z",
    "updatedAtUtc": "2026-05-07T10:00:00Z"
  }
]
```

### Get SKU

```http
GET /api/warehouse-foundation/skus/{skuId}
```

### Create SKU

```http
POST /api/warehouse-foundation/skus
```

Request:

```json
{
  "code": "SKU-001",
  "name": "Sample SKU",
  "baseUnitOfMeasureId": "00000000-0000-0000-0000-000000000000",
  "barcodes": [
    {
      "value": "4600000000011",
      "description": "Default barcode"
    }
  ],
  "description": "Sample SKU description"
}
```

Behavior:

- rejects duplicate SKU code;
- allows multiple barcode values for one SKU;
- rejects duplicate barcode values across SKUs.

### Update SKU

```http
PUT /api/warehouse-foundation/skus/{skuId}
```

Request:

```json
{
  "code": "SKU-001",
  "name": "Sample SKU",
  "baseUnitOfMeasureId": "00000000-0000-0000-0000-000000000000",
  "barcodes": [
    {
      "id": "00000000-0000-0000-0000-000000000000",
      "value": "4600000000011",
      "description": "Default barcode",
      "isActive": true
    }
  ],
  "description": "Updated SKU description"
}
```

Behavior:

- preserves stable identity;
- allows changing SKU code if the new code is unique;
- treats barcode collection as the desired current barcode set for the SKU;
- rejects duplicate barcode values across SKUs.

### Deactivate SKU

```http
POST /api/warehouse-foundation/skus/{skuId}/deactivate
```

### Reactivate SKU

```http
POST /api/warehouse-foundation/skus/{skuId}/reactivate
```

## Unit of Measure

Warehouse Foundation uses a seeded read-only Unit of Measure set for the first milestone.

### List Units of Measure

```http
GET /api/warehouse-foundation/units-of-measure?includeInactive=false
```

Response:

```json
[
  {
    "id": "00000000-0000-0000-0000-000000000000",
    "code": "piece",
    "name": "Piece",
    "symbol": "pcs",
    "isActive": true
  }
]
```

No create/update/deactivate/reactivate endpoints are required for Unit of Measure in this milestone.

## Warehouse Layout

### Get Warehouse Layout

```http
GET /api/warehouse-foundation/warehouses/{warehouseId}/layout?includeInactive=true
```

Default behavior:

- returns a read-oriented view of Warehouse → Zones → Storage Locations;
- includes `isActive` for Warehouse, Zones, and Storage Locations;
- may include inactive records by default or through `includeInactive=true` because this view is used for setup verification.

Response:

```json
{
  "warehouse": {
    "id": "00000000-0000-0000-0000-000000000000",
    "code": "WH-001",
    "name": "Main Warehouse",
    "isActive": true
  },
  "zones": [
    {
      "id": "00000000-0000-0000-0000-000000000000",
      "code": "STORAGE",
      "name": "Main Storage",
      "purpose": "Storage",
      "isActive": true,
      "storageLocations": [
        {
          "id": "00000000-0000-0000-0000-000000000000",
          "address": "A-01-01",
          "name": "Aisle A location 01",
          "purpose": "Storage",
          "isActive": true
        }
      ]
    }
  ],
  "setupReadiness": {
    "status": "Ready",
    "hasZones": true,
    "hasStorageLocations": true,
    "hasActiveStorageLocations": true,
    "hasLocationAddressRules": true,
    "warnings": []
  }
}
```

Possible `setupReadiness.status` values:

- `NotStarted`
- `Incomplete`
- `Ready`
- `HasWarnings`

## Location Address Rules

Location Address Rules are the default Warehouse Foundation address policy for this milestone, not a Warehouse-owned child resource.

Full UI/API management and Warehouse-specific overrides for Location Address Rules are deferred.

The `allowedPattern` value shown below is a suggested default for the first implementation, not a finalized long-term address-pattern language.

### Get Location Address Rules

```http
GET /api/warehouse-foundation/location-address-rules/default
```

Response:

```json
{
  "maxLength": 50,
  "allowedPattern": "^[A-Z0-9][A-Z0-9\\-_.]*$",
  "normalizeToUppercase": true,
  "trimWhitespace": true,
  "zonePrefixRequired": false
}
```

No create/update/delete endpoint is required for Location Address Rules in this milestone.

## Controlled Values

### Zone Purpose

Allowed values:

- Storage
- Receiving
- Shipping
- Picking
- Packing
- Staging
- QualityControl
- Quarantine
- Other

### Storage Location Purpose

Allowed values:

- Storage
- Picking
- Staging
- Receiving
- Shipping
- Packing
- QualityControl
- Quarantine
- Other

## Contract-Level Validation Summary

| Area | Validation |
|------|------------|
| Warehouse | Code required, name required, code globally unique. |
| Zone | WarehouseId required, code required, name required, purpose required, code unique within Warehouse. |
| Storage Location | WarehouseId required, ZoneId required, address required, purpose required, address valid and unique within Warehouse after normalization. |
| SKU | Code required, name required, baseUnitOfMeasureId required, code globally unique. |
| SKU Barcode | Value required when provided, value globally unique across SKUs. |
| Capacity | Optional; numeric values must be non-negative when provided. |
| Lifecycle | Reactivation reruns validation and uniqueness checks. |

## Deferred Contract Decisions

The following contract decisions are deferred:

- pagination and sorting conventions;
- search/filter syntax beyond `includeInactive`, `warehouseId`, and `zoneId`;
- PATCH versus PUT partial update semantics for ordinary editable attributes;
- separate barcode-management endpoints;
- create/update APIs for Unit of Measure;
- create/update APIs for Location Address Rules;
- bulk creation of zones or storage locations;
- import/export contracts;
- mobile/scanner-specific contracts.
