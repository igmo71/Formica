# Tasks: Warehouse Foundation

**Input**: Design documents from `specs/001-warehouse-foundation/`  
**Prerequisites**: `spec.md`, `plan.md`, `research.md`, `data-model.md`, `implementation-guidelines.md`, `contracts/api-contracts.md`, `quickstart.md`  
**Branch**: `001-warehouse-foundation`

**Tests**: Included. The specification and quickstart require validation of user-visible behavior, uniqueness rules, lifecycle behavior, and API/UI readiness.

**Organization**: Tasks are grouped by phases and user stories so each story can be implemented and validated incrementally.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel because it touches different files or independent areas.
- **[Story]**: User story from `spec.md` when applicable.
- Tasks include concrete file paths or concrete target folders where final file decomposition is intentionally left to implementation.

## Phase 1: Setup

**Purpose**: Prepare the existing Aspire solution for Warehouse Foundation implementation.

- [X] T001 Add PostgreSQL Aspire hosting package to `Formica.AppHost/Formica.AppHost.csproj`.
- [X] T002 Add EF Core Npgsql provider and EF Core design package references to `Formica.ApiService/Formica.ApiService.csproj`.
- [X] T003 Configure PostgreSQL resource and API service connection in `Formica.AppHost/Program.cs`.
- [X] T004 Create Warehouse module folders under `Formica.ApiService/Warehouse/`.
- [X] T005 Create Warehouse Foundation UI folders under `Formica.Web/Warehouse/WarehouseFoundation/`.
- [X] T006 Create Warehouse Foundation test folders under `Formica.Tests/Warehouse/WarehouseFoundation/`.

---

## Phase 2: Foundational

**Purpose**: Implement shared model, persistence, validation, routing, and test infrastructure required by all user stories.

**Critical**: No user story implementation should begin until this phase is complete.

**Implementation Guardrails**: Phase 2 implementation MUST follow `specs/001-warehouse-foundation/implementation-guidelines.md`. In particular, Domain must not depend on Features, Location Address Rules must not use `WarehouseId` as a primary key, and EF Core migrations must not be created unless explicitly requested after a coherent persisted model exists.

### Domain and Persistence Foundation

- [X] T007 [P] Create lifecycle base abstractions or helper types in `Formica.ApiService/Warehouse/WarehouseFoundation/Domain/Common/`.
- [X] T008 [P] Create controlled value definitions for zone and storage location purposes in `Formica.ApiService/Warehouse/WarehouseFoundation/Domain/Common/`.
- [X] T009 [P] Create `StorageLocationCapacity` value-object-style type in `Formica.ApiService/Warehouse/WarehouseFoundation/Domain/StorageLocations/StorageLocationCapacity.cs`.
- [X] T010 [P] Create self-contained `LocationAddressRules` domain model in `Formica.ApiService/Warehouse/WarehouseFoundation/Domain/LocationAddressing/LocationAddressRules.cs` without dependencies on `Features`, `Endpoints`, `Persistence`, `Contracts`, ASP.NET Core, or Blazor UI.
- [X] T011 Create `WarehouseDbContext` in `Formica.ApiService/Warehouse/Persistence/WarehouseDbContext.cs`.
- [X] T012 Create EF Core configuration folder `Formica.ApiService/Warehouse/Persistence/Configurations/WarehouseFoundation/`.
- [X] T013 Create Warehouse persistence registration extension in `Formica.ApiService/Warehouse/Persistence/WarehousePersistenceServiceCollectionExtensions.cs`.
- [X] T014 Register Warehouse persistence and Warehouse Foundation endpoints in `Formica.ApiService/Program.cs`.
- [X] T015 Prepare Warehouse Foundation migration infrastructure under `Formica.ApiService/Warehouse/Persistence/Migrations/` only if needed, but do not create EF Core migration files in Phase 2 without explicit instruction.

### API Foundation

