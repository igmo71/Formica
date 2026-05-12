# Feature Specification: Warehouse Foundation

**Feature Branch**: `001-warehouse-foundation`  
**Created**: 2026-05-04  
**Status**: Ready for Planning  
**Input**: Establish the first Formica Warehouse foundation feature. Users must be able to create warehouses, create warehouse zones, create storage locations, configure location addresses, maintain basic SKUs, view the basic warehouse layout, and prepare a stable base for future inventory workflows.

## User Scenarios & Testing

### User Story 1 - Create a warehouse (Priority: P1)

As a warehouse administrator, I want to create a warehouse with a clear code and name, so that Formica has an explicit operational representation of a physical or logical warehouse.

**Why this priority**: Warehouse is the top-level operational structure. Zones, storage locations, and future inventory workflows require a warehouse context.

**Independent Test**: A user can create a warehouse, view it in the warehouse list, and distinguish it from other warehouses by code and name.

**Acceptance Scenarios**:

1. **Given** no warehouse exists, **When** the warehouse administrator creates a warehouse with a unique code and name, **Then** the warehouse is available for future zone and storage location setup.
2. **Given** a warehouse already exists with a code, **When** the warehouse administrator attempts to create another warehouse with the same code, **Then** the system rejects the duplicate and explains that the warehouse code must be unique.

---

### User Story 2 - Create warehouse zones (Priority: P1)

As a warehouse administrator, I want to create zones inside a warehouse, so that the warehouse can be organized into meaningful operational areas.

**Why this priority**: Zones provide the first level of warehouse organization and are required to group storage locations in a manageable way.

**Independent Test**: A user can create zones for a selected warehouse and view them as part of that warehouse layout.

**Acceptance Scenarios**:

1. **Given** a warehouse exists, **When** the warehouse administrator creates a zone with a code, name, and purpose, **Then** the zone is listed under that warehouse.
2. **Given** a warehouse has an existing zone code, **When** the warehouse administrator attempts to create another zone with the same code in the same warehouse, **Then** the system rejects the duplicate.
3. **Given** two different warehouses exist, **When** the same zone code is used in both warehouses, **Then** the system allows it because zone codes are scoped to a warehouse.

---

### User Story 3 - Create storage locations with configured addresses (Priority: P1)

As a warehouse administrator, I want to create storage locations with consistent location addresses, so that warehouse workers and future workflows can identify where stock is stored or processed.

**Why this priority**: Storage locations and their addresses are the operational basis for future inventory balances, receiving, putaway, storage operations, picking, packing, shipping, and mobile scanning workflows.

**Independent Test**: A user can create storage locations inside zones, assign addresses, and see them in the warehouse layout.

**Acceptance Scenarios**:

1. **Given** a warehouse and zone exist, **When** the warehouse administrator creates a storage location with a location address, **Then** the location appears under the selected zone.
2. **Given** a storage location address already exists within a warehouse, **When** the warehouse administrator attempts to create another storage location with the same address in the same warehouse, **Then** the system rejects the duplicate.
3. **Given** the same location address is used in two different warehouses, **When** the warehouse administrator creates both locations, **Then** the system allows it because location addresses are unique within a warehouse, not globally.
4. **Given** a location address is entered, **When** it does not match the configured addressing expectations, **Then** the system prevents saving or asks the user to correct the address.

---

### User Story 4 - Maintain basic SKUs (Priority: P1)

As a warehouse administrator, I want to maintain basic SKUs, so that future inventory and warehouse operations can refer to stock consistently.

**Why this priority**: Warehouse layout alone is not enough for inventory workflows. Future stock balances and movements need stable SKUs.

**Independent Test**: A user can create a SKU with a unique code, name, base unit of measure, one or more optional barcode values, and active status.

**Acceptance Scenarios**:

1. **Given** no SKU exists with a code, **When** the warehouse administrator creates a SKU with code, name, and base unit of measure, **Then** the SKU is available for future warehouse workflows.
2. **Given** a SKU code already exists, **When** the warehouse administrator attempts to create another SKU with the same code, **Then** the system rejects the duplicate.
3. **Given** a SKU is no longer used operationally, **When** the warehouse administrator marks it inactive, **Then** the SKU remains visible as a reference but is not treated as an active operational choice.
4. **Given** a SKU has one barcode value, **When** the warehouse administrator adds another barcode value to the same SKU, **Then** the system allows multiple barcode values for that SKU.
5. **Given** a barcode value already belongs to an existing SKU, **When** the warehouse administrator attempts to assign the same barcode value to another SKU, **Then** the system rejects the duplicate barcode value.

---

