# Formica

.NET/Aspire в Visual Studio 2026

Проект ведётся через GitHub Spec Kit: .specify, AGENTS.md и локальная .agents/skills задают workflow constitution → specify → clarify/checklist → plan → tasks → analyze → implement. 

При обсуждении Formica формировать спецификации тщательно и последовательно: сначала constitution и принципы проекта, затем feature spec, уточнения, план, задачи и только потом реализация. 

Архитектурный ориентир: современный .NET 10/Aspire подход с DDD + Clean Architecture + Modular Monolith + Vertical Slices, Minimal API, EF Core, OpenTelemetry/Seq, минимум сторонних библиотек, собственные простые абстракции вместо MediatR и избыточных фреймворков. 
dotnet/eShop использовать только как справочный референс, не как канон. 

Formica должна быть практичным, качественным и портфолио-пригодным проектом; важны понятные имена, явные границы модулей, воспроизводимые CLI-команды, аккуратные коммиты и русскоязычные объяснения с точными .NET-терминами.
