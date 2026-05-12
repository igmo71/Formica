# Tasks: Warehouse Foundation

**Input**: Design documents from `specs/001-warehouse-foundation/`  
**Prerequisites**: `spec.md`, `plan.md`, `research.md`, `data-model.md`, `contracts/api-contracts.md`, `quickstart.md`  
**Branch**: `001-warehouse-foundation`

**Tests**: Included. The specification and quickstart require validation of user-visible behavior, uniqueness rules, lifecycle behavior, and API/UI readiness.

**Organization**: Tasks are grouped by phases and user stories so each story can be implemented and validated incrementally.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel because it touches different files or independent areas.
- **[Story]**: User story from `spec.md` when applicable.
- Tasks include concrete file paths or concrete target folders where final file decomposition is intentionally left to implementation.

## Phase 1: Setup

**Purpose**: Prepare the existing Aspire solution for Warehouse Foundation implementation.

- [ ] T001 Add PostgreSQL Aspire hosting package to `Formica.AppHost/Formica.AppHost.csproj`.
- [ ] T002 Add EF Core Npgsql provider and EF Core design package references to `Formica.ApiService/Formica.ApiService.csproj`.
- [ ] T003 Configure PostgreSQL resource and API service connection in `Formica.AppHost/Program.cs`.
- [ ] T004 Create Warehouse module folders under `Formica.ApiService/Warehouse/`.
- [ ] T005 Create Warehouse Foundation UI folders under `Formica.Web/Warehouse/WarehouseFoundation/`.
- [ ] T006 Create Warehouse Foundation test folders under `Formica.Tests/Warehouse/WarehouseFoundation/`.

---

## Phase 2: Foundational

**Purpose**: Implement shared model, persistence, validation, routing, and test infrastructure required by all user stories.

**Critical**: No user story implementation should begin until this phase is complete.

### Domain and Persistence Foundation

- [ ] T007 [P] Create lifecycle base abstractions or helper types in `Formica.ApiService/Warehouse/WarehouseFoundation/Domain/Common/`.
- [ ] T008 [P] Create controlled value definitions for zone and storage location purposes in `Formica.ApiService/Warehouse/WarehouseFoundation/Domain/Common/`.
- [ ] T009 [P] Create `StorageLocationCapacity` value-object-style type in `Formica.ApiService/Warehouse/WarehouseFoundation/Domain/StorageLocations/StorageLocationCapacity.cs`.
- [ ] T010 [P] Create `LocationAddressRules` domain model in `Formica.ApiService/Warehouse/WarehouseFoundation/Domain/LocationAddressing/LocationAddressRules.cs`.
- [ ] T011 Create `WarehouseDbContext` in `Formica.ApiService/Warehouse/Persistence/WarehouseDbContext.cs`.
- [ ] T012 Create EF Core configuration folder `Formica.ApiService/Warehouse/Persistence/Configurations/WarehouseFoundation/`.
- [ ] T013 Create Warehouse persistence registration extension in `Formica.ApiService/Warehouse/Persistence/WarehousePersistenceServiceCollectionExtensions.cs`.
- [ ] T014 Register Warehouse persistence and Warehouse Foundation endpoints in `Formica.ApiService/Program.cs`.
- [ ] T015 Create initial EF Core migration for Warehouse Foundation under `Formica.ApiService/Warehouse/Persistence/Migrations/`.

### API Foundation

- [ ] T016 Create endpoint group registration entry point in `Formica.ApiService/Warehouse/WarehouseFoundation/Endpoints/WarehouseFoundationEndpoints.cs`.
- [ ] T017 [P] Create shared API result/problem helpers in `Formica.ApiService/Warehouse/WarehouseFoundation/Endpoints/EndpointResults.cs`.
- [ ] T018 [P] Create shared contract DTO folder and common DTOs in `Formica.ApiService/Warehouse/WarehouseFoundation/Contracts/`.
- [ ] T019 [P] Create common validation helpers in `Formica.ApiService/Warehouse/WarehouseFoundation/Features/Common/Validation/`.
- [ ] T020 [P] Create address normalization helper in `Formica.ApiService/Warehouse/WarehouseFoundation/Features/StorageLocations/NormalizeLocationAddress.cs`.

### Test Foundation

