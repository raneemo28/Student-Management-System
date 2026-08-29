# Student Management System

A Clean Architecture ASP.NET Core Web API for managing students, instructors, courses, course content, and homework with CQRS, domain events, and a separate read database.

## Architecture

- **App.APIs** — Controllers, authentication, and DI setup
- **App.Application** — Services, CQRS commands/queries, validators, repositories, DTOs, and domain event abstractions
- **App.infra** — EF Core DbContexts, repositories, file storage, strategy implementations, and background services
- **App.domain** — Entities, read models, and domain events

## Key Features

### Authentication & Authorization
- JWT-based register/login/logout
- Registration creates an **Employee** user profile
- Role-based access control: **Student**, **Instructor**, or **Employee**
- Each role has distinct permissions (see below)

### Students
- View, create, update, and delete student profiles
- Enroll in courses
- View and download course content
- Submit homework solutions

### Instructors
- View, create, update, and delete instructor profiles
- Create, update, and delete courses
- Manage course content (upload, update, delete files such as PDFs, images, and videos)
- Create and manage homework assignments
- Upload solution files for homework
- Grade student submissions with per-question marks

### Employees
- View, create, update, and delete employee profiles
- Manage advertisements (create, update, delete, view)
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

## Databases

- **Write Database** — `StudentDomainDb` (students, instructors, courses, enrollments, course contents, homeworks, submissions, solutions, question marks, domain events)
- **Identity Database** — `StudentAuthDb` (ASP.NET Core Identity users and roles)
- **Read Database** — `StudentReadDb` (denormalized read models for queries)

## Getting Started

1. Configure connection strings in `App.APIs/appsettings.json`
2. Run migrations:
    ```bash
    dotnet ef database update --context AppDbContext
    dotnet ef database update --context AuthDbContext
    dotnet ef database update --context AppReadDbContext
    ```
3. Run the application:
    ```bash
    dotnet run --project App.APIs
    ```
