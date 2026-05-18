# Tasks: Warehouse Foundation

**Input**: Design documents from `specs/001-warehouse-foundation/`
**Prerequisites**: `spec.md`, `plan.md`, `research.md`, `data-model.md`, `implementation-guidelines.md`, `contracts/api-contracts.md`, `quickstart.md`
**Branch**: `001-warehouse-foundation`

## Format

- `[P]`: can run in parallel because it touches different files or independent areas.
- `[Story]`: user story from `spec.md`.
- Follow `implementation-guidelines.md` for dependency direction, domain/result patterns, migration policy, UI rules, and agent validation policy.

## Milestone Map

- **M1**: `Formica.WebApp` foundation plus Warehouse and Zone management end-to-end.
- **M2**: Storage Locations and configured address behavior end-to-end.
- **M3**: SKU, Unit of Measure, and barcode behavior end-to-end.
- **M4**: Warehouse Layout, lifecycle consistency, and final cross-cutting validation.

## Phase 1: Setup

- [X] T001 Add PostgreSQL Aspire hosting package to `Formica.AppHost/Formica.AppHost.csproj`.
- [X] T002 Add EF Core Npgsql provider and EF Core design package references to `Formica.ApiService/Formica.ApiService.csproj`.
- [X] T003 Configure PostgreSQL resource and API service connection in `Formica.AppHost/Program.cs`.
- [X] T004 Create Warehouse module folders under `Formica.ApiService/Warehouse/`.
- [X] T005 Create Warehouse Foundation UI folders under `Formica.Web/Warehouse/WarehouseFoundation/`.
- [X] T006 Create Warehouse Foundation test folders under `Formica.Tests/Warehouse/WarehouseFoundation/`.
- [ ] T006A Create target Warehouse Foundation UI folders under `Formica.WebApp/Warehouse/WarehouseFoundation/`.

## Phase 2: Foundational

### Domain and Persistence Foundation

- [X] T007 [P] Create lifecycle base abstractions or helper types in `Formica.ApiService/Warehouse/WarehouseFoundation/Domain/Common/`.
- [X] T008 [P] Create controlled value definitions for zone and storage location purposes in `Formica.ApiService/Warehouse/WarehouseFoundation/Domain/Common/`.
- [X] T009 [P] Create `StorageLocationCapacity` value-object-style type in `Formica.ApiService/Warehouse/WarehouseFoundation/Domain/StorageLocations/StorageLocationCapacity.cs`.
- [X] T010 [P] Create `LocationAddressRules` domain model in `Formica.ApiService/Warehouse/WarehouseFoundation/Domain/LocationAddressing/LocationAddressRules.cs`.
- [X] T011 Create `WarehouseDbContext` in `Formica.ApiService/Warehouse/Persistence/WarehouseDbContext.cs`.
- [X] T012 Create EF Core configuration folder `Formica.ApiService/Warehouse/Persistence/Configurations/WarehouseFoundation/`.
- [X] T013 Create Warehouse persistence registration extension in `Formica.ApiService/Warehouse/Persistence/WarehousePersistenceServiceCollectionExtensions.cs`.
- [X] T014 Register Warehouse persistence and Warehouse Foundation endpoints in `Formica.ApiService/Program.cs`.
- [X] T015 Prepare Warehouse Foundation migration infrastructure under `Formica.ApiService/Warehouse/Persistence/Migrations/` without creating migration files.

### API and Test Foundation

- [X] T016 Create endpoint group registration entry point in `Formica.ApiService/Warehouse/WarehouseFoundation/Endpoints/WarehouseFoundationEndpoints.cs`.
- [X] T017 [P] Create shared API result/problem helpers in `Formica.ApiService/Warehouse/WarehouseFoundation/Endpoints/EndpointResults.cs`.
- [X] T018 [P] Create shared contract DTO folder and currently needed common DTOs in `Formica.ApiService/Warehouse/WarehouseFoundation/Contracts/`.
- [X] T019 [P] Create domain validation primitives in `Formica.ApiService/Warehouse/WarehouseFoundation/Domain/Common/Validation/`.
- [X] T020 [P] Implement location address normalization behavior in Domain.
- [X] T021 Create Aspire-backed API test fixture in `Formica.Tests/Warehouse/WarehouseFoundation/WarehouseFoundationApiFixture.cs`.
- [X] T022 Create PostgreSQL-backed persistence test fixture in `Formica.Tests/Warehouse/WarehouseFoundation/WarehousePersistenceFixture.cs`.
- [X] T023 [P] Create test data builders in `Formica.Tests/Warehouse/WarehouseFoundation/TestData/WarehouseFoundationTestData.cs`.