- [ ] T021 Create Aspire-backed API test fixture in `Formica.Tests/Warehouse/WarehouseFoundation/WarehouseFoundationApiFixture.cs`.
- [ ] T022 Create PostgreSQL-backed persistence test fixture in `Formica.Tests/Warehouse/WarehouseFoundation/WarehousePersistenceFixture.cs`.
- [ ] T023 [P] Create test data builders in `Formica.Tests/Warehouse/WarehouseFoundation/TestData/WarehouseFoundationTestData.cs`.

**Checkpoint**: Persistence, endpoint registration, shared validation, and test fixtures are ready.

---

## Phase 3: User Story 1 - Create a warehouse (Priority: P1) MVP

**Goal**: A warehouse administrator can create, view, update, deactivate, and reactivate warehouses with unique warehouse codes.

**Independent Test**: Create a warehouse, retrieve it by id, list it, reject duplicate code, update editable attributes, deactivate/reactivate while preserving identity.

### Tests for User Story 1

- [ ] T024 [P] [US1] Add warehouse API contract tests in `Formica.Tests/Warehouse/WarehouseFoundation/Warehouses/WarehouseApiTests.cs`.
- [ ] T025 [P] [US1] Add warehouse uniqueness persistence tests in `Formica.Tests/Warehouse/WarehouseFoundation/Warehouses/WarehousePersistenceTests.cs`.
- [ ] T026 [P] [US1] Add default Location Address Rules creation test in `Formica.Tests/Warehouse/WarehouseFoundation/Warehouses/DefaultLocationAddressRulesTests.cs`.

### Implementation for User Story 1

- [ ] T027 [US1] Create `Warehouse` domain model in `Formica.ApiService/Warehouse/WarehouseFoundation/Domain/Warehouses/Warehouse.cs`.
- [ ] T028 [US1] Create Warehouse EF configuration in `Formica.ApiService/Warehouse/Persistence/Configurations/WarehouseFoundation/WarehouseConfiguration.cs`.
- [ ] T029 [US1] Create Location Address Rules EF configuration in `Formica.ApiService/Warehouse/Persistence/Configurations/WarehouseFoundation/LocationAddressRulesConfiguration.cs`.
- [ ] T030 [US1] Create warehouse request/response DTOs in `Formica.ApiService/Warehouse/WarehouseFoundation/Contracts/Warehouses/`.
- [ ] T031 [US1] Implement create warehouse feature in `Formica.ApiService/Warehouse/WarehouseFoundation/Features/Warehouses/CreateWarehouse.cs`.
- [ ] T032 [US1] Implement list/get warehouse features in `Formica.ApiService/Warehouse/WarehouseFoundation/Features/Warehouses/`.
- [ ] T033 [US1] Implement update warehouse feature in `Formica.ApiService/Warehouse/WarehouseFoundation/Features/Warehouses/UpdateWarehouse.cs`.
- [ ] T034 [US1] Implement deactivate/reactivate warehouse features in `Formica.ApiService/Warehouse/WarehouseFoundation/Features/Warehouses/`.
- [ ] T035 [US1] Implement warehouse endpoints in `Formica.ApiService/Warehouse/WarehouseFoundation/Endpoints/WarehouseEndpoints.cs`.
- [ ] T036 [US1] Add Warehouse API client methods in `Formica.Web/Warehouse/WarehouseFoundation/ApiClients/WarehouseFoundationApiClient.cs`.
- [ ] T037 [US1] Add basic Warehouse management page in `Formica.Web/Warehouse/WarehouseFoundation/Pages/Warehouses.razor`.

**Checkpoint**: US1 works independently through API and has initial UI surface.

---

## Phase 4: User Story 2 - Create warehouse zones (Priority: P1)

**Goal**: A warehouse administrator can create and maintain zones inside a warehouse, with zone codes unique within that warehouse.

**Independent Test**: Create two zones in a warehouse, reject duplicate zone code in the same warehouse, allow same zone code in another warehouse, update/deactivate/reactivate zone.

### Tests for User Story 2

- [ ] T038 [P] [US2] Add zone API contract tests in `Formica.Tests/Warehouse/WarehouseFoundation/Zones/ZoneApiTests.cs`.
- [ ] T039 [P] [US2] Add zone uniqueness persistence tests in `Formica.Tests/Warehouse/WarehouseFoundation/Zones/ZonePersistenceTests.cs`.
- [ ] T040 [P] [US2] Add zone lifecycle tests in `Formica.Tests/Warehouse/WarehouseFoundation/Zones/ZoneLifecycleTests.cs`.