- [X] T016 Create endpoint group registration entry point in `Formica.ApiService/Warehouse/WarehouseFoundation/Endpoints/WarehouseFoundationEndpoints.cs`.
- [X] T017 [P] Create shared API result/problem helpers in `Formica.ApiService/Warehouse/WarehouseFoundation/Endpoints/EndpointResults.cs`.
- [X] T018 [P] Create shared contract DTO folder and only currently needed common DTOs in `Formica.ApiService/Warehouse/WarehouseFoundation/Contracts/`.
- [X] T019 [P] Create simple domain validation primitives in `Formica.ApiService/Warehouse/WarehouseFoundation/Domain/Common/Validation/` for domain-level errors/results; do not place domain validation primitives under `Features/Common/Validation`.
- [X] T020 [P] Implement location address normalization behavior inside `LocationAddressRules` or `Formica.ApiService/Warehouse/WarehouseFoundation/Domain/LocationAddressing/`; do not create `Features/StorageLocations/NormalizeLocationAddress.cs` for domain behavior.

### Test Foundation

- [X] T021 Create Aspire-backed API test fixture in `Formica.Tests/Warehouse/WarehouseFoundation/WarehouseFoundationApiFixture.cs`.
- [X] T022 Create PostgreSQL-backed persistence test fixture in `Formica.Tests/Warehouse/WarehouseFoundation/WarehousePersistenceFixture.cs`.
- [X] T023 [P] Create test data builders in `Formica.Tests/Warehouse/WarehouseFoundation/TestData/WarehouseFoundationTestData.cs`.

**Checkpoint**: Persistence, endpoint registration, shared validation, and test fixtures are ready. No generated EF Core migration is expected from Phase 2 unless explicitly requested.

---

## Phase 3: User Story 1 - Create a warehouse (Priority: P1) MVP

**Goal**: A warehouse administrator can create, view, update, deactivate, and reactivate warehouses with unique warehouse codes.

**Independent Test**: Create a warehouse, retrieve it by id, list it, reject duplicate code, update editable attributes, deactivate/reactivate while preserving identity.

### Tests for User Story 1

- [X] T024 [P] [US1] Add warehouse API contract tests in `Formica.Tests/Warehouse/WarehouseFoundation/Warehouses/WarehouseApiTests.cs`.
- [X] T025 [P] [US1] Add warehouse uniqueness persistence tests in `Formica.Tests/Warehouse/WarehouseFoundation/Warehouses/WarehousePersistenceTests.cs`.
- [X] T026 [P] [US1] Add default Location Address Rules creation test in `Formica.Tests/Warehouse/WarehouseFoundation/Warehouses/DefaultLocationAddressRulesTests.cs`.

### Implementation for User Story 1

- [X] T027 [US1] Create `Warehouse` domain model in `Formica.ApiService/Warehouse/WarehouseFoundation/Domain/Warehouses/Warehouse.cs`.
- [X] T028 [US1] Create Warehouse EF configuration in `Formica.ApiService/Warehouse/Persistence/Configurations/WarehouseFoundation/WarehouseConfiguration.cs`.
- [X] T029 [US1] Create Location Address Rules EF configuration in `Formica.ApiService/Warehouse/Persistence/Configurations/WarehouseFoundation/LocationAddressRulesConfiguration.cs`, only if Location Address Rules are persisted with a coherent model and without using `WarehouseId` as a placeholder primary key.
- [X] T030 [US1] Create warehouse request/response DTOs in `Formica.ApiService/Warehouse/WarehouseFoundation/Contracts/Warehouses/`.
- [X] T031 [US1] Implement create warehouse feature in `Formica.ApiService/Warehouse/WarehouseFoundation/Features/Warehouses/CreateWarehouse.cs`.
- [X] T032 [US1] Implement list/get warehouse features in `Formica.ApiService/Warehouse/WarehouseFoundation/Features/Warehouses/`.
- [X] T033 [US1] Implement update warehouse feature in `Formica.ApiService/Warehouse/WarehouseFoundation/Features/Warehouses/UpdateWarehouse.cs`.
- [X] T034 [US1] Implement deactivate/reactivate warehouse features in `Formica.ApiService/Warehouse/WarehouseFoundation/Features/Warehouses/`.
- [X] T035 [US1] Implement warehouse endpoints in `Formica.ApiService/Warehouse/WarehouseFoundation/Endpoints/WarehouseEndpoints.cs`.
- [X] T036 [US1] Add Warehouse API client methods in `Formica.Web/Warehouse/WarehouseFoundation/ApiClients/WarehouseFoundationApiClient.cs`.
- [X] T037 [US1] Add basic Warehouse management page in `Formica.Web/Warehouse/WarehouseFoundation/Pages/Warehouses.razor`.

