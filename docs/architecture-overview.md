# MedicalSchedule — Guia de Arquitetura

## Visão Geral

MedicalSchedule é uma plataforma de agendamento veterinário construída com **microserviços .NET 10**, orquestrada pelo **.NET Aspire**, com frontend em **Flutter**. A ideia central é separar cada domínio de negócio em um serviço independente, com comunicação via HTTP (síncrona) e RabbitMQ (assíncrona).

---

## 1. Orquestração — .NET Aspire (AppHost)

O `AppHost` é o ponto de entrada de toda a stack. Ele declara todos os serviços e infraestrutura e o Aspire cuida de:
- Injetar connection strings automaticamente via **service discovery**
- Garantir ordem de inicialização (ex: Registry só sobe depois que Keycloak estiver pronto)
- Prover um dashboard visual com logs, traces e métricas de todos os serviços

**Infraestrutura levantada pelo Aspire:**

| Serviço | Tecnologia | Finalidade |
|---|---|---|
| `keycloak` | Keycloak (porta 8081) | Autenticação/autorização, emissão de JWT |
| `registry-db` | PostgreSQL | Banco do serviço Registry |
| `scheduling-db` | PostgreSQL | Banco do serviço Scheduling |
| `payments-db` | PostgreSQL | Banco do serviço Payments |
| `keycloak-db` | PostgreSQL | Banco interno do Keycloak |
| `rabbitmq` | RabbitMQ | Message broker para eventos de domínio |
| `redis` | Redis | Soft-locks de slots + idempotência |
| `mailhog` | MailHog | SMTP fake para desenvolvimento (porta 8025) |

**Serviços de aplicação orquestrados:**

```
AppHost registra e conecta:
├── Registry        ← usa: registry-db, rabbitmq, keycloak
├── Scheduling      ← usa: scheduling-db, rabbitmq, redis, keycloak
├── Payments        ← usa: payments-db, rabbitmq, redis
├── Notifications   ← usa: rabbitmq + HTTP para Registry
├── WhatsappBot     ← usa: rabbitmq, redis
└── Api.Gateway     ← usa: todos os serviços acima + keycloak
```

---

## 2. Gateway (YARP Reverse Proxy)

O `Api.Gateway` é o único ponto de entrada externo. Nenhum serviço é exposto diretamente ao cliente.

**Responsabilidades:**
- **Roteamento**: redireciona requisições para o microserviço correto via YARP
- **Autenticação**: valida JWT emitido pelo Keycloak antes de encaminhar
- **CORS**: libera apenas origens `localhost`/`127.0.0.1`
- **Auth endpoints**: `/api/auth/register` e `/api/auth/login` — cria usuários no Keycloak + cria entidade de domínio no Registry

**Tabela de rotas:**

| Prefixo da URL | Serviço de destino |
|---|---|
| `/registry/**` | Registry service |
| `/scheduling/**` | Scheduling service |
| `/payments/**` | Payments service |
| `/notifications/**` | Notifications service |
| `/whatsapp/**` | WhatsappBot service |

**Nota**: o path prefix é removido antes de encaminhar (ex: `/registry/api/pets` vira `/api/pets` no Registry).

---

## 3. Serviços de Domínio

### 3.1 Registry — Donos, Pets e Veterinários

Organizado em 4 projetos separados seguindo DDD clássico:

```
Registry.Api          ← Controllers HTTP (entrada)
Registry.Application  ← Use cases (Commands/Queries)
Registry.Domain       ← Entidades e regras de negócio puras
Registry.Infrastructure ← EF Core, repositórios, mensageria
```

**Entidades de domínio:**

- **Owner** — dono do pet (CPF, telefone, email)
- **Pet** — animal (espécie, raça, data de nascimento, estado de deleção)
- **Vet** — veterinário (CRM, especialidade)
- **PetOwnership** — relacionamento N:N entre Pet e Owner (suporta co-tutela)

**Pet possui um ciclo de deleção especial** (Saga):
```
None → PendingDeletion → Deleted
                       ↘ (rejeitado) → None
```
Antes de deletar um pet, o sistema verifica no Scheduling se há consultas ativas.

---

### 3.2 Scheduling — Consultas Veterinárias

Serviço "vertical slice" — organizado por feature, não por camada:

```
Features/
├── Consultations/
│   ├── ScheduleConsultationCommandHandler    ← agenda, com soft-lock Redis
│   ├── CancelConsultationCommandHandler
│   ├── RescheduleConsultationCommandHandler  ← relock no novo slot
│   └── GetConsultation(s)QueryHandler
└── Sagas/
    └── PetDeletionRequestedSagaHandler       ← cancela consultas ao deletar pet
```

**Entidade Consultation:**
- Campos: `PetId`, `VetId`, `OwnerId`, `ScheduledAt`, `Status`, `Notes`
- Status: `Scheduled` | `Cancelled`
- Ao ser agendada/cancelada/reagendada → levanta **domain events**

**Proteção contra race condition no agendamento:**
```
1. TryAcquireSlotLockAsync(vetId, scheduledAt) via Redis SET NX
2. Se lock não obtido → 409 Conflict (slot temporariamente reservado)
3. Verifica DB por conflito real
4. Persiste consulta
(lock expira em 30s automaticamente)
```

---

### 3.3 Notifications — E-mails Automáticos

