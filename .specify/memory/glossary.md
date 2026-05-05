# Formica Glossary

## Purpose

This glossary defines the initial ubiquitous language for Formica.

It is intentionally concise and must evolve through feature specifications. It should clarify durable terminology and known ambiguities without becoming an implementation model.

## Product Terms

### Formica

The working product name.

Formica is intended to become a practical business operations platform.

### Formica Warehouse

The initial product module focused on warehouse operations and warehouse accounting.

### Warehouse Foundation

The first milestone for Formica Warehouse.

Its purpose is to establish a coherent base for warehouse operations: warehouses, zones, storage locations, basic SKU references, and basic operational visibility needed by future stock workflows.

## Warehouse Structure

### Warehouse

A physical or logical warehouse where stock is stored, controlled, received, moved, counted, picked, packed, or shipped.

### Zone

A named area within a warehouse.

A zone may represent a storage area, receiving area, shipping area, picking area, quarantine area, temperature-controlled area, or another operational subdivision.

### Storage Location

An addressable warehouse place where stock can be stored or processed.

Examples include shelf locations, bin locations, pallet positions, staging places, and other operationally addressable places.

For the Warehouse Foundation milestone, use `Storage Location` as the primary term. Do not introduce `Warehouse Location` as a primary term until a feature requires a more general model for receiving docks, shipping docks, packing stations, staging areas, or other non-storage locations.

### Location Address

A human-readable or scannable code identifying a storage location.

## Product and Stock

### Product

A commercial or catalog concept.

A product may represent what a business sells, buys, imports from an external catalog, or exposes to external systems.

### SKU

Stock Keeping Unit.

A SKU is the primary warehouse-operational identity of stock.

SKU answers the question: what stock is this?

Product and SKU may be one-to-one in simple cases, but they must not be treated as globally equivalent concepts.

### Composite SKU / Kit

A SKU that represents a sellable or operationally recognized set of other SKUs.

A physical box containing multiple SKUs is not automatically a Composite SKU or Kit. It is a Handling Unit unless the business treats that box as a product or SKU in its own right.

### Unit of Measure

A quantity unit used for stock operations, such as piece, box, kilogram, liter, meter, pallet, or package.

### Stock

Goods or quantity available, expected, reserved, blocked, or recorded in a warehouse context.

Stock may refer to physical goods or to recorded quantities, depending on context.

### Inventory

The system's recorded view of stock state.

Use `Inventory` as a domain area term, not as a vague entity name.

Prefer more specific terms for concrete concepts, such as `Inventory Balance`, `Inventory Movement`, `Inventory Adjustment`, or `Inventory Count`.

### Inventory Balance

Recorded quantity of stock by relevant dimensions such as SKU, warehouse, storage location, batch, serial, status, or owner.

### Inventory Movement

A recorded change in inventory state.

Examples include receipt, movement between locations, adjustment, reservation, allocation, blocking, unblocking, picking, or shipping.

### Inventory Count

A physical counting process used to verify recorded inventory balances.

In Russian UI this may correspond to inventory counting or stocktaking, not to the general inventory domain.

### Batch

A group of stock units sharing the same production, supplier, receipt, or business batch identity.

### Serial

A unique identifier for an individual physical item.

## Handling and Identification

### Handling Unit

A physical or logical unit handled as one object in warehouse operations.

Examples include pallet, box, container, tote, roll, bag, or another movable/storage unit.

A handling unit may contain one SKU, multiple SKUs, batches, serials, or nested handling units.

### LPN

License Plate Number.

A unique identifier assigned to a handling unit.

LPN answers the question: which specific handling unit is this?

LPN is related to Handling Unit but is not the physical unit itself. Avoid using `LPN` as the name of the handling unit entity unless a feature deliberately chooses that simplification.

SKU identifies what the stock is. LPN identifies how specific stock is physically or logistically handled.

### Handling Unit Content

The stock contained inside a handling unit.

Handling unit content may include one SKU, multiple SKUs, batches, serials, or nested handling units.

