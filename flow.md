# Application Flow

## 1. Authentication & Registration Flow

1. User calls `POST /api/auth/register` with email, password, first name, and last name
2. System creates an Identity user and assigns the **Employee** role
3. System creates an `Employee` profile linked to the Identity user
4. JWT token is returned with `EmployeeId` claim
5. User calls `POST /api/auth/login` to authenticate
6. System validates credentials and returns a JWT token

## 2. Student Management Flow

1. Employee creates a student profile via `POST /api/students`
2. Student can view/update/delete their profile via `StudentsController`
3. Student enrolls in courses via `POST /api/enrollments`
4. Student views enrolled courses and downloads content
5. Student submits homework solutions via `POST /api/HomeworkSubmissions`

## 3. Instructor Management Flow

1. Employee creates an instructor profile via `POST /api/instructors`
2. Instructor creates courses via `POST /api/courses`
3. Instructor uploads course content (PDFs, images, videos) via `POST /api/CourseContents/course/{courseId}`
4. Instructor creates homework assignments with deadlines and total marks via `POST /api/Homeworks/course/{courseId}`
5. Instructor updates homework deadlines/details via `PUT /api/Homeworks/{id}`
6. Instructor uploads solution files via `POST /api/HomeworkSolutions`
7. Instructor grades submissions with per-question marks via `POST /api/HomeworkQuestionMarks`

## 4. Employee Administration Flow

1. Employee manages other employee profiles via `EmployeesController`
2. Employee manages advertisements via `AdvertisementsController`
3. Employee oversees system-wide operations

## 5. Course Content Flow

1. Instructor uploads content with a `ContentType` (Pdf, Image, Video)
2. `ContentTypeHandlerFactory` selects the appropriate handler
3. Handler validates and processes the file
4. File is stored locally under `Storage/`
5. Domain event `CourseContentCreated` is published
6. Write database is updated
7. Background service processes the event
8. Read database is updated via strategy pattern

## 6. Homework Flow

1. Instructor creates homework with question file, deadline, and total marks
2. Homework is saved to write database
3. Domain event `HomeworkCreated` is published
4. Student views available homework via read database
5. Student submits solution PDF before deadline
6. Instructor uploads official solution file
7. Instructor grades submission with per-question marks
8. All changes trigger domain events for read database sync

## 7. Data Flow: Write to Read Database

1. Command/Query modifies write database (`AppDbContext`)
2. Domain event is created and stored in `DomainEvents` table
3. Background `DomainEventProcessorService` polls for unprocessed events
4. Factory selects appropriate strategy based on `EventType`
5. Strategy transforms write model to read model
6. Read model is saved to `AppReadDbContext`
7. Event is marked as processed

## 8. Query Flow

1. Client sends GET request
2. Controller uses MediatR to send query
3. Query handler uses read repository
4. Read repository queries optimized read models from `AppReadDbContext`
5. DTO is returned to client
