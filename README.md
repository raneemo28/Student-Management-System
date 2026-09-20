# Student Management System

A Clean Architecture ASP.NET Core Web API for managing students, instructors, courses, course content, and homework with CQRS, domain events, a separate read database, and a dedicated logging microservice.

## Architecture

- **App.APIs** — Controllers, authentication, DI setup, request logging middleware
- **App.Application** — Services, CQRS commands/queries, validators, repositories, DTOs, domain event abstractions
- **App.infra** — EF Core DbContexts, repositories, file storage, strategy implementations, background services, RabbitMQ log publisher
- **App.domain** — Entities, read models, domain events
- **App.LoggingMicroservice** — Independent logging service consuming from RabbitMQ, storing in LogsDatabase

## Key Features

### Authentication & Authorization
- JWT-based register/login/logout
- Registration creates an **Employee** user profile
- Role-based access control: **Student**, **Instructor**, or **Employee**
- Each role has distinct permissions (see below)

### Students
- View student profiles (public)
- Enroll in courses
- View and download course content
- Submit homework solutions

### Instructors
- View instructor profiles (public)
- Create, update, and delete courses
- Manage course content (upload, update, delete files such as PDFs, images, and videos)
- Create and manage homework assignments
- Upload solution files for homework
- Grade student submissions with per-question marks

### Employees
- Register, login, logout (creates Employee profile with Employee role)
- View, create, update, and delete employee profiles
- Manage advertisements (create, update, delete, view)
- **Manage student and instructor profiles** (create, update, delete via admin endpoints)
- System administration and oversight responsibilities

### Course Content
- Supports multiple file types via `ContentType` enum (Pdf, Image, Video)
- Extensible content-type handling using the **Strategy pattern** (`IContentTypeHandler`)
- Files stored locally under `Storage/` and served via `/storage/`

### Homework Management
- Instructors create homework assignments with deadlines and total marks
- Instructors can update homework details and deadlines
- Instructors upload solution files for homework
- Students submit homework solutions as PDF files
- Instructors grade submissions with per-question marks
- Domain events track homework lifecycle (created, updated, deleted)
- Separate read models for optimized homework queries

### Centralized Logging (Microservice)
- **RabbitMQ** message broker for async log transport
- **App.APIs** publishes all HTTP requests/responses/errors via `ILogPublisher`
- **Logging Microservice** consumes logs and persists to `LogsDatabase`
- Structured log entries: level, message, exception, correlation ID, user ID, request path/method, properties
- Health checks and Swagger UI at `/swagger`
- Independent deployment and scaling

## Cross-Cutting Concerns

- **Result pattern** for explicit success/failure handling
- **Repository pattern** + **Unit of Work** for data access abstraction
- **CQRS** with MediatR for commands and queries
- **Domain events** persisted in the write database and processed by a **background service**
- **Strategy pattern** for writing domain events to the read database
- **FluentValidation** for request validation
- **Separate read database** (`AppReadDbContext`) for optimized queries
- **Eventual consistency** between write and read databases via domain events
- **File storage abstraction** for handling PDF, image, and video content
- **In-memory caching** with `IMemoryCache` for read repository queries
- **Centralized async logging** via RabbitMQ + dedicated microservice

## API Endpoints

| Controller | Endpoints |
|------------|-----------|
| `AuthController` | `POST /api/auth/register`, `POST /api/auth/login` |
| `StudentsController` | `GET /api/students`, `GET /api/students/{id}`, `POST /api/students`, `PUT /api/students/{id}`, `DELETE /api/students/{id}` |
| `InstructorsController` | `GET /api/instructors`, `GET /api/instructors/{id}`, `POST /api/instructors`, `PUT /api/instructors/{id}`, `DELETE /api/instructors/{id}` |
| `CoursesController` | `GET /api/courses`, `GET /api/courses/{id}`, `POST /api/courses`, `PUT /api/courses/{id}`, `DELETE /api/courses/{id}` |
| `EnrollmentsController` | `GET /api/enrollments`, `GET /api/enrollments/{studentId}/{courseId}`, `POST /api/enrollments`, `DELETE /api/enrollments/{studentId}/{courseId}` |
| `CourseContentsController` | `GET /api/CourseContents/course/{courseId}`, `GET /api/CourseContents/{id}`, `POST /api/CourseContents/course/{courseId}`, `PUT /api/CourseContents/{id}`, `DELETE /api/CourseContents/{id}` |
| `HomeworksController` | `GET /api/Homeworks/course/{courseId}`, `GET /api/Homeworks/{id}`, `POST /api/Homeworks/course/{courseId}`, `PUT /api/Homeworks/{id}`, `DELETE /api/Homeworks/{id}` |
| `HomeworkSubmissionsController` | `GET /api/HomeworkSubmissions/homework/{homeworkId}`, `GET /api/HomeworkSubmissions/student/{studentId}/homework/{homeworkId}`, `POST /api/HomeworkSubmissions`, `DELETE /api/HomeworkSubmissions/{submissionId}` |
| `HomeworkSolutionsController` | `GET /api/HomeworkSolutions/homework/{homeworkId}`, `POST /api/HomeworkSolutions`, `DELETE /api/HomeworkSolutions/{solutionId}` |
| `HomeworkQuestionMarksController` | `GET /api/HomeworkQuestionMarks/submission/{submissionId}`, `POST /api/HomeworkQuestionMarks`, `DELETE /api/HomeworkQuestionMarks/{questionMarkId}` |
| `AdvertisementsController` | `GET /api/advertisements`, `GET /api/advertisements/{id}`, `POST /api/advertisements`, `PUT /api/advertisements/{id}`, `DELETE /api/advertisements/{id}` |
| `EmployeesController` | `GET /api/employees`, `GET /api/employees/{id}`, `POST /api/employees`, `PUT /api/employees/{id}`, `DELETE /api/employees/{id}` |