### User Story 5 - View basic warehouse layout (Priority: P2)

As an operations manager, I want to view the warehouse layout by warehouse, zone, and storage location, so that I can verify that the operational model matches the real warehouse.

**Why this priority**: Users need confidence that the warehouse foundation has been configured correctly before future inventory workflows depend on it.

**Independent Test**: A user can open a warehouse layout view and see warehouses, zones, and storage locations with their addresses and statuses.

**Acceptance Scenarios**:

1. **Given** warehouses, zones, and storage locations exist, **When** the operations manager views the warehouse layout, **Then** the layout is shown as warehouse → zone → storage locations.
2. **Given** a storage location is inactive, **When** the warehouse layout is viewed, **Then** the inactive state is visible to the user.
3. **Given** a warehouse has no zones or locations yet, **When** the warehouse layout is viewed, **Then** the system clearly shows that setup is incomplete.

---

### User Story 6 - Maintain stable references for future inventory workflows (Priority: P2)

As a system stakeholder, I want the warehouse foundation to maintain stable references for warehouses, storage locations, and SKUs across create, update, deactivate, and reactivate operations, so that future inventory balances and movements can safely refer to them.

**Why this priority**: The first milestone must not implement full inventory accounting, but it must avoid choices that would block future inventory workflows or destroy operational reference history.

**Independent Test**: A configured warehouse layout and SKU list remain stable enough for future features to reference without relying on display names or manually interpreted text.

**Acceptance Scenarios**:

1. **Given** a warehouse, storage location, or SKU is created, **When** its display name changes later, **Then** its identity remains stable for future workflows.
2. **Given** a warehouse, zone, storage location, or SKU may be used by future operational records, **When** a user wants to stop using it, **Then** the system supports deactivation rather than forcing destructive removal.
3. **Given** an inactive warehouse, zone, storage location, or SKU still satisfies uniqueness and validation rules, **When** the warehouse administrator reactivates it, **Then** the reference becomes active again without changing its stable identity.
4. **Given** a user updates editable attributes of a warehouse, zone, storage location, or SKU, **When** the update passes validation, **Then** the system preserves the same stable identity and applies the changed attributes.

## Requirements

### Functional Requirements

- **FR-001**: The system MUST allow a warehouse administrator to create and maintain multiple warehouses.
- **FR-002**: Each warehouse MUST have a unique warehouse code.
- **FR-003**: Each warehouse MUST have a human-readable display name.
- **FR-004**: The system MUST allow a warehouse administrator to create and maintain zones inside a warehouse.
- **FR-005**: Zone codes MUST be unique within a warehouse.
- **FR-006**: The system MUST allow a zone to be classified by operational purpose or type.
- **FR-007**: The system MUST allow a warehouse administrator to create and maintain storage locations inside zones.
- **FR-008**: Each storage location MUST have a location address.
- **FR-009**: Location addresses MUST be unique within a warehouse.
- **FR-010**: The system MUST support configurable location addressing sufficient to keep location addresses consistent, human-readable, unique within a warehouse, and suitable for future scanning workflows.
- **FR-011**: The system MUST allow a storage location to be classified by operational purpose or type.
- **FR-012**: The system MUST allow warehouses, zones, and storage locations to be marked active or inactive.
- **FR-013**: The system MUST allow a warehouse administrator to create and maintain basic SKUs.
- **FR-014**: Each SKU MUST have a unique SKU code.
- **FR-015**: Each SKU MUST have a human-readable name.
- **FR-016**: Each SKU MUST have a base unit of measure.
- **FR-017**: A SKU MAY have one or more optional barcode values.
- **FR-018**: Barcode values, when provided, MUST be unique across all SKUs unless a future feature explicitly defines barcode reuse or aliasing rules.
- **FR-019**: The system MUST allow SKUs to be marked active or inactive.
- **FR-020**: The system MUST allow an operations manager to view the warehouse layout as warehouse → zone → storage locations.
- **FR-021**: The warehouse layout view MUST show location addresses and active/inactive status.
- **FR-022**: The system MUST keep stable references for warehouses, zones, storage locations, and SKUs so future inventory workflows can refer to them safely.
- **FR-023**: The system MUST prevent duplicate warehouse codes, duplicate SKU codes, duplicate zone codes within a warehouse, duplicate location addresses within a warehouse, and duplicate barcode values across SKUs.
- **FR-024**: The system MUST allow users to update editable attributes of warehouses, zones, storage locations, and SKUs while preserving their stable identity.
- **FR-025**: The system MUST use deactivation and reactivation as the primary lifecycle mechanism for operational reference data that may be used by future workflows.
- **FR-026**: The system MUST NOT require physical deletion of warehouses, zones, storage locations, or SKUs for normal user workflows.
- **FR-027**: If physical deletion is introduced later, it MUST be limited to reference records that have never been used by operational workflows or referenced by other records.