## Phase 3: M1 / US1 Warehouses

- [X] T024 [P] [US1] Add warehouse API contract tests in `Formica.Tests/Warehouse/WarehouseFoundation/Warehouses/WarehouseApiTests.cs`.
- [X] T025 [P] [US1] Add warehouse uniqueness persistence tests in `Formica.Tests/Warehouse/WarehouseFoundation/Warehouses/WarehousePersistenceTests.cs`.
- [X] T026 [P] [US1] Add default Location Address Rules creation test in `Formica.Tests/Warehouse/WarehouseFoundation/Warehouses/DefaultLocationAddressRulesTests.cs`.
- [X] T027 [US1] Create `Warehouse` domain model in `Formica.ApiService/Warehouse/WarehouseFoundation/Domain/Warehouses/Warehouse.cs`.
- [X] T028 [US1] Create Warehouse EF configuration in `Formica.ApiService/Warehouse/Persistence/Configurations/WarehouseFoundation/WarehouseConfiguration.cs`.
- [X] T029 [US1] Create Location Address Rules EF configuration in `Formica.ApiService/Warehouse/Persistence/Configurations/WarehouseFoundation/LocationAddressRulesConfiguration.cs`.
- [X] T030 [US1] Create warehouse request/response DTOs in `Formica.ApiService/Warehouse/WarehouseFoundation/Contracts/Warehouses/`.
- [X] T031 [US1] Implement create warehouse feature in `Formica.ApiService/Warehouse/WarehouseFoundation/Features/Warehouses/CreateWarehouse.cs`.
- [X] T032 [US1] Implement list/get warehouse features in `Formica.ApiService/Warehouse/WarehouseFoundation/Features/Warehouses/`.
- [X] T033 [US1] Implement update warehouse feature in `Formica.ApiService/Warehouse/WarehouseFoundation/Features/Warehouses/UpdateWarehouse.cs`.
- [X] T034 [US1] Implement deactivate/reactivate warehouse features in `Formica.ApiService/Warehouse/WarehouseFoundation/Features/Warehouses/`.
- [X] T035 [US1] Implement warehouse endpoints in `Formica.ApiService/Warehouse/WarehouseFoundation/Endpoints/WarehouseEndpoints.cs`.
- [X] T036 [US1] Add Warehouse API client methods in `Formica.Web/Warehouse/WarehouseFoundation/ApiClients/WarehouseFoundationApiClient.cs`.
- [X] T037 [US1] Add basic Warehouse management page in `Formica.Web/Warehouse/WarehouseFoundation/Pages/Warehouses.razor`.
- [ ] T037A [US1] Migrate Warehouse API client methods and Warehouse management UI into `Formica.WebApp/Warehouse/WarehouseFoundation/`.

## Phase 4: M1 / US2 Zones

### Phase 4A: Zone domain and persistence

- [X] T038A [P] [US2] Add Zone domain lifecycle tests in `Formica.Tests/Warehouse/WarehouseFoundation/Zones/ZoneLifecycleTests.cs`.
- [X] T039A [P] [US2] Add Zone uniqueness persistence tests in `Formica.Tests/Warehouse/WarehouseFoundation/Zones/ZonePersistenceTests.cs`.
- [X] T041A [US2] Create `Zone` domain model in `Formica.ApiService/Warehouse/WarehouseFoundation/Domain/Zones/Zone.cs`.
- [X] T042A [US2] Create Zone EF configuration in `Formica.ApiService/Warehouse/Persistence/Configurations/WarehouseFoundation/ZoneConfiguration.cs`.
- [X] T042B [US2] Add Zone `DbSet` and model configuration registration to `Formica.ApiService/Warehouse/Persistence/WarehouseDbContext.cs`.
- [X] T042C [US2] Create explicit EF Core migration for Zone persistence.

