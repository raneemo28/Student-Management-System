# Student Management System — Technical Documentation

## Overview

A Clean Architecture ASP.NET Core 10 Web API for managing students, instructors, courses, course content, and homework assignments. The system uses CQRS with MediatR, domain events for cross-cutting synchronization, a separate read database for optimized queries, and a multi-tier caching strategy.

---

## Table of Contents

1. [Architecture](#architecture)
2. [Solution Structure](#solution-structure)
3. [Databases](#databases)
4. [CQRS & MediatR](#cqrs--mediatr)
5. [Domain Events](#domain-events)
6. [Caching Strategy](#caching-strategy)
7. [Authentication & Authorization](#authentication--authorization)
8. [File Storage](#file-storage)
9. [API Endpoints](#api-endpoints)
10. [Key Design Patterns](#key-design-patterns)
11. [Dependencies](#dependencies)
12. [Deployment](#deployment)
13. [Build & Run](#build--run)

---

## Architecture

**Clean Architecture** with the following layers:

| Layer | Project | Responsibility |
|-------|---------|---------------|
| **Domain** | `App.domain` | Entities, read models, domain events, contracts |
| **Application** | `App.Application` | Services, CQRS commands/queries, validators, DTOs, repository interfaces |
| **Infrastructure** | `App.infra` | EF Core DbContexts, repositories, file storage, caching, strategy implementations, background services |
| **API** | `App.APIs` | Controllers, authentication, DI composition root |

**Dependency direction**: API → Application → Domain; Infrastructure → Application → Domain

---

## Solution Structure

```
task_1/
├── task_1.slnx                          # Solution file
├── Dockerfile                           # Container build definition
├── docker-compose.yml                   # Multi-service deployment (API + Redis + SQL Server)
├── README.md                            # Project overview
├── flow.md                              # Application flow documentation
├── App.domain/                          # Domain layer
│   ├── entity/                          # Domain entities (Student, Instructor, Course, etc.)
│   ├── ReadModels/                      # Denormalized read models (StudentRead, CourseRead, etc.)
│   └── ...
├── App.Application/                     # Application layer
│   ├── CQRS/                          # MediatR commands and queries
│   │   ├── Students/
│   │   ├── Instructors/
│   │   ├── Courses/
│   │   ├── Enrollments/
│   │   ├── CourseContents/
│   │   ├── Homeworks/
│   │   ├── HomeworkSubmissions/
│   │   ├── HomeworkSolutions/
│   │   ├── HomeworkQuestionMarks/
│   │   ├── Employees/
│   │   └── Advertisements/
│   ├── Services/                      # Application services (CRUD wrappers)
│   ├── Repositories/                  # Repository interfaces
│   ├── DTOs/                          # Data Transfer Objects
│   ├── Validators/                    # FluentValidation validators
│   └── Common/                        # Shared abstractions (Result pattern, events)
├── App.infra/                         # Infrastructure layer
│   ├── Persistence/                   # EF Core DbContexts
│   │   ├── AppDbContext.cs            # Write database context
│   │   ├── AuthDbContext.cs           # Identity database context
│   │   └── AppReadDbContext.cs        # Read database context
│   ├── Repositories/                  # Repository implementations
│   │   ├── Repository.cs            # Generic repository base
│   │   ├── UnitOfWork.cs            # Unit of work pattern
│   │   ├── *ReadRepository.cs       # Read model repositories (11)
│   │   └── *Repository.cs           # Write model repositories (12)
│   ├── Strategies/                    # Domain event → read model sync strategies (27)
│   ├── Services/                      # Infrastructure services
│   │   ├── DomainEventProcessorService.cs  # Background event processor
│   │   ├── AuthService.cs             # JWT authentication service
│   │   └── ...
│   ├── FileStorage/                   # File storage abstraction
│   │   ├── LocalFileStorageService.cs
│   │   ├── ContentTypeHandlerFactory.cs
│   │   └── *ContentTypeHandler.cs    # Pdf, Image, Video handlers
│   └── Caching/                       # Caching infrastructure
│       ├── CacheService.cs           # ICacheService (IMemoryCache wrapper)
│       ├── CacheInvalidationService.cs # ICacheInvalidationService (memory-based)
│       ├── RedisCacheInvalidationService.cs # ICacheInvalidationService (Redis-based)
│       ├── CacheKeyGenerator.cs      # Centralized cache key generation
│       └── CacheTtl.cs              # Per-entity TTL configuration
└── App.APIs/                          # API layer
    ├── Controllers/                   # 14 controllers
    ├── Program.cs                     # Composition root
    ├── appsettings.json              # Configuration
    └── ...
```

---

## Databases

### Three Database Contexts

| Context | Database | Purpose |
|---------|----------|---------|
| `AppDbContext` | `StudentDomainDb` | Write database — all domain entities and domain events |
| `AuthDbContext` | `StudentAuthDb` | ASP.NET Core Identity — users, roles, JWT auth |
| `AppReadDbContext` | `StudentReadDb` | Read-optimized denormalized models |

### Write Database Schema (`StudentDomainDb`)

| Entity | Key | Description |
|--------|-----|-------------|
| `Student` | `Student_id` (GUID) | Student profiles |
| `Instructor` | `Instructor_id` (GUID) | Instructor profiles |
| `Employee` | `Employee_id` (GUID) | Employee/admin profiles |
| `Course` | `Course_id` (GUID) | Course catalog |
| `Course_student` | (`Student_id`, `Course_id`) | Enrollment join table |
| `CourseContent` | `Content_id` (GUID) | Course materials (PDF, image, video) |
| `Advertisement` | `Advertisement_id` (GUID) | System advertisements |
| `Homework` | `Homework_id` (GUID) | Homework assignments |
| `HomeworkSubmission` | `Submission_id` (GUID) | Student submissions |
| `HomeworkSolution` | `Solution_id` (GUID) | Instructor solution files |
| `HomeworkQuestionMark` | `QuestionMark_id` (GUID) | Per-question grading marks |
| `DomainEvent` | `Id` (GUID) | Persisted domain events for event-driven sync |

### Read Database Schema (`StudentReadDb`)

Denormalized read models optimized for queries (11 models):

| Read Model | Key | Cached Entity | TTL |
|------------|-----|--------------|-----|
| `StudentRead` | `Student_id` | Student | 30s |
| `InstructorRead` | `Instructor_id` | Instructor | 60s |
| `EmployeeRead` | `Employee_id` | Employee | 60s |
| `CourseRead` | `Course_id` | Course | 60s |
| `CourseStudentRead` | (`Student_id`, `Course_id`) | Enrollment | 30s |
| `CourseContentRead` | `Content_id` | CourseContent | 120s |
| `AdvertisementRead` | `Advertisement_id` | Advertisement | 300s |
| `HomeworkRead` | `Homework_id` | Homework | 15s |
| `HomeworkSubmissionRead` | `Submission_id` | Submission | 15s |
| `HomeworkSolutionRead` | `Solution_id` | Solution | 30s |
| `HomeworkQuestionMarkRead` | `QuestionMark_id` | QuestionMark | 10s |

---

## CQRS & MediatR

All reads use **Queries** and all writes use **Commands**, dispatched via MediatR's `IMediator`.

### Query Pattern

```
Client → Controller → [Mediator.Send(Query)] → QueryHandler → IReadRepository → AppReadDbContext → DTO → Client
```

### Command Pattern

```
Client → Controller → [Mediator.Send(Command)] → CommandHandler → IUnitOfWork → AppDbContext → DomainEvent → DTO → Client
```

### CQRS Modules

Each domain area has its own CQRS folder with separate Commands and Queries:

- **Students**: CreateStudentCommand, UpdateStudentCommand, DeleteStudentCommand, GetAllStudentsQuery, GetStudentByIdQuery
- **Instructors**: Create/Update/Delete Commands, GetAll/GetById Queries
- **Courses**: Create/Update/Delete Commands (incl. UpdateWeights), GetAll/GetById Queries
- **Enrollments**: Create/Delete Commands, GetAll/GetById Queries
- **CourseContents**: Create/Update/Delete Commands, GetByCourse/GetById Queries
- **Homeworks**: Create/Update/Delete Commands, GetByCourse/GetById Queries
- **HomeworkSubmissions**: Submit/Delete Commands, GetByHomework/GetByStudent Queries
- **HomeworkSolutions**: Create/Delete Commands, GetByHomework Query
- **HomeworkQuestionMarks**: Create/Delete Commands, GetBySubmission Query
- **Employees**: Create/Update/Delete Commands, GetAll/GetById Queries
- **Advertisements**: Create/Update/Delete Commands, GetAll/GetById Queries
- **Marks**: UpdateMarks/GetMarks/GetFinalMark (via `MarksController`)
- **Weights**: GetWeights/UpdateWeights (via `WeightsController`)

---

## Domain Events

### Event-Driven Sync Between Write and Read Databases

```
Command → Write DB (AppDbContext) → DomainEvent created → Saved to DomainEvents table
    ↓
DomainEventProcessorService (Background Service)
    ↓ Polls every 5 seconds, batches up to 100 events
    ↓
DomainEventWriteStrategyFactory → Selects strategy by EventType
    ↓
Strategy.ProcessAsync() → Writes to Read DB (AppReadDbContext)
    ↓
Cache invalidation triggered
    ↓
Event marked as IsProcessed = true
```

### Write Strategies (27 strategies)

Each strategy maps a domain event type to a read model update:

| Event Type | Strategy | Action |
|-----------|----------|--------|
| StudentCreated/Updated/Deleted | `Student*WriteStrategy` | Add/Update/Remove StudentRead |
| InstructorCreated/Updated/Deleted | `Instructor*WriteStrategy` | Add/Update/Remove InstructorRead |
| CourseCreated/Updated/Deleted | `Course*WriteStrategy` | Add/Update/Remove CourseRead |
| EnrollmentCreated/Deleted | `Enrollment*WriteStrategy` | Add/Remove CourseStudentRead |
| CourseContentCreated/Updated/Deleted | `CourseContent*WriteStrategy` | Add/Update/Remove CourseContentRead |
| EmployeeCreated/Updated/Deleted | `Employee*WriteStrategy` | Add/Update/Remove EmployeeRead |
| AdvertisementCreated/Updated/Deleted | `Advertisement*WriteStrategy` | Add/Update/Remove AdvertisementRead |
| HomeworkCreated/Updated/Deleted | `Homework*WriteStrategy` | Add/Update/Remove HomeworkRead |
| HomeworkSubmissionCreated/Deleted | `HomeworkSubmission*WriteStrategy` | Add/Remove HomeworkSubmissionRead |
| HomeworkSolutionCreated/Deleted | `HomeworkSolution*WriteStrategy` | Add/Remove HomeworkSolutionRead |
| HomeworkQuestionMarkCreated/Deleted | `HomeworkQuestionMark*WriteStrategy` | Add/Remove HomeworkQuestionMarkRead |

### Entity Type Mapping

| Event Type Contains | Cache Entity Type |
|--------------------|--------------------|
| "Student" | StudentRead |
| "Instructor" | InstructorRead |
| "CourseCreated"/"CourseUpdated"/"CourseDeleted" | CourseRead |
| "Enrollment" | CourseStudentRead |
| "CourseContent" | CourseContentRead |
| "Employee" | EmployeeRead |
| "Advertisement" | AdvertisementRead |
| "Homework" (not Submission/Solution/Question) | HomeworkRead |
| "HomeworkSubmission" | HomeworkSubmissionRead |
| "HomeworkSolution" | HomeworkSolutionRead |
| "HomeworkQuestion" | HomeworkQuestionMarkRead |

---

## Caching Strategy

### Three-Tier Caching Architecture

```
┌─────────────────────────────────────┐
│  Tier 3: HTTP Response Caching      │
│  [ResponseCache] on GET endpoints   │
│  Duration varies per endpoint       │
├─────────────────────────────────────┤
│  Tier 2: Distributed Cache (Redis)  │
│  ICacheInvalidationService          │
│  Enabled via Caching:UseDistributed │
│  Cache config                       │
├─────────────────────────────────────┤
│  Tier 1: In-Memory Cache            │
│  ICacheService (IMemoryCache)       │
│  Applied in all read repositories   │
├─────────────────────────────────────┤
│  AppReadDbContext (SQL Server)      │
└─────────────────────────────────────┘
```

### Tier 1 — In-Memory Cache

Every read repository (`*ReadRepository`) injects `ICacheService` and wraps all queries with cache lookups.

**Cache keys** follow the pattern: `read:{EntityType}:{Id}` or `read:{EntityType}:all`.

**TTL per entity type** (defined in `CacheTtl.cs`):

| Entity Type | TTL |
|------------|-----|
| StudentRead | 30s |
| InstructorRead | 60s |
| EmployeeRead | 60s |
| CourseRead | 60s |
| CourseStudentRead | 30s |
| CourseContentRead | 120s |
| AdvertisementRead | 300s |
| HomeworkRead | 15s |
| HomeworkSubmissionRead | 15s |
| HomeworkSolutionRead | 30s |
| HomeworkQuestionMarkRead | 10s |

### Tier 2 — Distributed Cache (Redis)

Conditionally enabled via `Caching:UseDistributedCache: true` in configuration. Uses `StackExchange.Redis` via `IDistributedCache`. Includes `RedisCacheInvalidationService` for cross-instance cache invalidation.

### Tier 3 — HTTP Response Caching

All 14 controllers have `[ResponseCache]` attributes on GET endpoints with duration varying by data volatility. Auth endpoints have `NoStore = true`.

### Cache Invalidation

Triggered by the `DomainEventProcessorService` after each domain event is processed. The service:
1. Extracts entity type from event type name
2. Extracts entity ID from event JSON data
3. Calls `ICacheInvalidationService.InvalidateByEntityAsync(entityType, entityId)`
4. This removes both the specific entity cache and the "all" list cache for that entity type

Each write strategy also independently calls invalidation after its own processing, providing defense in depth.

---

## Authentication & Authorization

### Authentication Flow

1. `POST /api/auth/register` — Creates Identity user + Employee profile, returns JWT
2. `POST /api/auth/login` — Validates credentials, returns JWT with `EmployeeId` claim
3. JWT token included in `Authorization: Bearer <token>` header on subsequent requests

### Roles & Permissions

| Role | Permissions |
|------|------------|
| **Student** | View/update/delete own profile, enroll in courses, view/download course content, submit homework solutions |
| **Instructor** | View/update/delete own profile, create/manage courses, manage course content, create/manage homework, grade submissions |
| **Employee** | All instructor permissions + manage employee profiles, manage advertisements, system administration |

### Authentication Services

- `IAuthService` → `AuthService` (`App.infra.Services.AuthService`)
- ASP.NET Core Identity with `AuthDbContext`
- JWT Bearer tokens with configurable secret, issuer, audience, expiry

---

## File Storage

### Abstraction

- `IFileStorageService` → `LocalFileStorageService`
- Files stored locally under `Storage/` directory
- Served via `/storage/` URL path

### Content Type Strategy

- `ContentTypeHandlerFactory` selects handler based on file extension/content type
- `IContentTypeHandler` implementations:
  - `PdfContentTypeHandler` — PDF file validation and processing
  - `ImageContentTypeHandler` — Image validation and processing
  - `VideoContentTypeHandler` — Video validation and processing
- Extensible: add new handlers by implementing `IContentTypeHandler`

### File Usage in Domain

| File Type | Context | Accepted Types |
|-----------|---------|---------------|
| Homework question file | `Homework` creation | PDF |
| Homework solution file | `HomeworkSolution` creation | PDF |
| Course content file | `CourseContent` creation | PDF, Image, Video |

---

## API Endpoints

| Controller | Method | Endpoint | Description |
|-----------|--------|----------|-------------|
| `AuthController` | POST | `/api/auth/register` | Register new user |
| `AuthController` | POST | `/api/auth/login` | Login and get JWT |
| `StudentsController` | GET | `/api/students` | Get all students |
| `StudentsController` | GET | `/api/students/{id}` | Get student by ID |
| `StudentsController` | POST | `/api/students` | Create student |
| `StudentsController` | PUT | `/api/students/{id}` | Update student |
| `StudentsController` | DELETE | `/api/students/{id}` | Delete student |
| `InstructorsController` | GET/POST/PUT/DELETE | `/api/instructors[/{id}]` | Instructor CRUD |
| `EmployeesController` | GET/POST/PUT/DELETE | `/api/employees[/{id}]` | Employee CRUD |
| `CoursesController` | GET/POST/PUT/DELETE | `/api/courses[/{id}]` | Course CRUD |
| `EnrollmentsController` | GET/POST/DELETE | `/api/enrollments` | Enrollment management |
| `CourseContentsController` | GET/POST/PUT/DELETE | `/api/CourseContents/...` | Course content management |
| `HomeworksController` | GET/POST/PUT/DELETE | `/api/Homeworks/...` | Homework management |
| `HomeworkSubmissionsController` | GET/POST/DELETE | `/api/HomeworkSubmissions/...` | Submission management |
| `HomeworkSolutionsController` | GET/POST/DELETE | `/api/HomeworkSolutions/...` | Solution management |
| `HomeworkQuestionMarksController` | GET/POST/DELETE | `/api/HomeworkQuestionMarks/...` | Grading management |
| `MarksController` | GET/PUT | `/api/Marks/enrollment/...` | Course marks |
| `WeightsController` | GET/PUT | `/api/Weights/course/{courseId}` | Course weight configuration |
| `AdvertisementsController` | GET/POST/PUT/DELETE | `/api/Advertisements[/{id}]` | Advertisement management |

### Response Caching Summary

| Endpoint | Duration | Vary By |
|----------|----------|---------|
| `GET /api/students` | 10s | None |
| `GET /api/students/{id}` | 20s | id |
| `GET /api/instructors` | 20s | None |
| `GET /api/courses` | 20s | None |
| `GET /api/courses/{id}` | 60s | id |
| `GET /api/Advertisements` | 300s | None |
| `GET /api/Homeworks/course/{courseId}` | 10s | courseId |
| `GET /api/HomeworkSubmissions/homework/{homeworkId}` | 10s | homeworkId |
| `GET /api/HomeworkQuestionMarks/submission/{submissionId}` | 5s | submissionId |
| `GET /api/Marks/enrollment/{studentId}/{courseId}` | 10s | studentId, courseId |

---

## Key Design Patterns

| Pattern | Usage |
|---------|-------|
| **Clean Architecture** | 4-layer separation (Domain, Application, Infrastructure, API) |
| **CQRS** | Separate Commands (writes) and Queries (reads) via MediatR |
| **Repository Pattern** | Generic `IRepository<T>` for writes, specific `I*ReadRepository` for reads |
| **Unit of Work** | `IUnitOfWork` wraps all write repositories with single `SaveChangesAsync()` |
| **Strategy Pattern** | 27 `IDomainEventWriteStrategy` implementations for read DB sync |
| **Domain Events** | Persisted events processed by background service for eventual consistency |
| **Result Pattern** | `Result<T>` / `Result` for explicit success/failure handling |
| **FluentValidation** | `AbstractValidator<T>` for request DTO validation |
| **Dependency Injection** | All services injected via constructor in Program.cs composition root |
| **Background Service** | `DomainEventProcessorService` polls write DB for unprocessed events |
| **Read/Write DB Separation** | `AppDbContext` (write) vs `AppReadDbContext` (read) |
| **Cache Aside Pattern** | Read repositories check cache before DB, invalidate on write |
| **Content Type Strategy** | `IContentTypeHandler` for extensible file type handling |

---

## Dependencies

### NuGet Packages

| Package | Version | Project | Purpose |
|---------|---------|---------|---------|
| `Microsoft.EntityFrameworkCore` | 10.0.11 | All | ORM base |
| `Microsoft.EntityFrameworkCore.SqlServer` | 10.0.11 | All | SQL Server provider |
| `Microsoft.EntityFrameworkCore.Tools` | 10.0.11 | App.infra | Migrations |
| `Microsoft.AspNetCore.Identity.EntityFrameworkCore` | 10.0.11 | All | Identity integration |
| `Microsoft.AspNetCore.Authentication.JwtBearer` | 10.0.11 | App.APIs | JWT auth |
| `MediatR` | 12.4.1 | App.APIs | CQRS mediator |
| `FluentValidation` | 11.11.0 | App.APIs | Request validation |
| `FluentValidation.DependencyInjectionExtensions` | 11.11.0 | App.APIs | DI integration |
| `Microsoft.Extensions.Caching.Memory` | 10.0.11 | App.infra | In-memory cache |
| `Microsoft.Extensions.Caching.StackExchangeRedis` | 10.0.11 | App.infra | Redis cache |
| `StackExchange.Redis` | 2.79.4* | App.infra | Redis client |
| `Microsoft.AspNetCore.OpenApi` | 10.0.5 | App.APIs | Swagger/OpenAPI |

*Note: StackExchange.Redis 2.79.4 not found; 3.0.0 resolved as compatible alternative.

---

## Deployment

### Docker

The project includes a `Dockerfile` for containerized deployment and `docker-compose.yml` for multi-service orchestration.

**Services in docker-compose.yml:**

| Service | Image | Ports | Purpose |
|---------|-------|-------|---------|
| `api` | Built from Dockerfile | 8080, 8081 | ASP.NET Core Web API |
| `redis` | `redis:7-alpine` | 6379 | Distributed cache |
| `sqlserver` | `mcr.microsoft.com/mssql/server:2022-latest` | 1433 | Database server |

### Configuration Flags

| Setting | Default | Description |
|---------|---------|-------------|
| `Caching:UseDistributedCache` | `false` | Enable Redis distributed cache |
| `Caching:DefaultTtlSeconds` | `30` | Default cache TTL |
| `Caching:TtlOverrides:{EntityType}:Seconds` | varies | Per-entity TTL overrides |
| `Redis:ConnectionString` | `localhost:6379` | Redis connection string |
| `Redis:InstanceName` | `studentmgmt:` | Redis key prefix |

### Connection Strings

| Name | Default | Database |
|------|---------|----------|
| `BusinessDbConnection` | `Server=localhost;Database=StudentDomainDb` | Write DB |
| `IdentityDbConnection` | `Server=localhost;Database=StudentAuthDb` | Identity DB |
| `ReadDbConnection` | `Server=localhost;Database=StudentReadDb` | Read DB |

---

## Build & Run

### Prerequisites

- .NET 10 SDK
- SQL Server (or Docker)
- Redis (or Docker)

### Build

```bash
dotnet build
```

### Run

```bash
dotnet run --project App.APIs
```

### Database Migrations

```bash
dotnet ef database update --context AppDbContext
dotnet ef database update --context AuthDbContext
dotnet ef database update --context AppReadDbContext
```

### With Docker Compose

```bash
docker-compose up --build
```

### Running Tests

No test project exists yet. Add one with `dotnet new xunit` following the existing project structure.

---

## Project Key Metrics

| Metric | Value |
|--------|-------|
| Projects | 4 (App.domain, App.Application, App.infra, App.APIs) |
| Controllers | 14 |
| CQRS Modules | 14 (Students, Instructors, Courses, Enrollments, CourseContents, Homeworks, HomeworkSubmissions, HomeworkSolutions, HomeworkQuestionMarks, Employees, Advertisements, Marks, Weights, Auth) |
| Commands | ~35 |
| Queries | ~30 |
| Write Strategies | 27 |
| Read Models | 11 |
| Read Repositories | 11 |
| Write Repositories | 12 |
| Domain Entities | 12 |
| File | 0 |
| Cached Endpoints | All GET endpoints |
| Cache Tiers | 3 (In-Memory, Redis, HTTP Response) |
