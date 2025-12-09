# DevTasker API

DevTasker is a layered ASP.NET Core **Web API** for managing:

- **Projects**
- **Tasks (Task Items)**
- **Work logs** (time tracking and comments)

It’s designed to be consumed by a frontend (e.g. Angular) and includes:

- **MySQL** persistence via Entity Framework Core  
- **OpenAPI** metadata (for generating clients / API docs)  
- **SignalR** for real-time notifications  
- Centralized **error handling** with consistent JSON responses  
- Optional **Splunk** logging

---

## Project structure

```text
DevTasker.Api.slnx
├─ DevTasker.Api/            # ASP.NET Core Web API (controllers, DTOs, middleware, hubs)
│  ├─ Controllers/           # Projects, Tasks, WorkLogs controllers
│  ├─ Dto/                   # Request/response DTOs
│  ├─ Database/              # SQL scripts (MySQL + MSSQL)
│  ├─ Hubs/                  # SignalR NotificationHub
│  ├─ Middleware/            # ExceptionHandlingMiddleware
│  └─ appsettings*.json      # Configuration
├─ DevTasker.Domain/         # Domain entities, service interfaces, domain exceptions
├─ DevTasker.Infrastructure/ # EF Core DbContext + repositories
├─ DevTasker.Test/           # xUnit tests for domain + infrastructure
├─ Dockerfile
└─ docker-compose.yml
```

---

## Prerequisites

- [.NET SDK **9.0**](https://dotnet.microsoft.com/)
- **MySQL 8.x** (recommended, used by default via `UseMySQL`)
- (Optional) **SQL Server** if you prefer, with provided schema script
- (Optional) **Docker** and **Docker Compose** if you want containerized deployment
- (Optional) **Splunk** instance with HTTP Event Collector (HEC) enabled

---

## Getting started

### 1. Clone the repository

```bash
git clone https://github.com/<your-username>/DevTasker.Api.git
cd DevTasker.Api
```

### 2. Configure the database

The API is wired to use **MySQL** by default:

```csharp
// Program.cs
builder.Services.AddDbContext<DevTaskerDbContext>(options =>
    options.UseMySQL(builder.Configuration.GetConnectionString("MySQLConnection")));
```

There are ready-made SQL scripts in `DevTasker.Api/Database/`:

- `CreateMySQLDB.sql`
- `CreateMSSQLDB.sql`
- `Truncate DB.sql`

#### MySQL (recommended / default)

1. Run the script in your MySQL server:

   ```sql
   -- in DevTasker.Api/Database/CreateMySQLDB.sql
   CREATE DATABASE IF NOT EXISTS devtasker;
   USE devtasker;
   -- tables: projects, task_items, work_logs ...
   ```

2. Update your connection string in `DevTasker.Api/appsettings.Development.json`:

   ```jsonc
   {
     "ConnectionStrings": {
       "MySQLConnection": "Server=localhost;Port=3306;Database=devtasker;User=<user>;Password=<password>;"
     }
   }
   ```

3. For production, either:
   - put the connection string in `appsettings.json`, or  
   - use environment variables (e.g. `ConnectionStrings__MySQLConnection=<...>` when running in containers).

> 🔒 **Security note:** don’t commit real passwords or tokens to Git. Treat `appsettings.*.json` as secret if it contains credentials.

#### SQL Server (optional)

If you’d rather use SQL Server:

1. Run `DevTasker.Api/Database/CreateMSSQLDB.sql` against your SQL Server instance.
2. Change the DbContext configuration in `Program.cs` to use `UseSqlServer(...)` instead of `UseMySQL(...)`.
3. Point the connection string at your SQL Server database (e.g. `DevTaskerDB`).

---

### 3. Configure logging (optional, Splunk)

`Program.cs` uses **Serilog** with the Splunk sink:

```csharp
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.EventCollector(
        splunkHost: configuration["Splunk:Host"],
        eventCollectorToken: configuration["Splunk:Token"],
        index: "testindex",
        uriPath: "services/collector/event")
    .CreateLogger();
```

Configure Splunk either in `appsettings*.json`:

```jsonc
{
  "Splunk": {
    "Host": "http://your-splunk-host:8088",
    "Token": "<your-hec-token>"
  }
}
```

or via environment variables:

```bash
export Splunk__Host=http://your-splunk-host:8088
export Splunk__Token=<your-hec-token>
```

If you don’t have Splunk, you can leave these empty—the app will still run and log to console.

---

### 4. Configure CORS (for your frontend)

CORS is configured in `Program.cs` with a named policy `AllowFrontend`:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:4200", "http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});
```

If your frontend runs on a different origin (e.g. `http://localhost:3000` or a real domain), add it here.