### Phase 4B: Zone create/list/get API

- [X] T038B [P] [US2] Add Zone create/list/get API contract tests in `Formica.Tests/Warehouse/WarehouseFoundation/Zones/ZoneApiTests.cs`.
- [X] T043B [US2] Create Zone request/response DTOs in `Formica.ApiService/Warehouse/WarehouseFoundation/Contracts/Zones/`.
- [X] T044B [US2] Implement create zone feature in `Formica.ApiService/Warehouse/WarehouseFoundation/Features/Zones/CreateZone.cs`.
- [X] T045B [US2] Implement list/get zone features in `Formica.ApiService/Warehouse/WarehouseFoundation/Features/Zones/`.
- [X] T048B [US2] Implement create/list/get zone endpoints in `Formica.ApiService/Warehouse/WarehouseFoundation/Endpoints/ZoneEndpoints.cs`.

### Phase 4C: Zone update and lifecycle API

- [X] T038C [P] [US2] Add Zone update/deactivate/reactivate API tests in `Formica.Tests/Warehouse/WarehouseFoundation/Zones/ZoneLifecycleApiTests.cs`.
- [X] T046C [US2] Implement update zone feature in `Formica.ApiService/Warehouse/WarehouseFoundation/Features/Zones/UpdateZone.cs`.
- [X] T047C [US2] Implement deactivate/reactivate zone features in `Formica.ApiService/Warehouse/WarehouseFoundation/Features/Zones/`.
- [X] T048C [US2] Add update/deactivate/reactivate routes to `Formica.ApiService/Warehouse/WarehouseFoundation/Endpoints/ZoneEndpoints.cs`.

### Phase 4D: Zone UI/client

- [ ] T049D [US2] Add Zone API client methods in `Formica.WebApp/Warehouse/WarehouseFoundation/ApiClients/WarehouseFoundationApiClient.cs`.
- [ ] T050D [US2] Add MudBlazor Zone management component in `Formica.WebApp/Warehouse/WarehouseFoundation/Components/ZonesPanel.razor`.

### Phase 4E: M1 consistency pass

- [ ] T050E [US2] Run `dotnet build .\Formica.slnx -m:1` and fix build errors.
- [ ] T050F [US2] Run `dotnet test .\Formica.Tests\Formica.Tests.csproj` and fix test failures if tests are explicitly allowed.
- [ ] T050G [US2] Verify `quickstart.md` Scenarios 4 and 5.
- [ ] T050H [US2] Review `contracts/api-contracts.md` and implementation for consistency.
- [ ] T050I [US2] Confirm no out-of-scope Storage Location, Warehouse Layout, SKU, Inventory, Receiving, Putaway, LPN, scanner, or integration workflow was implemented.

## Phase 5: M2 / US3 Storage Locations

### Phase 5A: Storage Location domain and persistence

- [ ] T052 [P] [US3] Add storage location address uniqueness tests in `Formica.Tests/Warehouse/WarehouseFoundation/StorageLocations/StorageLocationAddressUniquenessTests.cs`.
- [ ] T053 [P] [US3] Add address normalization tests in `Formica.Tests/Warehouse/WarehouseFoundation/StorageLocations/LocationAddressNormalizationTests.cs`.
- [ ] T054 [P] [US3] Add storage location lifecycle tests in `Formica.Tests/Warehouse/WarehouseFoundation/StorageLocations/StorageLocationLifecycleTests.cs`.
- [ ] T055 [US3] Create `StorageLocation` domain model in `Formica.ApiService/Warehouse/WarehouseFoundation/Domain/StorageLocations/StorageLocation.cs`.
- [ ] T056 [US3] Create Storage Location EF configuration in `Formica.ApiService/Warehouse/Persistence/Configurations/WarehouseFoundation/StorageLocationConfiguration.cs`.
- [ ] T057 [US3] Add normalized address persistence support in Storage Location domain/configuration.