### Logging Microservice Endpoints
| Service | Endpoints |
|---------|-----------|
| `LoggingMicroservice` | `GET /health`, `GET /swagger` |

## Databases

- **Write Database** — `StudentDomainDb` (students, instructors, courses, enrollments, course contents, homeworks, submissions, solutions, question marks, domain events)
- **Identity Database** — `StudentAuthDb` (ASP.NET Core Identity users and roles)
- **Read Database** — `StudentReadDb` (denormalized read models for queries)
- **Logs Database** — `LogsDatabase` (structured log entries from all services)

## Infrastructure

### Docker Compose Services
| Service | Image | Ports | Description |
|---------|-------|-------|-------------|
| `rabbitmq` | `rabbitmq:3.13-management` | 5672, 15672 | Message broker (management UI on 15672) |
| `business-db` | SQL Server 2022 | 1433 | `StudentDomainDb` |
| `identity-db` | SQL Server 2022 | 1434 | `StudentAuthDb` |
| `read-db` | SQL Server 2022 | 1435 | `StudentReadDb` |
| `logs-db` | SQL Server 2022 | 1436 | `LogsDatabase` |
| `logging-service` | Built from `App.LoggingMicroservice/Dockerfile` | 5232, 5233 | Log consumer microservice |
| `app` | Built from `Dockerfile` | 5230, 5231 | Main API |

All services share the `student-network` bridge network.

## Getting Started

### Local Development (without Docker)

1. Configure connection strings in `App.APIs/appsettings.json` and `App.LoggingMicroservice/appsettings.json`
2. Start SQL Server and RabbitMQ locally
3. Run migrations:
    ```bash
    dotnet ef database update --context AppDbContext
    dotnet ef database update --context AuthDbContext
    dotnet ef database update --context AppReadDbContext
    ```
4. Run the application:
    ```bash
    dotnet run --project App.APIs
    dotnet run --project App.LoggingMicroservice
    ```

### With Docker Compose (Recommended)

1. Build and start all services:
    ```bash
    docker-compose up -d
    ```

2. For development with hot reload on the main API:
    ```bash
    docker-compose -f docker-compose.yml -f docker-compose.override.yml up -d
    ```

3. Run migrations (after containers start):
    ```bash
    docker-compose exec app dotnet ef database update --context AppDbContext
    docker-compose exec app dotnet ef database update --context AuthDbContext
    docker-compose exec app dotnet ef database update --context AppReadDbContext
    ```

4. Access services:
   - **Main API**: `http://localhost:5230` (Swagger at `/swagger`)
   - **Logging Microservice**: `http://localhost:5232` (Swagger at `/swagger`, health at `/health`)
   - **RabbitMQ Management**: `http://localhost:15672` (guest/guest)

5. View logs:
    ```bash
    docker-compose logs -f app
    docker-compose logs -f logging-service
    ```

6. Stop all services:
    ```bash
    docker-compose down
    ```

### Default Credentials
- **SQL Server SA Password**: `YourStrong@Passw0rd` (change for production!)
- **RabbitMQ**: guest/guest

## Project Structure

```
task_1/
├── App.APIs/                    # Web API project
│   ├── Controllers/             # REST endpoints
│   ├── Middleware/              # RequestLoggingMiddleware
│   ├── Dockerfile
│   └── appsettings.json
├── App.Application/             # Application layer
│   ├── CQRS/                    # Commands & Queries
│   ├── DTOs/                    # Data Transfer Objects
│   ├── Interfaces/              # Service & Repository contracts
│   ├── Services/                # Business logic
│   └── Validators/              # FluentValidation rules
├── App.infra/                   # Infrastructure layer
│   ├── Persistence/             # DbContexts, Migrations
│   ├── Repositories/            # EF Core implementations
│   ├── Services/                # AuthService, RabbitMqLogPublisher, etc.
│   ├── Strategies/              # Domain event → read model projections
│   ├── Caching/                 # In-memory cache implementations
│   └── FileStorage/             # Local file storage
├── App.domain/                  # Domain layer
│   ├── entity/                  # Write-side entities
│   └── ReadModels/              # Denormalized read models
├── App.LoggingMicroservice/     # Logging microservice
│   ├── Consumers/               # LogConsumer (RabbitMQ)
│   ├── Data/                    # LogsDbContext
│   ├── Models/                  # LogEntry, LogMessage
│   ├── Dockerfile
│   └── appsettings.json
├── docker-compose.yml           # Production compose
├── docker-compose.override.yml  # Development compose (hot reload)
├── flow.md                      # Application flow documentation
└── README.md
```