Serviço puramente reativo — não tem endpoints HTTP próprios (exceto health).

**Funcionamento:**
1. Subscreve eventos no RabbitMQ
2. Faz HTTP para o Registry para buscar dados do dono (nome, email)
3. Envia e-mail via SMTP (MailHog em dev)

**Eventos consumidos:**
- `ConsultationScheduledEvent` → e-mail de confirmação
- `ConsultationCancelledEvent` → e-mail de cancelamento
- `ConsultationRescheduledEvent` → e-mail de reagendamento

---

### 3.4 Payments e WhatsappBot

Serviços em fase inicial (scaffolding). A estrutura existe e está conectada à infraestrutura, mas a lógica de negócio ainda não foi implementada.

---

## 4. Building Blocks — Bibliotecas Compartilhadas

Cada serviço referencia as libs que precisa. Nenhuma duplicação de código de infraestrutura.

### SharedKernel
Contratos universais:
```csharp
BaseEntity / LifeCycleEntity   ← base para todas as entidades
IDomainEvent                   ← base para eventos de domínio
ICommandHandler<TCmd>          ← CQRS
IQueryHandler<TQuery, TResult> ← CQRS
ConflictException / NotFoundException / BusinessLogicException
```

### Persistence
```csharp
AppDbContext        ← DbContext base com interceptors
AuditInterceptor    ← preenche CreatedAt/UpdatedAt automaticamente
OutboxInterceptor   ← salva domain events na tabela OutboxMessages
OutboxWorker        ← background service que processa a tabela e publica no RabbitMQ
```
**Outbox pattern garante**: se o DB salvar mas o RabbitMQ cair, o evento não se perde — o OutboxWorker vai retentá-lo.

### Messaging
```csharp
IMessagePublisher    ← publish de eventos para RabbitMQ
IMessageSubscriber   ← subscribe de eventos
// Implementado via EasyNetQ
```

### Caching
```csharp
ISlotLockService    ← SET NX com TTL de 30s para locks de slot
IdempotencyStore    ← evita processamento duplicado de mensagens
```

### ServiceDefaults
Configuração Aspire padrão injetada em todos os serviços:
- OpenTelemetry (logs, métricas, traces)
- Health checks (`/health`, `/alive`)
- Service discovery
- Resilience (retry com backoff)
- Configuração JWT/Keycloak

---

## 5. Fluxos de Comunicação

### 5.1 Fluxo de Autenticação
```
Flutter → POST /api/auth/register → Gateway
  Gateway → Keycloak Admin API (cria usuário, atribui role)
  Gateway → Registry API (cria Owner ou Vet)
  Gateway ← Keycloak password grant → JWT
Flutter ← JWT (armazenado em flutter_secure_storage)
```

### 5.2 Fluxo de Agendamento
```
Flutter → POST /scheduling/api/consultations (com JWT)
  Gateway valida JWT → encaminha para Scheduling
  Scheduling → Redis: TryAcquireSlotLock(vetId, scheduledAt)
  Scheduling → DB: verifica conflito real
  Scheduling → DB: persiste Consultation + OutboxMessage (mesma transação)
  OutboxWorker → RabbitMQ: publica ConsultationScheduledEvent
  Notifications subscreve → busca dados no Registry → envia e-mail
```

### 5.3 Saga de Deleção de Pet
```
DELETE /registry/api/pets/{id}
  Registry: Pet.RequestDeletion() → status PendingDeletion
  Registry: publica PetDeletionRequestedEvent
  
Scheduling recebe evento:
  Verifica consultas ativas
  Cancela cada uma → publica ConsultationCancelledEvent
  Publica PetDeletionApprovedEvent
  
Registry recebe aprovação:
  Pet.ConfirmDeletion() → status Deleted
```

---

## 6. Frontend Flutter

**Padrão de estado:** Riverpod (providers reativos)  
**Navegação:** go_router (rotas declarativas)  
**HTTP:** Dio com interceptor de token  
**Armazenamento:** flutter_secure_storage para JWT

```
lib/
├── core/       ← config, network, routes, theme, widgets
├── models/     ← DTOs (appointment, auth, owner, pet, vet)
├── pages/      ← telas por perfil (owner, vet, shared)
├── repositories/ ← acesso a dados (chama o Gateway)
├── services/   ← lógica de negócio do lado cliente
└── state/      ← Riverpod providers
```

A imagem Docker do frontend é um build Flutter Web servido pelo Nginx, com `API_BASE_URL` configurável via build arg.

---

## 7. Decisões Arquiteturais Chave

| Decisão | Por quê |
|---|---|
| Microserviços por domínio | Isolamento de responsabilidades, deploys independentes |
| Gateway único (YARP) | Um ponto de entrada, auth centralizada, sem exposição direta dos serviços |
| Outbox Pattern | Garante que eventos de domínio não se percam se o broker cair |
| Redis SET NX para slots | Resolve race condition sem locks de banco de dados (leve, TTL automático) |
| Saga para deleção de Pet | Coordena efeitos colaterais cross-service de forma assíncrona e resiliente |
| Aspire para orquestração | Dev experience: uma tecla sobe tudo; service discovery automático; dashboard de observabilidade |
| Keycloak externo | Não reinventar auth — delegamos gestão de usuários, roles, tokens a uma solução battle-tested |
| MailHog em dev | Captura e-mails sem envio real, visualizável via browser na porta 8025 |