### Phase 5B: Storage Location create/list/get API

- [ ] T051 [P] [US3] Add storage location create/list/get API contract tests in `Formica.Tests/Warehouse/WarehouseFoundation/StorageLocations/StorageLocationApiTests.cs`.
- [ ] T058 [US3] Create storage location request/response DTOs in `Formica.ApiService/Warehouse/WarehouseFoundation/Contracts/StorageLocations/`.
- [ ] T059 [US3] Implement create storage location feature in `Formica.ApiService/Warehouse/WarehouseFoundation/Features/StorageLocations/CreateStorageLocation.cs`.
- [ ] T060 [US3] Implement list/get storage location features in `Formica.ApiService/Warehouse/WarehouseFoundation/Features/StorageLocations/`.

### Phase 5C: Storage Location update and lifecycle API

- [ ] T061 [US3] Implement update storage location feature in `Formica.ApiService/Warehouse/WarehouseFoundation/Features/StorageLocations/UpdateStorageLocation.cs`.
- [ ] T062 [US3] Implement deactivate/reactivate storage location features in `Formica.ApiService/Warehouse/WarehouseFoundation/Features/StorageLocations/`.
- [ ] T063 [US3] Implement storage location endpoints in `Formica.ApiService/Warehouse/WarehouseFoundation/Endpoints/StorageLocationEndpoints.cs`.

### Phase 5D: Storage Location UI/client

- [ ] T064 [US3] Add Storage Location API client methods in `Formica.WebApp/Warehouse/WarehouseFoundation/ApiClients/WarehouseFoundationApiClient.cs`.
- [ ] T065 [US3] Add MudBlazor Storage Location management component in `Formica.WebApp/Warehouse/WarehouseFoundation/Components/StorageLocationsPanel.razor`.

### Phase 5E: M2 consistency pass

- [ ] T065A [US3] Verify quickstart Scenarios 6 and 7, review contracts, and confirm no out-of-scope SKU, Warehouse Layout, Inventory, Receiving, Putaway, LPN, scanner, or integration workflow was implemented.

## Phase 6: M3 / US4 SKUs

### Phase 6A: Unit of Measure seed/list

- [ ] T066 [P] [US4] Add Unit of Measure API tests in `Formica.Tests/Warehouse/WarehouseFoundation/UnitsOfMeasure/UnitOfMeasureApiTests.cs`.
- [ ] T071 [US4] Create `UnitOfMeasure` domain model in `Formica.ApiService/Warehouse/WarehouseFoundation/Domain/UnitsOfMeasure/UnitOfMeasure.cs`.
- [ ] T074 [US4] Create Unit of Measure EF configuration in `Formica.ApiService/Warehouse/Persistence/Configurations/WarehouseFoundation/UnitOfMeasureConfiguration.cs`.
- [ ] T077 [US4] Add seeded read-only Unit of Measure data in `Formica.ApiService/Warehouse/Persistence/WarehouseFoundationSeedData.cs`.
- [ ] T078 [US4] Create Unit of Measure DTOs and list feature in `Formica.ApiService/Warehouse/WarehouseFoundation/Contracts/UnitsOfMeasure/` and `Formica.ApiService/Warehouse/WarehouseFoundation/Features/UnitsOfMeasure/`.

### Phase 6B: SKU and barcode domain and persistence

