# Formica Product Vision

## Product Identity

Formica is the working product name.

The initial product module is Formica Warehouse.

A personal or umbrella brand is not defined yet.

## Product Intent

Formica is intended to become a practical business operations platform.

Its purpose is to help small and medium-sized trading, warehouse, and fulfillment-oriented organizations control operational processes that are often fragmented between spreadsheets, accounting systems, manual warehouse work, and ad-hoc integrations.

Formica should bridge the gap between formal accounting systems and real operational execution.

## Initial Product Area

The first product area is warehouse operations and warehouse accounting.

Formica Warehouse starts with the operational foundation required for future warehouse workflows:

- warehouse structure;
- zones;
- storage locations;
- SKU references;
- stock visibility foundation;
- future receiving, putaway, inventory, picking, packing, and shipping workflows.

The initial goal is not to implement a complete WMS at once.

## Target Users

Initial target users are:

- warehouse administrator or operations manager;
- warehouse operator or storekeeper;
- business owner or manager responsible for operational control;
- developer or integration specialist connecting Formica with external systems.

The first milestone primarily serves the warehouse administrator or operations manager by making the warehouse structure explicit and manageable.

## Problems to Solve

Formica should address the following practical problems:

- warehouse structure is often implicit, inconsistent, or maintained outside the operational system;
- stock balances are difficult to trust when warehouse operations and accounting records diverge;
- manual warehouse work and spreadsheets hide operational errors;
- integrations with accounting or ERP systems are often fragile, blocking, or too tightly coupled;
- mobile and scanning workflows are hard to introduce without a stable warehouse model;
- operational analytics are unreliable when basic warehouse data is not structured.

## Product Boundaries

Formica is not intended to replace 1C, ERP, accounting, CRM, or e-commerce systems.

Formica should own warehouse execution and operational state where real warehouse work requires it.

External systems may remain systems of record for selected master data, documents, customers, suppliers, or accounting facts.

Formica must integrate with external systems without allowing them to dictate the internal domain model.

## First Milestone

The first milestone is Warehouse Foundation.

The goal is to establish a coherent base for warehouse operations by supporting:

- warehouses;
- zones;
- storage locations;
- basic SKU references;
- basic operational visibility needed by future stock workflows.

Out of scope for the first milestone:

- complete receiving workflow;
- putaway optimization;
- picking route optimization;
- Android/mobile client;
- full 1C synchronization;
- Bitrix24 integration;
- advanced analytics;
- demand forecasting or planning.

## Long-Term Direction

Potential future capabilities include:

- receiving;
- putaway;
- storage operations;
- inventory checks and adjustments;
- picking;
- packing;
- shipping;
- barcode and QR scanning;
- Android handheld terminal or mobile thin client;
- 1C UT integration;
- possible 1C ERP and 1C Complex Automation integration;
- possible Bitrix24 integration;
- operational analytics and KPIs;
- replenishment, demand, and stock optimization.

These directions are not commitments until expressed through feature specifications.