**Checkpoint**: US1 works independently through API and has initial UI surface.

---

## Phase 4: User Story 2 - Create warehouse zones (Priority: P1)

**Goal**: A warehouse administrator can create and maintain zones inside a warehouse, with zone codes unique within that warehouse.

**Independent Test**: Create two zones in a warehouse, reject duplicate zone code in the same warehouse, allow same zone code in another warehouse, update/deactivate/reactivate zone.

**Review Strategy**: Phase 4 MUST be implemented in small reviewable increments. Do not implement the full Zone user story in one Codex pass.

### Phase 4A: Zone domain and persistence

**Goal**: Introduce the Zone domain model and PostgreSQL persistence constraints without exposing Zone API/UI behavior yet.

- [X] T038A [P] [US2] Add Zone domain lifecycle tests in `Formica.Tests/Warehouse/WarehouseFoundation/Zones/ZoneLifecycleTests.cs`.
- [X] T039A [P] [US2] Add Zone uniqueness persistence tests in `Formica.Tests/Warehouse/WarehouseFoundation/Zones/ZonePersistenceTests.cs`.
- [X] T041A [US2] Create `Zone` domain model in `Formica.ApiService/Warehouse/WarehouseFoundation/Domain/Zones/Zone.cs`.
- [X] T042A [US2] Create Zone EF configuration in `Formica.ApiService/Warehouse/Persistence/Configurations/WarehouseFoundation/ZoneConfiguration.cs`.
- [X] T042B [US2] Add Zone `DbSet` and model configuration registration to `Formica.ApiService/Warehouse/Persistence/WarehouseDbContext.cs`.
- [X] T042C [US2] Create explicit EF Core migration for Zone persistence after the coherent Zone model and configuration exist.

**Checkpoint**: Zone domain and persistence constraints exist, including `WarehouseId + Code` uniqueness and `Zone -> Warehouse` relationship. No Zone API or UI behavior is implemented yet.

### Phase 4B: Zone create/list/get API

**Goal**: Expose create/list/get behavior for Zones through contracts, features, endpoints, and API tests.

- [ ] T038B [P] [US2] Add Zone create/list/get API contract tests in `Formica.Tests/Warehouse/WarehouseFoundation/Zones/ZoneApiTests.cs`.
- [ ] T043B [US2] Create Zone request/response DTOs in `Formica.ApiService/Warehouse/WarehouseFoundation/Contracts/Zones/`.
- [ ] T044B [US2] Implement create zone feature in `Formica.ApiService/Warehouse/WarehouseFoundation/Features/Zones/CreateZone.cs`.
- [ ] T045B [US2] Implement list/get zone features in `Formica.ApiService/Warehouse/WarehouseFoundation/Features/Zones/`.
- [ ] T048B [US2] Implement create/list/get zone endpoints in `Formica.ApiService/Warehouse/WarehouseFoundation/Endpoints/ZoneEndpoints.cs`.

**Checkpoint**: Zone create/list/get works through API. Duplicate zone code in the same warehouse is rejected. Same zone code in another warehouse is allowed.

### Phase 4C: Zone update and lifecycle API

**Goal**: Add update/deactivate/reactivate behavior for Zones while preserving stable identity and scoped uniqueness semantics.