- [ ] T068 [P] [US4] Add SKU uniqueness persistence tests in `Formica.Tests/Warehouse/WarehouseFoundation/Skus/SkuPersistenceTests.cs`.
- [ ] T069 [P] [US4] Add barcode uniqueness tests in `Formica.Tests/Warehouse/WarehouseFoundation/Skus/SkuBarcodeUniquenessTests.cs`.
- [ ] T070 [P] [US4] Add SKU lifecycle tests in `Formica.Tests/Warehouse/WarehouseFoundation/Skus/SkuLifecycleTests.cs`.
- [ ] T072 [US4] Create `Sku` domain model in `Formica.ApiService/Warehouse/WarehouseFoundation/Domain/Skus/Sku.cs`.
- [ ] T073 [US4] Create `SkuBarcode` domain model in `Formica.ApiService/Warehouse/WarehouseFoundation/Domain/Skus/SkuBarcode.cs`.
- [ ] T075 [US4] Create SKU EF configuration in `Formica.ApiService/Warehouse/Persistence/Configurations/WarehouseFoundation/SkuConfiguration.cs`.
- [ ] T076 [US4] Create SKU Barcode EF configuration in `Formica.ApiService/Warehouse/Persistence/Configurations/WarehouseFoundation/SkuBarcodeConfiguration.cs`.

### Phase 6C: SKU and barcode API

- [ ] T067 [P] [US4] Add SKU API contract tests in `Formica.Tests/Warehouse/WarehouseFoundation/Skus/SkuApiTests.cs`.
- [ ] T079 [US4] Create SKU request/response DTOs in `Formica.ApiService/Warehouse/WarehouseFoundation/Contracts/Skus/`.
- [ ] T080 [US4] Implement create SKU feature in `Formica.ApiService/Warehouse/WarehouseFoundation/Features/Skus/CreateSku.cs`.
- [ ] T081 [US4] Implement list/get SKU features in `Formica.ApiService/Warehouse/WarehouseFoundation/Features/Skus/`.
- [ ] T082 [US4] Implement update SKU feature in `Formica.ApiService/Warehouse/WarehouseFoundation/Features/Skus/UpdateSku.cs`.
- [ ] T083 [US4] Implement deactivate/reactivate SKU features in `Formica.ApiService/Warehouse/WarehouseFoundation/Features/Skus/`.
- [ ] T084 [US4] Implement Unit of Measure endpoints in `Formica.ApiService/Warehouse/WarehouseFoundation/Endpoints/UnitOfMeasureEndpoints.cs`.
- [ ] T085 [US4] Implement SKU endpoints in `Formica.ApiService/Warehouse/WarehouseFoundation/Endpoints/SkuEndpoints.cs`.

### Phase 6D: SKU UI/client

- [ ] T086 [US4] Add SKU and Unit of Measure API client methods in `Formica.WebApp/Warehouse/WarehouseFoundation/ApiClients/WarehouseFoundationApiClient.cs`.
- [ ] T087 [US4] Add MudBlazor SKU management page in `Formica.WebApp/Warehouse/WarehouseFoundation/Pages/Skus.razor`.

### Phase 6E: M3 consistency pass

- [ ] T087A [US4] Verify quickstart Scenarios 8, 9, 10, 13, and 15, review contracts, and confirm no out-of-scope Inventory, Receiving, Putaway, LPN, scanner, or integration workflow was implemented.

## Phase 7: M4 / US5 Warehouse Layout

- [ ] T088 [P] [US5] Add Warehouse Layout API tests in `Formica.Tests/Warehouse/WarehouseFoundation/WarehouseLayout/WarehouseLayoutApiTests.cs`.
- [ ] T089 [P] [US5] Add setup readiness tests in `Formica.Tests/Warehouse/WarehouseFoundation/WarehouseLayout/SetupReadinessTests.cs`.
- [ ] T090 [US5] Create Warehouse Layout response DTOs in `Formica.ApiService/Warehouse/WarehouseFoundation/Contracts/WarehouseLayout/`.
- [ ] T091 [US5] Implement Warehouse Layout query feature in `Formica.ApiService/Warehouse/WarehouseFoundation/Features/WarehouseLayout/GetWarehouseLayout.cs`.
- [ ] T092 [US5] Implement setup readiness calculation in `Formica.ApiService/Warehouse/WarehouseFoundation/Features/WarehouseLayout/CalculateSetupReadiness.cs`.
- [ ] T093 [US5] Implement Warehouse Layout endpoint in `Formica.ApiService/Warehouse/WarehouseFoundation/Endpoints/WarehouseLayoutEndpoints.cs`.
- [ ] T094 [US5] Add Warehouse Layout API client method in `Formica.WebApp/Warehouse/WarehouseFoundation/ApiClients/WarehouseFoundationApiClient.cs`.
- [ ] T095 [US5] Add MudBlazor Warehouse Layout page in `Formica.WebApp/Warehouse/WarehouseFoundation/Pages/WarehouseLayout.razor`.
- [ ] T096 [US5] Add MudBlazor setup readiness UI component in `Formica.WebApp/Warehouse/WarehouseFoundation/Components/SetupReadinessPanel.razor`.