### Implementation for User Story 2

- [ ] T041 [US2] Create `Zone` domain model in `Formica.ApiService/Warehouse/WarehouseFoundation/Domain/Zones/Zone.cs`.
- [ ] T042 [US2] Create Zone EF configuration in `Formica.ApiService/Warehouse/Persistence/Configurations/WarehouseFoundation/ZoneConfiguration.cs`.
- [ ] T043 [US2] Create zone request/response DTOs in `Formica.ApiService/Warehouse/WarehouseFoundation/Contracts/Zones/`.
- [ ] T044 [US2] Implement create zone feature in `Formica.ApiService/Warehouse/WarehouseFoundation/Features/Zones/CreateZone.cs`.
- [ ] T045 [US2] Implement list/get zone features in `Formica.ApiService/Warehouse/WarehouseFoundation/Features/Zones/`.
- [ ] T046 [US2] Implement update zone feature in `Formica.ApiService/Warehouse/WarehouseFoundation/Features/Zones/UpdateZone.cs`.
- [ ] T047 [US2] Implement deactivate/reactivate zone features in `Formica.ApiService/Warehouse/WarehouseFoundation/Features/Zones/`.
- [ ] T048 [US2] Implement zone endpoints in `Formica.ApiService/Warehouse/WarehouseFoundation/Endpoints/ZoneEndpoints.cs`.
- [ ] T049 [US2] Add Zone API client methods in `Formica.Web/Warehouse/WarehouseFoundation/ApiClients/WarehouseFoundationApiClient.cs`.
- [ ] T050 [US2] Add Zone management component in `Formica.Web/Warehouse/WarehouseFoundation/Components/ZonesPanel.razor`.

**Checkpoint**: US1 and US2 work independently and together.

---

## Phase 5: User Story 3 - Create storage locations with configured addresses (Priority: P1)

**Goal**: A warehouse administrator can create and maintain storage locations inside zones with validated, normalized, warehouse-unique addresses.

**Independent Test**: Create a storage location, normalize address, reject duplicate normalized address in the same warehouse, allow same address in another warehouse, validate zone belongs to warehouse, update/deactivate/reactivate location.

### Tests for User Story 3

- [ ] T051 [P] [US3] Add storage location API contract tests in `Formica.Tests/Warehouse/WarehouseFoundation/StorageLocations/StorageLocationApiTests.cs`.
- [ ] T052 [P] [US3] Add storage location address uniqueness tests in `Formica.Tests/Warehouse/WarehouseFoundation/StorageLocations/StorageLocationAddressUniquenessTests.cs`.
- [ ] T053 [P] [US3] Add address normalization tests in `Formica.Tests/Warehouse/WarehouseFoundation/StorageLocations/LocationAddressNormalizationTests.cs`.
- [ ] T054 [P] [US3] Add storage location lifecycle tests in `Formica.Tests/Warehouse/WarehouseFoundation/StorageLocations/StorageLocationLifecycleTests.cs`.

### Implementation for User Story 3

- [ ] T055 [US3] Create `StorageLocation` domain model in `Formica.ApiService/Warehouse/WarehouseFoundation/Domain/StorageLocations/StorageLocation.cs`.
- [ ] T056 [US3] Create Storage Location EF configuration in `Formica.ApiService/Warehouse/Persistence/Configurations/WarehouseFoundation/StorageLocationConfiguration.cs`.
- [ ] T057 [US3] Add normalized address persistence support in `Formica.ApiService/Warehouse/WarehouseFoundation/Domain/StorageLocations/StorageLocation.cs` and `Formica.ApiService/Warehouse/Persistence/Configurations/WarehouseFoundation/StorageLocationConfiguration.cs`.
- [ ] T058 [US3] Create storage location request/response DTOs in `Formica.ApiService/Warehouse/WarehouseFoundation/Contracts/StorageLocations/`.
- [ ] T059 [US3] Implement create storage location feature in `Formica.ApiService/Warehouse/WarehouseFoundation/Features/StorageLocations/CreateStorageLocation.cs`.
- [ ] T060 [US3] Implement list/get storage location features in `Formica.ApiService/Warehouse/WarehouseFoundation/Features/StorageLocations/`.
- [ ] T061 [US3] Implement update storage location feature in `Formica.ApiService/Warehouse/WarehouseFoundation/Features/StorageLocations/UpdateStorageLocation.cs`.
- [ ] T062 [US3] Implement deactivate/reactivate storage location features in `Formica.ApiService/Warehouse/WarehouseFoundation/Features/StorageLocations/`.
- [ ] T063 [US3] Implement storage location endpoints in `Formica.ApiService/Warehouse/WarehouseFoundation/Endpoints/StorageLocationEndpoints.cs`.
- [ ] T064 [US3] Add Storage Location API client methods in `Formica.Web/Warehouse/WarehouseFoundation/ApiClients/WarehouseFoundationApiClient.cs`.
- [ ] T065 [US3] Add Storage Location management component in `Formica.Web/Warehouse/WarehouseFoundation/Components/StorageLocationsPanel.razor`.