- [ ] T038C [P] [US2] Add Zone update/deactivate/reactivate API tests in `Formica.Tests/Warehouse/WarehouseFoundation/Zones/ZoneLifecycleApiTests.cs`.
- [ ] T046C [US2] Implement update zone feature in `Formica.ApiService/Warehouse/WarehouseFoundation/Features/Zones/UpdateZone.cs`.
- [ ] T047C [US2] Implement deactivate/reactivate zone features in `Formica.ApiService/Warehouse/WarehouseFoundation/Features/Zones/`.
- [ ] T048C [US2] Add update/deactivate/reactivate routes to `Formica.ApiService/Warehouse/WarehouseFoundation/Endpoints/ZoneEndpoints.cs`.

**Checkpoint**: Zone update/deactivate/reactivate works through API. Update preserves identity and does not move Zone to another Warehouse. Reactivation reruns uniqueness and validation checks.

### Phase 4D: Zone UI/client

**Goal**: Add Zone client methods and a Bootstrap-based Zone management component without moving business rules into the UI.

- [ ] T049D [US2] Add Zone API client methods in `Formica.Web/Warehouse/WarehouseFoundation/ApiClients/WarehouseFoundationApiClient.cs`.
- [ ] T050D [US2] Add Zone management component in `Formica.Web/Warehouse/WarehouseFoundation/Components/ZonesPanel.razor`.

**Checkpoint**: A user can manage zones for a selected warehouse through the initial Blazor/Bootstrap UI. UI shows active/inactive state and delegates business validation to API/domain/application behavior.

### Phase 4E: US2 consistency pass

**Goal**: Validate that US2 is coherent across specs, API behavior, persistence, UI, and tests.

- [ ] T050E [US2] Run `dotnet build .\Formica.slnx -m:1` and fix build errors.
- [ ] T050F [US2] Run `dotnet test .\Formica.Tests\Formica.Tests.csproj` and fix test failures.
- [ ] T050G [US2] Verify `quickstart.md` Scenario 4 and Scenario 5 against implemented behavior.
- [ ] T050H [US2] Review `contracts/api-contracts.md` and implementation for consistency.
- [ ] T050I [US2] Confirm no out-of-scope Storage Location, Warehouse Layout, SKU, Inventory, Receiving, Putaway, LPN, Picking, Packing, Shipping, scanner, or integration workflow was implemented.

**Checkpoint**: US1 and US2 work independently and together. Phase 4 can be marked accepted after review.

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
- Phase 4A tests can be created before or alongside the Zone domain/persistence implementation.
- Phase 4B must wait for Phase 4A persistence to be accepted.
- Phase 4C must wait for Phase 4B API foundations to be accepted.
- Phase 4D must wait for stable Zone contracts and API behavior.
- US4 can proceed in parallel with US2/US3 after persistence foundation is stable because SKU is independent from warehouse setup in this milestone.
- UI tasks can begin after corresponding API client methods and endpoint contracts are stable.

## Notes

- Read and follow `specs/001-warehouse-foundation/implementation-guidelines.md` before implementing any task.
- Tests should be written before implementation and should fail before the corresponding implementation task is completed.
- Use lightweight command/query naming inside vertical slices: commands mutate state, queries read state.
- Do not introduce MediatR, a global dispatcher, or a generic CQRS framework for this milestone.
- Keep endpoint handlers thin; delegate behavior to feature/application/domain code.
- Keep business rules out of EF configuration and Blazor API clients.
- Keep Domain independent from Features, Endpoints, Persistence, Contracts, ASP.NET Core, and Blazor UI.
- Keep reusable domain validation primitives under Domain, not under Features.
- Keep `Formica.ApiService` as host/composition root; Warehouse business logic remains inside the logical Warehouse boundary.
- Do not create EF Core migration files unless explicitly requested after a coherent persisted entity model exists.
- Codex must not modify files under `specs/` unless the prompt explicitly asks for spec-only changes. If Codex finds a specification inconsistency, it must stop and report it instead of changing the specification.
- Use default Blazor with Bootstrap for the first Warehouse Foundation UI implementation.
- Do not introduce MudBlazor or another UI component framework unless a concrete implementation need is explicitly documented first.
- Do not introduce external validation frameworks, mapping frameworks, or additional UI libraries for this milestone.
- Commit after each task or coherent task group.