### Non-Goals / Out of Scope

The Warehouse Foundation feature MUST NOT implement:

- full inventory accounting;
- inventory balances;
- inventory movements;
- inventory adjustments;
- inventory counts;
- receiving workflow;
- putaway workflow;
- handling unit lifecycle;
- LPN management;
- picking workflow;
- packing workflow;
- shipping workflow;
- Android or handheld mobile client;
- barcode scanning workflow;
- 1C synchronization implementation;
- Bitrix24 integration;
- analytics or optimization;
- automatic slotting;
- route optimization.

This feature specification intentionally does not define:

- database schema;
- API contracts;
- UI component design;
- implementation structure.

These HOW-level details belong to downstream Spec Kit artifacts such as `plan.md`, `data-model.md`, `contracts/`, `quickstart.md`, and `tasks.md`.

### Key Entities

- **Warehouse**: A physical or logical warehouse where stock is stored, controlled, received, moved, counted, picked, packed, or shipped.
- **Zone**: A named area within a warehouse used to organize storage locations and future operational workflows.
- **Storage Location**: An addressable warehouse place where stock can be stored or processed.
- **Location Address**: A human-readable or scannable code identifying a storage location within a warehouse.
- **SKU**: The primary warehouse-operational identity of stock. In Warehouse Foundation this is a basic SKU model, not a full Product Catalog.
- **SKU Barcode**: A barcode value associated with a SKU. A SKU may have multiple barcode values.
- **Unit of Measure**: A quantity unit used for stock operations.

## Success Criteria

### Measurable Outcomes

- **SC-001**: A warehouse administrator can configure at least one complete warehouse layout consisting of one warehouse, multiple zones, and multiple storage locations.
- **SC-002**: The system prevents duplicate warehouse codes, duplicate SKU codes, duplicate zone codes within the same warehouse, duplicate location addresses within the same warehouse, and duplicate barcode values across SKUs.
- **SC-003**: An operations manager can view the configured warehouse layout in a way that clearly shows warehouse, zone, storage location, address, and active/inactive status.
- **SC-004**: A warehouse administrator can create basic SKUs with code, name, base unit of measure, one or more optional barcode values, and active/inactive status.
- **SC-005**: Future inventory specifications can refer to warehouses, storage locations, and SKUs without redefining these concepts.
- **SC-006**: A warehouse administrator can update, deactivate, and reactivate warehouse foundation records without changing their stable identity.

## Assumptions

- The system supports multiple warehouses.
- Location addresses are unique within a warehouse, not globally.
- Zone codes are unique within a warehouse, not globally.
- SKU codes are globally unique within Formica Warehouse unless a later feature specification changes this assumption.
- A SKU may have multiple barcode values, reflecting practical catalog and 1C-style nomenclature scenarios.
- Barcode values are unique across all SKUs unless a future feature explicitly defines barcode reuse or aliasing rules.
- Full Product Catalog is out of scope; the first milestone requires only basic SKUs.
- Russian UI labels may use user-friendly business wording later, but the domain language for specifications remains based on the glossary.
- Operational reference data uses a lifecycle model where deactivation/reactivation is preferred over destructive deletion.

## Clarifications

### Session 2026-05-06

- **CN-001**: Location addresses use user-defined address codes validated by configurable warehouse-level address rules. The first milestone does not require a fixed aisle/rack/level/position model.
- **CN-002**: Initial zone purpose/type values are: Storage, Receiving, Shipping, Picking, Packing, Staging, QualityControl, Quarantine, Other.
- **CN-003**: Initial storage location purpose/type values are: Storage, Picking, Staging, Receiving, Shipping, Packing, QualityControl, Quarantine, Other.
- **CN-004**: A SKU may have multiple barcode values. Barcode values, when provided, must be unique across all SKUs unless a future feature explicitly defines barcode reuse or aliasing rules.
- **CN-005**: Inactive warehouses, zones, storage locations, and SKUs can be reactivated if they still satisfy uniqueness and validation rules.
- **CN-006**: Storage locations may have optional capacity attributes for future use, but Warehouse Foundation does not calculate capacity consumption, suitability, slotting, or optimization.
- **CN-007**: Warehouses, zones, storage locations, and SKUs use a lifecycle model: create, update, deactivate, and reactivate. Physical deletion is not the primary user operation. If deletion is supported later, it must be limited to records that have never been used by operational workflows or referenced by other records.