**Checkpoint**: P1 warehouse setup path works through Warehouse → Zone → Storage Location.

---

## Phase 6: User Story 4 - Maintain basic SKUs (Priority: P1)

**Goal**: A warehouse administrator can create and maintain basic SKUs with base unit of measure and multiple unique barcode values.

**Independent Test**: Seed units of measure, create SKU, add multiple barcodes, reject duplicate SKU code, reject duplicate barcode across SKUs, update/deactivate/reactivate SKU.

### Tests for User Story 4

- [ ] T066 [P] [US4] Add Unit of Measure API tests in `Formica.Tests/Warehouse/WarehouseFoundation/UnitsOfMeasure/UnitOfMeasureApiTests.cs`.
- [ ] T067 [P] [US4] Add SKU API contract tests in `Formica.Tests/Warehouse/WarehouseFoundation/Skus/SkuApiTests.cs`.
- [ ] T068 [P] [US4] Add SKU uniqueness persistence tests in `Formica.Tests/Warehouse/WarehouseFoundation/Skus/SkuPersistenceTests.cs`.
- [ ] T069 [P] [US4] Add barcode uniqueness tests in `Formica.Tests/Warehouse/WarehouseFoundation/Skus/SkuBarcodeUniquenessTests.cs`.
- [ ] T070 [P] [US4] Add SKU lifecycle tests in `Formica.Tests/Warehouse/WarehouseFoundation/Skus/SkuLifecycleTests.cs`.

### Implementation for User Story 4

- [ ] T071 [US4] Create `UnitOfMeasure` domain model in `Formica.ApiService/Warehouse/WarehouseFoundation/Domain/UnitsOfMeasure/UnitOfMeasure.cs`.
- [ ] T072 [US4] Create `Sku` domain model in `Formica.ApiService/Warehouse/WarehouseFoundation/Domain/Skus/Sku.cs`.
- [ ] T073 [US4] Create `SkuBarcode` domain model in `Formica.ApiService/Warehouse/WarehouseFoundation/Domain/Skus/SkuBarcode.cs`.
- [ ] T074 [US4] Create Unit of Measure EF configuration in `Formica.ApiService/Warehouse/Persistence/Configurations/WarehouseFoundation/UnitOfMeasureConfiguration.cs`.
- [ ] T075 [US4] Create SKU EF configuration in `Formica.ApiService/Warehouse/Persistence/Configurations/WarehouseFoundation/SkuConfiguration.cs`.
- [ ] T076 [US4] Create SKU Barcode EF configuration in `Formica.ApiService/Warehouse/Persistence/Configurations/WarehouseFoundation/SkuBarcodeConfiguration.cs`.
- [ ] T077 [US4] Add seeded read-only Unit of Measure data in `Formica.ApiService/Warehouse/Persistence/WarehouseFoundationSeedData.cs`.
- [ ] T078 [US4] Create Unit of Measure DTOs and list feature in `Formica.ApiService/Warehouse/WarehouseFoundation/Contracts/UnitsOfMeasure/` and `Formica.ApiService/Warehouse/WarehouseFoundation/Features/UnitsOfMeasure/`.
- [ ] T079 [US4] Create SKU request/response DTOs in `Formica.ApiService/Warehouse/WarehouseFoundation/Contracts/Skus/`.
- [ ] T080 [US4] Implement create SKU feature in `Formica.ApiService/Warehouse/WarehouseFoundation/Features/Skus/CreateSku.cs`.
- [ ] T081 [US4] Implement list/get SKU features in `Formica.ApiService/Warehouse/WarehouseFoundation/Features/Skus/`.
- [ ] T082 [US4] Implement update SKU feature in `Formica.ApiService/Warehouse/WarehouseFoundation/Features/Skus/UpdateSku.cs`.
- [ ] T083 [US4] Implement deactivate/reactivate SKU features in `Formica.ApiService/Warehouse/WarehouseFoundation/Features/Skus/`.
- [ ] T084 [US4] Implement Unit of Measure endpoints in `Formica.ApiService/Warehouse/WarehouseFoundation/Endpoints/UnitOfMeasureEndpoints.cs`.
- [ ] T085 [US4] Implement SKU endpoints in `Formica.ApiService/Warehouse/WarehouseFoundation/Endpoints/SkuEndpoints.cs`.
- [ ] T086 [US4] Add SKU and Unit of Measure API client methods in `Formica.Web/Warehouse/WarehouseFoundation/ApiClients/WarehouseFoundationApiClient.cs`.
- [ ] T087 [US4] Add SKU management page in `Formica.Web/Warehouse/WarehouseFoundation/Pages/Skus.razor`.

