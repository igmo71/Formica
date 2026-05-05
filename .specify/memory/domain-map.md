# Formica Domain Map

## Purpose

This document describes the initial domain landscape for Formica.

It defines capability areas, their responsibilities, maturity, and relationships. It does not prescribe .NET project structure, database schema, API contracts, UI components, messaging mechanisms, or implementation layers.

## Current Product Boundary

The current product boundary is Formica Warehouse.

Formica Warehouse is the first product module of Formica and focuses on warehouse operations and warehouse accounting.

The first milestone is Warehouse Foundation.

## Domain Area Statuses

Domain areas may use the following maturity/status markers:

- Current: active focus for the first milestone.
- Supporting: required to support the current focus, but not a complete standalone capability yet.
- Near-term: expected to become a core area soon after the first milestone.
- Future: important future capability, not part of the first milestone.
- External-facing: boundary area used to interact with external systems.
- Cross-cutting: capability that participates in multiple warehouse workflows.

## Domain Areas

### Warehouse Foundation

Status: Current.

Purpose:
Describe the physical and logical warehouse structure needed by future warehouse workflows.

Candidate concepts:

- Warehouse
- Zone
- Storage Location
- Location Address
- Location Type
- Location Status
- basic operational attributes

Responsibilities:

- make warehouse structure explicit and manageable;
- provide a stable location model for future inventory, receiving, putaway, storage, picking, packing, shipping, and mobile workflows;
- establish the initial operational foundation without implementing a complete WMS.

Notes:

Warehouse Foundation is the first milestone. It must stay focused and must not absorb full inventory accounting, receiving, putaway, outbound, mobile, or integration workflows prematurely.

### Product / SKU Reference

Status: Supporting.

Purpose:
Provide the minimal item reference model needed for warehouse operations.

Candidate concepts:

- Product
- SKU
- Composite SKU / Kit
- Unit of Measure
- Barcode

Responsibilities:

- identify what stock can be stored, received, moved, counted, picked, packed, or shipped;
- support Warehouse Foundation with basic SKU references;
- leave room for future product catalog and integration scenarios.

Notes:

Product and SKU are distinct concepts. The first milestone requires only basic SKU references, not a complete product catalog.

### Inventory

Status: Near-term.

Purpose:
Represent the system's recorded view of stock state.

Candidate concepts:

- Stock
- Inventory Balance
- Inventory Movement
- Inventory Adjustment
- Inventory Count
- Stock Status
- Reservation
- Allocation
- Batch
- Serial

Responsibilities:

- record stock quantities by relevant dimensions;
- record stock movements and state changes;
- support stock visibility, adjustments, counts, reservations, allocations, and operational control.

Notes:

Inventory is a core warehouse area, but full inventory accounting is not part of the Warehouse Foundation milestone. Warehouse Foundation should be compatible with future inventory balances and movements without implementing the full inventory model immediately.

### Handling Units

Status: Future, Cross-cutting.

Purpose:
Represent physical or logical units handled as one object in warehouse operations.

Candidate concepts:

- Handling Unit
- LPN
- Handling Unit Content
- nested handling units
- pallet
- box
- container
- tote

Responsibilities:

- support handling of pallets, boxes, containers, and other movable units;
- support future receiving, putaway, storage, picking, shipping, and mobile scanning workflows;
- represent mixed contents when one handling unit contains multiple SKUs, batches, serials, or nested handling units.

Notes:

Handling Unit is the object being handled. LPN is the identifier assigned to it. Handling Units are future-critical but out of scope for Warehouse Foundation unless a specific feature requires them.

### Inbound

Status: Future.

Purpose:
Manage incoming goods before they become available for storage or downstream operations.

Candidate concepts:

- Expected Receipt
- Supplier Delivery
- Receiving
- Receiving Line
- Actual Receipt
- Receiving Discrepancy
- Quality Check
- Staging

Responsibilities:

- represent expected and actual incoming goods;
- support receiving and verification workflows;
- capture discrepancies between expected and actual receipts;
- hand received goods off to putaway or storage workflows.

Notes:

Inbound is the broader domain area. Receiving is a specific workflow inside Inbound.

### Putaway

Status: Future.

Purpose:
Move received goods from receiving or staging areas into storage locations.

Candidate concepts:

- Putaway Task
- Suggested Storage Location
- Actual Storage Location
- Putaway Confirmation
- Putaway Exception

Responsibilities:

- support movement from receiving/staging to storage;
- confirm actual placement;
- prepare for future mobile execution;
- integrate with future inventory movements.

Notes:

Putaway is execution. Slotting is planning or optimization and must not be treated as the same capability.

### Storage Operations

Status: Future.

Purpose:
Manage warehouse operations related to stored stock after receiving and before outbound fulfillment.

Candidate concepts:

- Internal Movement
- Location-to-Location Transfer
- Blocking
- Unblocking
- Adjustment
- Replenishment
- Stock Status Change