---

## Running the API (local)

From the **solution root**:

```bash
dotnet restore
dotnet build
dotnet run --project DevTasker.Api/DevTasker.Api.csproj
```

ASP.NET will print the listening URLs in the console, e.g.:

```text
Now listening on: http://localhost:5261
Now listening on: https://localhost:7xxx
```

By default in **Development**:

- OpenAPI document: `GET /openapi/v1.json`
- CORS enabled for the configured frontend origins
- Developer exception page is enabled

---

## Running with Docker

There is a `Dockerfile` (for the API) and a `docker-compose.yml` (which also configures a Splunk container).

1. Copy `.env.example` to `.env` and fill in your values:

   ```bash
   cp .env.example .env
   ```

   Example environment variables you may need to set:

   ```env
   DB_CONNECTION_STRING=Server=host;Port=3306;Database=devtasker;User=user;Password=pass;
   SPLUNK_HOST=http://your-splunk:8088
   SPLUNK_HEC_TOKEN=<your-hec-token>
   SPLUNK_ADMIN_PASSWORD=<your-splunk-admin-password>
   ```

2. Start the stack:

   ```bash
   docker compose up --build
   ```

   - The API will be exposed on the ports mapped in `docker-compose.yml` (e.g. `http://localhost:8080`).
   - Splunk Web & HEC will be exposed on the ports specified in the compose file.

---

## API overview

Base path for all routes: `/api`

### Projects

**Routes**

- `GET /api/projects/all`  
  Returns all projects as `ProjectDto[]`.

- `GET /api/projects/all/details`  
  Returns all projects including their tasks (detailed view).

- `GET /api/projects/{id}`  
  Returns a single project.

- `GET /api/projects/{id}/details`  
  Returns a single project with its task details.

- `POST /api/projects/create`  
  Creates a project.

- `PUT /api/projects/update`  
  Updates a project.

- `PATCH /api/projects/{id}/archive`  
  Toggles the `is_archived` flag for a project.

**Create project – example**

```http
POST /api/projects/create
Content-Type: application/json
```

```json
{
  "name": "New Project",
  "description": "Optional description"
}
```

**Update project – example**

```http
PUT /api/projects/update
Content-Type: application/json
```

```json
{
  "project_id": 1001,
  "name": "Renamed Project",
  "description": "Updated description"
}
```

---

### Tasks

Base path: `/api/tasks`

**Routes**

- `GET /api/tasks/all`  
  Returns all tasks.

- `GET /api/tasks/all/details`  
  Returns all tasks with additional details.

- `GET /api/tasks/project/{projectId}`  
  Returns all tasks for a given project.

- `GET /api/tasks/project/{projectId}/details`  
  Returns detailed tasks for a given project.

- `GET /api/tasks/{taskId}`  
  Returns a single task.

- `GET /api/tasks/{taskId}/details`  
  Returns a task with detailed info.

- `POST /api/tasks/create`  
  Creates a new task.

- `PUT /api/tasks/update`  
  Updates a task.

- `PATCH /api/tasks/{taskId}/status`  
  Updates the **status** of a task.

- `PATCH /api/tasks/{taskId}/priority`  
  Updates the **priority** of a task.

**Enums (conceptually)**

In JSON, `status` and `priority` are sent as integers:

```csharp
public enum TaskItemStatus
{
    Todo = 0,
    InProgress = 1,
    Done = 2,
    Blocked = 3
}

// Example priority enum in Domain:
public enum TaskItemPriority
{
    VeryHigh = 0,
    High     = 1,
    Medium   = 2,
    Low      = 3,
    VeryLow  = 4
}
```

**Create task – example**

```http
POST /api/tasks/create
Content-Type: application/json
```

```json
{
  "project_id": 1001,
  "title": "Implement projects endpoint",
  "description": "Implement CRUD and wire it to the frontend",
  "status": 0,
  "priority": 2,
  "due_date": "2025-12-31T00:00:00Z",
  "completed_at": null
}
```

**Update task status – example**

```http
PATCH /api/tasks/1/status
Content-Type: application/json
```

```json
{
  "status": 1
}
```

**Update task priority – example**

```http
PATCH /api/tasks/1/priority
Content-Type: application/json
```

```json
{
  "task_id": 1,
  "priority": 0
}
```

---

### Work logs

Base path: `/api/worklogs`

**Routes**

- `POST /api/worklogs/create`  
  Create a new work log entry for a task.

- `GET /api/worklogs/task/{taskItemId}`  
  Get all work logs for a specific task.

- `GET /api/worklogs/{workLogId}`  
  Get a single work log by ID.

**Create work log – example**

```http
POST /api/worklogs/create
Content-Type: application/json
```

```json
{
  "task_item_id": 1,
  "started_at": "2025-12-08T09:00:00Z",
  "ended_at": "2025-12-08T11:30:00Z",
  "comment": "Initial implementation and unit tests"
}
```

---

## Real-time notifications (SignalR)

The API exposes a **SignalR Hub** at:

- `/hubs/notifications`

Events that are broadcast:

- `"ProjectCreated"` – sent when a project is created
- `"ProjectUpdated"` – sent when a project is updated or archived
- `"TaskCreated"` – sent when a task is created
- `"TaskUpdated"` – sent when a task is updated (status, priority, etc.)
- `"WorkLogCreated"` – sent when a work log is created

**Example (TypeScript with @microsoft/signalr):**

```ts
import * as signalR from "@microsoft/signalr";

const connection = new signalR.HubConnectionBuilder()
  .withUrl("http://localhost:5261/hubs/notifications", {
    withCredentials: true
  })
  .build();

connection.on("ProjectCreated", project => {
  console.log("Project created:", project);
});

connection.on("TaskUpdated", task => {
  console.log("Task updated:", task);
});

await connection.start();
```

---

## Error handling

All unhandled exceptions and known domain exceptions flow through `ExceptionHandlingMiddleware`.

Error responses are wrapped in a consistent shape:

```json
{
  "error": {
    "code": "UNEXPECTED_ERROR | VALIDATION_ERROR | NOT_FOUND | CONFLICT | FORBIDDEN",
    "message": "Human readable description",
    "details": { /* optional extra info */ }
  }
}
```

- Validation errors (e.g. missing required fields) return **HTTP 400**.  
- Missing resources return **HTTP 404**.  
- Conflicts (e.g. duplicate/invalid state) return **HTTP 409**.  
- Forbidden operations return **HTTP 403**.  
- All others are treated as unexpected errors (**HTTP 500**).

---

## Running tests

From the solution root:

```bash
dotnet test
```

The `DevTasker.Test` project uses:

- **xUnit**  
- **Moq**  
- **EF Core InMemory** provider  

to exercise services and repositories.

---

## Notes / tips

- **OpenAPI**: In Development, the OpenAPI document is available at `/openapi/v1.json`. You can use that to generate client SDKs or plug into tools like Postman, Swagger UI, or Scalar.
- **Ports**: Exact ports depend on `launchSettings.json` or your hosting environment—check the console output when you run `dotnet run`.
- **Frontend**: This project is API-only; integrate it into whatever frontend you like (Angular, React, Vue, etc.). Just remember to update the CORS origins.