## Phase 8: M4 / US6 Stable References

- [ ] T097 [P] [US6] Add stable identity tests in `Formica.Tests/Warehouse/WarehouseFoundation/Lifecycle/StableIdentityTests.cs`.
- [ ] T098 [P] [US6] Add `includeInactive` filtering tests in `Formica.Tests/Warehouse/WarehouseFoundation/Lifecycle/IncludeInactiveTests.cs`.
- [ ] T099 [P] [US6] Add lifecycle command POST semantics tests in `Formica.Tests/Warehouse/WarehouseFoundation/Lifecycle/LifecycleCommandTests.cs`.
- [ ] T100 [US6] Ensure all list/detail response DTOs include `isActive` in `Formica.ApiService/Warehouse/WarehouseFoundation/Contracts/`.
- [ ] T101 [US6] Ensure all list features apply `includeInactive=false` by default in `Formica.ApiService/Warehouse/WarehouseFoundation/Features/`.
- [ ] T102 [US6] Ensure all reactivation features rerun uniqueness and validation checks in `Formica.ApiService/Warehouse/WarehouseFoundation/Features/`.
- [ ] T103 [US6] Ensure UI surfaces inactive state in `Formica.WebApp/Warehouse/WarehouseFoundation/Pages/` and `Formica.WebApp/Warehouse/WarehouseFoundation/Components/`.

## Phase 9: M4 / Polish and Cross-Cutting Validation

- [ ] T104 [P] Run `dotnet format` for `Formica.slnx` and fix formatting issues.
- [ ] T105 Run `dotnet build Formica.slnx` and fix build errors.
- [ ] T106 Run Warehouse Foundation tests in `Formica.Tests/Warehouse/WarehouseFoundation/` and fix failures if tests are explicitly allowed.
- [ ] T107 Validate `specs/001-warehouse-foundation/quickstart.md` manually or through tests.
- [ ] T108 [P] Update `specs/001-warehouse-foundation/quickstart.md` if implementation behavior differs from accepted design decisions.
- [ ] T109 [P] Review `specs/001-warehouse-foundation/contracts/api-contracts.md` against generated OpenAPI output if available.
- [ ] T110 Confirm no out-of-scope workflows were implemented: Inventory, Receiving, Putaway, LPN, barcode scanning, 1C sync, mobile client.

## Dependencies and Execution Order

### Accepted Milestone Path

1. Complete Phase 1 Setup.
2. Complete Phase 2 Foundational.
3. Complete M1: `Formica.WebApp` foundation, US1 Warehouse management migration, and US2 Zone management.
4. Stop and validate M1 before expanding the UI surface.
5. Complete M2: Storage Locations and configured address behavior.
6. Complete M3: SKU, Unit of Measure, and barcode behavior.
7. Complete M4: Warehouse Layout, lifecycle consistency, and final cross-cutting validation.

### Key Dependencies

- M1 / US1 depends on Phase 2.
- M1 / US2 depends on Phase 2 and uses Warehouse data from US1.
- M2 / US3 depends on accepted M1.
- M3 / US4 depends on Phase 2 and can proceed after US1, but is accepted separately.
- M4 / US5 depends on accepted M2.
- M4 / US6 depends on US1 through US5.
- UI tasks wait for corresponding API contracts and behavior.

## Notes

- `implementation-guidelines.md` is the authoritative guardrail source.
- `Formica.WebApp` is the target UI project; `Formica.Web` is a temporary Bootstrap migration source only.
- Commit after each task or coherent task group.