**Checkpoint**: All P1 user stories work independently.

---

## Phase 7: User Story 5 - View basic warehouse layout (Priority: P2)

**Goal**: An operations manager can view Warehouse → Zones → Storage Locations with active/inactive state and setup readiness indicators.

**Independent Test**: Create warehouse setup data, retrieve layout, verify hierarchy, `isActive` values, incomplete setup state, and setup readiness indicators.

### Tests for User Story 5

- [ ] T088 [P] [US5] Add Warehouse Layout API tests in `Formica.Tests/Warehouse/WarehouseFoundation/WarehouseLayout/WarehouseLayoutApiTests.cs`.
- [ ] T089 [P] [US5] Add setup readiness tests in `Formica.Tests/Warehouse/WarehouseFoundation/WarehouseLayout/SetupReadinessTests.cs`.

### Implementation for User Story 5

- [ ] T090 [US5] Create Warehouse Layout response DTOs in `Formica.ApiService/Warehouse/WarehouseFoundation/Contracts/WarehouseLayout/`.
- [ ] T091 [US5] Implement Warehouse Layout query feature in `Formica.ApiService/Warehouse/WarehouseFoundation/Features/WarehouseLayout/GetWarehouseLayout.cs`.
- [ ] T092 [US5] Implement setup readiness calculation in `Formica.ApiService/Warehouse/WarehouseFoundation/Features/WarehouseLayout/CalculateSetupReadiness.cs`.
- [ ] T093 [US5] Implement Warehouse Layout endpoint in `Formica.ApiService/Warehouse/WarehouseFoundation/Endpoints/WarehouseLayoutEndpoints.cs`.
- [ ] T094 [US5] Add Warehouse Layout API client method in `Formica.Web/Warehouse/WarehouseFoundation/ApiClients/WarehouseFoundationApiClient.cs`.
- [ ] T095 [US5] Add Warehouse Layout page in `Formica.Web/Warehouse/WarehouseFoundation/Pages/WarehouseLayout.razor`.
- [ ] T096 [US5] Add setup readiness UI component in `Formica.Web/Warehouse/WarehouseFoundation/Components/SetupReadinessPanel.razor`.

**Checkpoint**: Warehouse Layout supports setup verification.

---

## Phase 8: User Story 6 - Maintain stable references for future inventory workflows (Priority: P2)

**Goal**: Foundation records preserve stable identity across update, deactivate, and reactivate operations and expose lifecycle state consistently.

**Independent Test**: For warehouse, zone, storage location, and SKU, update editable fields, deactivate/reactivate, verify id remains unchanged and `isActive` is returned in list/detail/layout responses.

### Tests for User Story 6

- [ ] T097 [P] [US6] Add stable identity tests in `Formica.Tests/Warehouse/WarehouseFoundation/Lifecycle/StableIdentityTests.cs`.
- [ ] T098 [P] [US6] Add `includeInactive` filtering tests in `Formica.Tests/Warehouse/WarehouseFoundation/Lifecycle/IncludeInactiveTests.cs`.
- [ ] T099 [P] [US6] Add lifecycle command POST semantics tests in `Formica.Tests/Warehouse/WarehouseFoundation/Lifecycle/LifecycleCommandTests.cs`.

