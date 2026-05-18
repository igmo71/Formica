# Quickstart: Warehouse Foundation

**Feature**: `001-warehouse-foundation`
**Spec**: `specs/001-warehouse-foundation/spec.md`
**Plan**: `specs/001-warehouse-foundation/plan.md`
**Contracts**: `specs/001-warehouse-foundation/contracts/api-contracts.md`

## Purpose

This quickstart defines the expected manual validation walkthrough. It is not a task list and not implementation code.

## Preconditions

The application runs through the Aspire solution:

```text
Formica.slnx
├── Formica.AppHost/
├── Formica.ServiceDefaults/
├── Formica.ApiService/
├── Formica.WebApp/
├── Formica.Web/        # temporary migration source only
└── Formica.Tests/
```

Manual UI validation uses `Formica.WebApp`, the MudBlazor-based target Blazor UI.

Base API route:

```text
/api/warehouse-foundation
```

## Milestone Checklists

### M1: Warehouse and Zone Management

- Create a warehouse.
- Verify duplicate warehouse code is rejected.
- Verify default Location Address Rules are available.
- Create multiple zones for a warehouse.
- Verify duplicate zone code in the same warehouse is rejected.
- Verify Warehouse and Zone management through `Formica.WebApp`.

### M2: Storage Locations

- Create a storage location inside a zone.
- Verify address normalization.
- Verify duplicate normalized address in the same warehouse is rejected.
- Verify deactivate/reactivate behavior for storage locations.

### M3: SKUs and Barcodes

- List seeded Units of Measure.
- Create a SKU with one or more barcode values.
- Verify duplicate SKU code is rejected.
- Verify duplicate barcode across SKUs is rejected.
- Verify deactivate/reactivate behavior for SKUs.

### M4: Layout and Lifecycle Consistency

- View Warehouse → Zones → Storage Locations layout.
- Verify setup readiness indicators.
- Verify `isActive` appears in list/detail/layout responses where lifecycle state exists.
- Verify stable IDs remain unchanged across update/deactivate/reactivate workflows.

## Representative API Walkthrough

### Scenario 1: Create a Warehouse

```http
POST /api/warehouse-foundation/warehouses
```

```json
{
  "code": "WH-001",
  "name": "Main Warehouse",
  "description": "Primary warehouse"
}
```

Expected:

- `201 Created`;
- stable `id`;
- `isActive: true`;
- `Location` header points to the created warehouse.

### Scenario 2: Verify Default Location Address Rules

```http
GET /api/warehouse-foundation/location-address-rules/default
```

Expected:

- default rules are returned;
- `maxLength` is available;
- trim/uppercase behavior is explicit;
- any `allowedPattern` is deterministic and warehouse-safe.

### Scenario 3: Create Zones

```http
POST /api/warehouse-foundation/zones
```

```json
{
  "warehouseId": "{warehouseId}",
  "code": "STORAGE",
  "name": "Main Storage",
  "purpose": "Storage",
  "description": "Main storage area"
}
```

Repeat with another code such as `PICKING`.

Expected:

- both zones are created;
- each zone has stable `id`;
- each zone has `isActive: true`;
- zone codes are unique within the warehouse.

### Scenario 4: Create Storage Location

```http
POST /api/warehouse-foundation/storage-locations
```

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

Expected:

- storage location is created;
- response includes stable `id` and `isActive: true`;
- address satisfies Location Address Rules;
- returned address may be normalized, for example `A-01-01`.

### Scenario 5: Create SKU With Barcodes

```http
GET /api/warehouse-foundation/units-of-measure
POST /api/warehouse-foundation/skus
```

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

Expected:

- SKU is created;
- response includes stable `id` and `isActive: true`;
- both barcode values are returned;
- barcode identities are stable if barcodes are persisted as entities.

## Concise Validation Scenarios

| Scenario | Action | Expected Result |
|----------|--------|-----------------|
| Duplicate warehouse code | Create another warehouse with `WH-001`. | Conflict or validation-style response; no duplicate warehouse. |
| Duplicate zone code | Create another `STORAGE` zone in the same warehouse. | Conflict or validation-style response; same code remains allowed in a different warehouse if implemented. |
| Duplicate location address | Create another location with equivalent normalized address such as ` A-01-01 `. | Conflict or validation-style response after normalization. |
| Duplicate SKU code | Create another SKU with `SKU-001`. | Conflict or validation-style response; no duplicate SKU. |
| Duplicate barcode | Create another SKU with barcode `4600000000011`. | Conflict or validation-style response; no duplicate barcode assignment. |
| Storage Location lifecycle | Deactivate, list with `includeInactive=true`, reactivate. | Record is not deleted; stable ID remains unchanged; `isActive` changes correctly. |
| SKU lifecycle | Deactivate, list with `includeInactive=true`, reactivate. | SKU is not deleted; stable ID remains unchanged; `isActive` changes correctly. |
| Update identity | Update Warehouse, Zone, Storage Location, or SKU editable attributes. | Stable `id` remains unchanged and uniqueness validation still applies. |
| Unit of Measure | List Units of Measure. | Seeded read-only units are available; create/update/delete UoM is out of scope. |

## Warehouse Layout

```http
GET /api/warehouse-foundation/warehouses/{warehouseId}/layout?includeInactive=true
```

Expected response represents:

```text
Warehouse → Zones → Storage Locations
```

It includes warehouse, zone, and storage location identity/display fields, active/inactive state, location addresses, and setup readiness indicators.

## Manual UI Validation

`Formica.WebApp` should allow the user to verify:

- Warehouse and Zone management in M1;
- Storage Location management in M2;
- SKU/barcode management in M3;
- Warehouse Layout, lifecycle state, and setup readiness in M4;
- validation and uniqueness errors without duplicating business rules in UI code.

UI labels may be localized later, but domain/API terminology remains based on the specification and glossary.

## Full Completion Criteria

Full Warehouse Foundation is ready when:

- all user stories from `spec.md` pass through API behavior or UI behavior;
- uniqueness rules are enforced by validation and database constraints;
- response models expose lifecycle state through `isActive`;
- default Location Address Rules exist;
- seeded Units of Measure are available for SKUs;
- Warehouse Layout shows configured Warehouse → Zones → Storage Locations;
- no out-of-scope workflow is implemented accidentally.
