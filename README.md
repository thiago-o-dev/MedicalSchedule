# MedicalSchedule

Sistema de agendamento de consultas veterinárias. Trabalho acadêmico com foco em DDD (Domain-Driven Design).

## Arquitetura

Clean Architecture com dois Bounded Contexts:

- **Registration** — Cadastro de tutores (`Owner`) e pets (`Pet`)
- **Consultations** — Cadastro de veterinários (`Vet`) e agendamento de consultas (`Appointment`)

```
MedicalSchedule.Domain          → Entidades, eventos de domínio, abstrações
MedicalSchedule.Application     → Use cases, ViewModels, validators, event handlers
MedicalSchedule.Infrastructure  → EF Core, DbContext, DomainEventDispatcher
MedicalSchedule.WebApi          → Controllers, JWT, configuração do host
MedicalSchedule.Frontend        → (em desenvolvimento)
```

## Stack

- .NET 10 / C# 13
- EF Core 10 + SQLite
- FluentValidation 11
- Scrutor 5 (auto-registro de handlers)
- JWT Bearer (temporário — Keycloak futuramente)
- Scalar (documentação da API)

## Como rodar

```bash
# Restaurar dependências
dotnet restore

# Aplicar migrations (banco já criado se clonar com o .db)
dotnet ef database update --project MedicalSchedule.Infrastructure --startup-project MedicalSchedule.WebApi

# Rodar a API
dotnet run --project MedicalSchedule.WebApi
```

A API sobe em `https://localhost:5001`. Documentação interativa em `/scalar/v1`.

## Autenticação

Endpoint temporário para testes:

```
POST /api/auth/token
{ "username": "admin", "password": "admin" }
```

Retorna um JWT Bearer token. Todos os demais endpoints exigem o header `Authorization: Bearer <token>`.

## Endpoints

| Bounded Context | Recurso | Prefixo |
|---|---|---|
| Registration | Tutores | `api/registration/owners` |
| Registration | Pets | `api/registration/pets` |
| Consultations | Veterinários | `api/consultations/vets` |
| Consultations | Consultas | `api/consultations/appointments` |

## Regra cross-BC

Um pet com consultas futuras agendadas **não pode ser desativado**. Implementado via Domain Event (`PetDeactivationRequestedEvent`) disparado antes do `SaveChanges`, validado pelo handler do BC de Consultations.