### Implementation for User Story 6

- [ ] T100 [US6] Ensure all list/detail response DTOs include `isActive` in `Formica.ApiService/Warehouse/WarehouseFoundation/Contracts/`.
- [ ] T101 [US6] Ensure all list features apply `includeInactive=false` by default in `Formica.ApiService/Warehouse/WarehouseFoundation/Features/`.
- [ ] T102 [US6] Ensure all reactivation features rerun uniqueness and validation checks in `Formica.ApiService/Warehouse/WarehouseFoundation/Features/`.
- [ ] T103 [US6] Ensure UI surfaces inactive state in `Formica.Web/Warehouse/WarehouseFoundation/Pages/` and `Formica.Web/Warehouse/WarehouseFoundation/Components/`.

**Checkpoint**: Lifecycle semantics are consistent across Warehouse Foundation.

---

## Phase 9: Polish and Cross-Cutting Validation

**Purpose**: Final consistency, validation, documentation, and quickstart verification. Polish means final cleanup and presentation-quality finishing work, not new feature scope.

- [ ] T104 [P] Run `dotnet format` for `Formica.slnx` and fix formatting issues.
- [ ] T105 Run `dotnet build Formica.slnx` and fix build errors.
- [ ] T106 Run Warehouse Foundation tests in `Formica.Tests/Warehouse/WarehouseFoundation/` and fix failures.
- [ ] T107 Validate `specs/001-warehouse-foundation/quickstart.md` manually or through tests.
- [ ] T108 [P] Update `specs/001-warehouse-foundation/quickstart.md` if implementation behavior differs from accepted design decisions.
- [ ] T109 [P] Review `specs/001-warehouse-foundation/contracts/api-contracts.md` against generated OpenAPI output if available.
- [ ] T110 Confirm no out-of-scope workflows were implemented: Inventory, Receiving, Putaway, LPN, barcode scanning, 1C sync, mobile client.

---

## Dependencies and Execution Order

### Phase Dependencies

- **Phase 1 Setup**: no dependencies.
- **Phase 2 Foundational**: depends on Phase 1.
- **US1 Warehouses**: depends on Phase 2.
- **US2 Zones**: depends on Phase 2 and uses Warehouse data from US1 for end-to-end validation.
- **US3 Storage Locations**: depends on US1 and US2.
- **US4 SKUs**: depends on Phase 2 and can run after US1 if persistence foundation is ready.
- **US5 Warehouse Layout**: depends on US1, US2, and US3.
- **US6 Stable References**: depends on US1 through US5 implementations.
- **Polish**: depends on selected user stories being complete.

### MVP Path

1. Complete Phase 1 Setup.
2. Complete Phase 2 Foundational.
3. Complete US1 Warehouses.
4. Complete US2 Zones.
5. Complete US3 Storage Locations.
6. Complete US4 SKUs.
7. Stop and validate all P1 stories.

### Parallel Opportunities

- T007-T010 can run in parallel.
- T017-T020 can run in parallel.
- T023 can run in parallel with endpoint foundation tasks after test fixtures exist.
- Test tasks within each user story marked `[P]` can be created in parallel.
- US4 can proceed in parallel with US2/US3 after persistence foundation is stable because SKU is independent from warehouse setup in this milestone.
- UI tasks can begin after corresponding API client methods and endpoint contracts are stable.

## Notes

- Tests should be written before implementation and should fail before the corresponding implementation task is completed.
- Use lightweight command/query naming inside vertical slices: commands mutate state, queries read state.
- Do not introduce MediatR, a global dispatcher, or a generic CQRS framework for this milestone.
- Keep endpoint handlers thin; delegate behavior to feature/application/domain code.
- Keep business rules out of EF configuration and Blazor API clients.
- Keep `Formica.ApiService` as host/composition root; Warehouse business logic remains inside the logical Warehouse boundary.
- Use default Blazor with Bootstrap for the first Warehouse Foundation UI implementation.
- Do not introduce MudBlazor or another UI component framework unless a concrete implementation need is explicitly documented first.
- Do not introduce external validation frameworks, mapping frameworks, or additional UI libraries for this milestone.
- Commit after each task or coherent task group.