Responsibilities:

- support day-to-day stock movement and state changes inside the warehouse;
- keep recorded inventory aligned with operational actions;
- provide operational history for future analysis and auditing.

Notes:

Storage Operations are different from Warehouse Foundation. Warehouse Foundation describes the warehouse structure; Storage Operations describe actions performed with stock inside that structure.

### Outbound

Status: Future.

Purpose:
Manage outbound fulfillment from demand to shipment.

Candidate concepts:

- Shipment Order
- Reservation
- Allocation
- Picking Task
- Pick Confirmation
- Packing
- Shipment
- Shipping Confirmation

Responsibilities:

- reserve and allocate stock for demand;
- support picking, packing, and shipping workflows;
- prepare for future route optimization and mobile execution.

Notes:

Outbound depends on Warehouse Foundation, Product / SKU Reference, Inventory, and Storage Operations maturity.

### Mobile Operations

Status: Future, Cross-cutting.

Purpose:
Support warehouse operator execution through mobile or handheld devices.

Candidate concepts:

- Operator Task
- Scan Action
- Device Session
- Location Confirmation
- SKU Confirmation
- LPN Confirmation
- Discrepancy Report

Responsibilities:

- allow operators to execute warehouse workflows through guided tasks;
- support barcode and QR scanning;
- confirm operational actions at the point of work.

Notes:

Mobile Operations is an execution channel for warehouse workflows, not an independent source of domain rules. A mobile client may be implemented as a separate application, but business rules must belong to the relevant warehouse domain areas.

### Integrations

Status: External-facing.

Purpose:
Translate between external systems and Formica domain capabilities.

Candidate external systems:

- 1C UT
- 1C ERP
- 1C Complex Automation
- Bitrix24

Candidate concepts:

- External Reference
- Notification
- Synchronization Job
- Import State
- Export State
- Mapping
- Conflict

Responsibilities:

- receive or detect external changes;
- translate external data into Formica domain concepts;
- maintain synchronization state;
- isolate the Formica domain model from external system models.

Notes:

Integrations must not own Formica's core domain model. Specific mechanisms such as HTTP callbacks, queues, channels, background workers, or brokers belong in feature `plan.md`, not in this domain map.

### Analytics / Optimization

Status: Future.

Purpose:
Provide analytical and optimization capabilities based on mature operational data.

Candidate concepts:

- Operational KPI
- Stock Turnover
- Stock Coverage
- Shortage Risk
- ABC Analysis
- XYZ Analysis
- Picking Performance
- Slotting Recommendation
- Demand Forecast

Responsibilities:

- analyze warehouse performance;
- support better stock and operational decisions;
- enable optimization after reliable operational data exists.

Notes:

Analytics and optimization depend on mature operational data. They are not part of Warehouse Foundation.

## First Milestone Scope

The first milestone is Warehouse Foundation.

In scope:

- warehouses;
- zones;
- storage locations;
- location addresses;
- basic SKU references;
- basic operational visibility required by future stock workflows.

Out of scope:

- full inventory accounting;
- inventory movements;
- receiving workflow;
- putaway workflow;
- handling unit lifecycle;
- LPN management;
- outbound workflows;
- mobile client;
- 1C synchronization implementation;
- Bitrix24 integration;
- analytics and optimization.

## Boundary Decisions

### Product / SKU Reference

Product / SKU Reference is logically separate from Warehouse Foundation, but the first milestone requires only basic SKU references.

Do not build a full Product Catalog until a feature requires it.

### Inventory

Inventory is a near-term core area, not a complete part of the first milestone.

Warehouse Foundation should prepare for future inventory balances and movements, but must not implement full inventory accounting prematurely.

### Inbound and Putaway

Inbound and Putaway are separate future workflow areas.

Receiving belongs to Inbound. Putaway starts after goods are received or staged and moves them into storage locations.

### Putaway and Slotting

Putaway is an execution workflow.

Slotting is a planning or optimization capability.

They must not be treated as the same domain area.

### Handling Units and LPN

Handling Units are cross-cutting warehouse capability.

LPN identifies a handling unit but is not the handling unit itself.

Handling Units should be introduced through a concrete feature, not through Warehouse Foundation by default.

### Mobile Operations

Mobile Operations is an execution channel for warehouse workflows.

It may require a separate application later, but it must not become an independent source of business rules.

### Integrations

Integrations are boundary areas.

External systems may be systems of record for selected data, but they must not dictate Formica's internal domain model.

## Out of Scope for This Domain Map

This document does not define:

- .NET solution or project structure;
- module folder structure;
- database schema;
- API contracts;
- UI screens or components;
- messaging technology;
- queue names;
- background worker design;
- command, handler, repository, or service classes;
- implementation tasks.

Those decisions belong in feature specifications, plans, contracts, research notes, or tasks.
