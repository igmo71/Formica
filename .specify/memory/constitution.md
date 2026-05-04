# Formica Constitution

## Core Principles

### I. Specification-First Development

Every meaningful product change starts with a specification.

A feature must be traceable through the Spec Kit workflow:

- `spec.md` defines WHAT and WHY;
- clarification/checklists resolve ambiguity before planning;
- `plan.md` defines HOW;
- `tasks.md` defines executable implementation work;
- analysis validates consistency before implementation.

Code is not the primary source of product intent. Specifications are.

### II. WHAT/WHY before HOW

Feature specifications must describe user needs, business goals, scenarios, acceptance criteria, and observable outcomes.

Feature specifications must not define project structure, database schema, API route names, UI component structure, messaging mechanisms, persistence details, or other implementation choices.

Implementation choices belong in `plan.md` and must be justified by the feature's needs.

### III. Modular Monolith First

Formica starts as a modular monolith.

Module boundaries must be explicit, but physical project proliferation must be justified. Prefer fewer projects until a real business or technical boundary requires separation.

A module boundary should protect a meaningful business capability, not merely organize code by technical layer.

### IV. Clean Architecture without Ceremony

Formica uses Clean Architecture as a dependency direction discipline, not as a requirement to create excessive layers, projects, or abstractions.

Business rules must remain independent from UI, HTTP transport, persistence, external integrations, and framework-specific infrastructure.

Feature implementation should prefer the simplest structure that preserves clear boundaries and testability. Additional layers, interfaces, projects, repositories, or mapping models must be introduced only when they protect a real boundary or reduce concrete complexity.

Avoid architecture-shaped boilerplate.

### V. Vertical Slice Delivery

Features should be delivered as vertical slices whenever practical.

A vertical slice may include UI, API, application behavior, domain logic, persistence, tests, and observability required to satisfy one user-visible capability.

Horizontal layers must not become the primary unit of planning. Shared abstractions must emerge from repeated need, not anticipation.

### VI. DDD-Oriented Design

Use DDD language and patterns where they clarify business behavior.

Prefer bounded contexts, aggregates, value objects, domain events, and ubiquitous language when they protect invariants or improve clarity.

Do not force DDD patterns where simple CRUD is sufficient. Domain modeling must serve business understanding, not architectural decoration.

### VII. Integration Isolation

External systems must not own Formica's core domain model.

Integrations with 1C, Bitrix24, mobile clients, scanners, or other systems must be isolated from core business behavior.

When business latency allows, integrations should prefer asynchronous processing and explicit synchronization state.

Specific integration mechanisms belong in the relevant feature `plan.md`, not in global project memory.

### VIII. Quality, Observability, and Maintainability

Every feature must define acceptance criteria that can be tested.

Implementation plans must define an appropriate validation strategy before implementation.

Operationally significant behavior must be observable through logs, traces, metrics, audit records, or diagnostics as appropriate.

The project should remain practical, understandable, and portfolio-grade: clear names, reproducible commands, clean commits, and maintainable structure matter.

### IX. Minimal Dependencies

Prefer built-in .NET, ASP.NET Core, Aspire, EF Core, and platform capabilities before adding third-party libraries.

Third-party dependencies are allowed when they provide clear value, reduce risk, or avoid unreasonable custom implementation.

Non-trivial dependencies must be justified in `plan.md`.

### X. Human-Readable Project Memory

Project memory files must stay concise, explicit, and readable by a human maintainer.

They must capture durable decisions, terminology, boundaries, and principles, not temporary discussion history or implementation details.

When a decision belongs to a specific feature, it must be documented in that feature's `spec.md`, `plan.md`, `research.md`, or `tasks.md`, not promoted to global memory prematurely.

Project memory must help future work start from shared context without forcing the reader to reconstruct decisions from chat history.

## Technical Constraints

Formica is a modern .NET solution using Aspire as the starting application model.

The default implementation direction is:

- .NET 10 / ASP.NET Core;
- Aspire Starter App baseline;
- Blazor UI;
- separate Web API surface;
- Minimal API where practical;
- EF Core for persistence when relational storage is needed;
- OpenTelemetry and Seq for observability where appropriate;
- simple in-house abstractions instead of MediatR or excessive framework dependencies by default.

These are defaults, not immutable laws. Exceptions are allowed when explicitly justified in the relevant `plan.md`.

The Microsoft `dotnet/eShop` repository may be used as a reference source, but it is not a project canon and must not be copied mechanically.

## Development Workflow and Quality Gates

Spec Kit is the primary and only Spec-Driven Development workflow for Formica.

The expected workflow is:

1. update project memory when global context changes;
2. create or update the feature specification;
3. clarify ambiguity before planning;
4. create the implementation plan;
5. generate executable tasks;
6. analyze consistency across artifacts;
7. implement only after the above artifacts are coherent.

Implementation work should not start from code-first exploration unless the task is explicitly marked as research or prototyping.

Before implementation, verify that:

- the feature has clear acceptance criteria;
- domain terms are consistent with the glossary;
- architecture choices are documented in `plan.md`;
- tasks are executable and ordered;
- deviations from this constitution are explicit and justified.

## Governance

This constitution governs Formica engineering and specification work.

It supersedes informal habits and ad-hoc architectural preferences when they conflict with documented project principles.

The constitution must not contain warehouse domain rules, business workflows, database design, API contracts, or UI design decisions. Those belong in product memory, feature specifications, plans, contracts, or tasks.

Amendments must be intentional, documented, and reflected in affected specifications or plans when necessary.

Principles are defaults, not handcuffs. Exceptions are allowed, but they must be explicit, local to the relevant feature or plan, and justified by concrete project needs.

**Version**: 0.1.0 | **Ratified**: 2026-05-04 | **Last Amended**: 2026-05-04