### Barcode

A machine-readable code used to identify SKU, package, location, document, handling unit, or another operational object.

### QR Code

A two-dimensional machine-readable code used similarly to a barcode, often with more encoded data.

### TSD / Handheld Terminal

A mobile warehouse device with scanning capabilities.

### Mobile Thin Client

A lightweight mobile client used by warehouse operators, likely on Android handheld terminals or phones.

## Warehouse Operations

### Warehouse Accounting

Tracking recorded warehouse state: stock, balances, movements, batches, serials, statuses, reservations, and adjustments.

Warehouse accounting is a supporting capability of warehouse management, not a replacement for it.

### Warehouse Management

Operational execution of warehouse workflows: receiving, putaway, storage operations, inventory counts, picking, packing, shipping, and worker tasks.

Warehouse management answers what people and systems should do in the warehouse.

### Inbound

The broader domain area for incoming goods.

Inbound may include expected receipts, supplier deliveries, arrival, receiving, quality checks, staging, and handoff to putaway.

### Receiving

The workflow of accepting and verifying goods into the warehouse.

Receiving is part of the broader inbound area.

### Putaway

The workflow of moving received goods from receiving or staging areas into storage locations.

### Slotting

A planning or optimization capability that determines where stock should be stored for operational efficiency and constraints.

Slotting is not the same as putaway. Putaway is execution; slotting is planning or optimization.

### Storage Operations

Warehouse operations related to stored stock after receiving and before outbound fulfillment.

Examples include internal movement, blocking, unblocking, adjustment, replenishment, and stock status changes.

### Picking

The workflow of collecting stock from storage locations for outbound fulfillment.

### Packing

The workflow of preparing picked goods for shipment.

### Shipping

The workflow of dispatching goods from the warehouse.

### Reservation

A stock availability constraint indicating that a quantity is held for a demand source, such as an order.

Reservation does not necessarily identify the exact physical stock, location, or handling unit to use.

### Allocation

A more concrete assignment of stock for execution.

Allocation may identify specific stock, location, batch, serial, handling unit, or quantity to satisfy a demand source.

## Integrations

### 1C UT

1C Trade Management.

A likely external system for selected master data, documents, accounting records, or synchronization scenarios.

### 1C ERP / 1C Complex Automation

Possible future external systems for integration.

### Bitrix24

A possible future external system for integration.

### Notification

A short message from an external system indicating that an entity or document changed.

A notification should not automatically imply that the full entity payload is included.

### Asynchronous Synchronization

A synchronization approach where Formica receives or detects a change and later loads or processes the complete data without blocking the external system request.

Specific integration mechanisms belong in feature `plan.md`, not in the glossary.

## Ambiguous Terms and Decisions

### Product and SKU

Product and SKU are distinct concepts.

SKU is the primary domain term for warehouse stock identity.

Russian UI may later use user-friendly labels such as product, item, goods, nomenclature, or SKU, but the domain language should keep SKU explicit.

### Storage Location

Use `Storage Location` for the first milestone.

Avoid `Cell` as a domain term because it is less natural in English and may conflict with non-warehouse meanings.

Avoid introducing a general `Warehouse Location` term until a feature requires it.

### Handling Unit and LPN

Handling Unit is the physical or logical unit handled as one object.

LPN is the identifier assigned to that handling unit.

A mixed physical box is a Handling Unit unless the business treats it as a sellable or operationally recognized Composite SKU / Kit.

### Inventory

Avoid using `Inventory` as a vague entity name.

Use specific terms such as `Inventory Balance`, `Inventory Movement`, `Inventory Adjustment`, and `Inventory Count`.

### Stock Item

Avoid using `Stock Item` as a primary domain term unless a feature defines it explicitly.

The term is ambiguous and may refer to SKU, inventory balance, physical unit, document line, or handling unit content.

Prefer more specific terms: `SKU`, `Inventory Balance`, `Inventory Movement`, `Handling Unit Content`, `Receiving Line`, or `Shipment Line`.
