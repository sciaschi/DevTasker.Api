# \# DevTasker API

# 

# DevTasker is a layered ASP.NET Core \*\*Web API\*\* for managing:

# 

# \- \*\*Projects\*\*

# \- \*\*Tasks (Task Items)\*\*

# \- \*\*Work logs\*\* (time tracking and comments)

# 

# It’s designed to be consumed by a frontend (e.g. Angular) and includes:

# 

# \- \*\*MySQL\*\* persistence via Entity Framework Core  

# \- \*\*OpenAPI\*\* metadata (for generating clients / API docs)  

# \- \*\*SignalR\*\* for real-time notifications  

# \- Centralized \*\*error handling\*\* with consistent JSON responses  

# \- Optional \*\*Splunk\*\* logging

# 

# ---

# 

# \## Project structure

# 

# ```text

# DevTasker.Api.slnx

# ├─ DevTasker.Api/            # ASP.NET Core Web API (controllers, DTOs, middleware, hubs)

# │  ├─ Controllers/           # Projects, Tasks, WorkLogs controllers

# │  ├─ Dto/                   # Request/response DTOs

# │  ├─ Database/              # SQL scripts (MySQL + MSSQL)

# │  ├─ Hubs/                  # SignalR NotificationHub

# │  ├─ Middleware/            # ExceptionHandlingMiddleware

# │  └─ appsettings\*.json      # Configuration

# ├─ DevTasker.Domain/         # Domain entities, service interfaces, domain exceptions

# ├─ DevTasker.Infrastructure/ # EF Core DbContext + repositories

# ├─ DevTasker.Test/           # xUnit tests for domain + infrastructure

# ├─ Dockerfile

# └─ docker-compose.yml

# ```

# 

# ---

# 

# \## Prerequisites

# 

# \- \[.NET SDK \*\*9.0\*\*](https://dotnet.microsoft.com/)

# \- \*\*MySQL 8.x\*\* (recommended, used by default via `UseMySQL`)

# \- (Optional) \*\*SQL Server\*\* if you prefer, with provided schema script

# \- (Optional) \*\*Docker\*\* and \*\*Docker Compose\*\* if you want containerized deployment

# \- (Optional) \*\*Splunk\*\* instance with HTTP Event Collector (HEC) enabled

# 

# ---

# 

# \## Getting started

# 

# \### 1. Clone the repository

# 

# ```bash

# git clone https://github.com/<your-username>/DevTasker.Api.git

# cd DevTasker.Api

# ```

# 

# \### 2. Configure the database

# 

# The API is wired to use \*\*MySQL\*\* by default:

# 

# ```csharp

# // Program.cs

# builder.Services.AddDbContext<DevTaskerDbContext>(options =>

# &nbsp;   options.UseMySQL(builder.Configuration.GetConnectionString("MySQLConnection")));

# ```

# 

# There are ready-made SQL scripts in `DevTasker.Api/Database/`:

# 

# \- `CreateMySQLDB.sql`

# \- `CreateMSSQLDB.sql`

# \- `Truncate DB.sql`

# 

# \#### MySQL (recommended / default)

# 

# 1\. Run the script in your MySQL server:

# 

# &nbsp;  ```sql

# &nbsp;  -- in DevTasker.Api/Database/CreateMySQLDB.sql

# &nbsp;  CREATE DATABASE IF NOT EXISTS devtasker;

# &nbsp;  USE devtasker;

# &nbsp;  -- tables: projects, task\_items, work\_logs ...

# &nbsp;  ```

# 

# 2\. Update your connection string in `DevTasker.Api/appsettings.Development.json`:

# 

# &nbsp;  ```jsonc

# &nbsp;  {

# &nbsp;    "ConnectionStrings": {

# &nbsp;      "MySQLConnection": "Server=localhost;Port=3306;Database=devtasker;User=<user>;Password=<password>;"

# &nbsp;    }

# &nbsp;  }

# &nbsp;  ```

# 

# 3\. For production, either:

# &nbsp;  - put the connection string in `appsettings.json`, or  

# &nbsp;  - use environment variables (e.g. `ConnectionStrings\_\_MySQLConnection=<...>` when running in containers).

# 

# > 🔒 \*\*Security note:\*\* don’t commit real passwords or tokens to Git. Treat `appsettings.\*.json` as secret if it contains credentials.

# 

# \#### SQL Server (optional)

# 

# If you’d rather use SQL Server:

# 

# 1\. Run `DevTasker.Api/Database/CreateMSSQLDB.sql` against your SQL Server instance.

# 2\. Change the DbContext configuration in `Program.cs` to use `UseSqlServer(...)` instead of `UseMySQL(...)`.

# 3\. Point the connection string at your SQL Server database (e.g. `DevTaskerDB`).

# 

# ---

# 

# \### 3. Configure logging (optional, Splunk)

# 

# `Program.cs` uses \*\*Serilog\*\* with the Splunk sink:

# 

# ```csharp

# Log.Logger = new LoggerConfiguration()

# &nbsp;   .MinimumLevel.Information()

# &nbsp;   .Enrich.FromLogContext()

# &nbsp;   .WriteTo.Console()

# &nbsp;   .WriteTo.EventCollector(

# &nbsp;       splunkHost: configuration\["Splunk:Host"],

# &nbsp;       eventCollectorToken: configuration\["Splunk:Token"],

# &nbsp;       index: "testindex",

# &nbsp;       uriPath: "services/collector/event")

# &nbsp;   .CreateLogger();

# ```

# 

# Configure Splunk either in `appsettings\*.json`:

# 

# ```jsonc

# {

# &nbsp; "Splunk": {

# &nbsp;   "Host": "http://your-splunk-host:8088",

# &nbsp;   "Token": "<your-hec-token>"

# &nbsp; }

# }

# ```

# 

# or via environment variables:

# 

# ```bash

# export Splunk\_\_Host=http://your-splunk-host:8088

# export Splunk\_\_Token=<your-hec-token>

# ```

# 

# If you don’t have Splunk, you can leave these empty—the app will still run and log to console.

# 

# ---

# 

# \### 4. Configure CORS (for your frontend)

# 

# CORS is configured in `Program.cs` with a named policy `AllowFrontend`:

# 

# ```csharp

# builder.Services.AddCors(options =>

# {

# &nbsp;   options.AddPolicy("AllowFrontend", policy =>

# &nbsp;   {

# &nbsp;       policy.WithOrigins("http://localhost:4200", "http://localhost:5173")

# &nbsp;             .AllowAnyHeader()

# &nbsp;             .AllowAnyMethod()

# &nbsp;             .AllowCredentials();

# &nbsp;   });

# });

# ```

# 

# If your frontend runs on a different origin (e.g. `http://localhost:3000` or a real domain), add it here.

# 

# ---

# 

# \## Running the API (local)

# 

# From the \*\*solution root\*\*:

# 

# ```bash

# dotnet restore

# dotnet build

# dotnet run --project DevTasker.Api/DevTasker.Api.csproj

# ```

# 

# ASP.NET will print the listening URLs in the console, e.g.:

# 

# ```text

# Now listening on: http://localhost:5261

# Now listening on: https://localhost:7xxx

# ```

# 

# By default in \*\*Development\*\*:

# 

# \- OpenAPI document: `GET /openapi/v1.json`

# \- CORS enabled for the configured frontend origins

# \- Developer exception page is enabled

# 

# ---

# 

# \## Running with Docker

# 

# There is a `Dockerfile` (for the API) and a `docker-compose.yml` (which also configures a Splunk container).

# 

# 1\. Copy `.env.example` to `.env` and fill in your values:

# 

# &nbsp;  ```bash

# &nbsp;  cp .env.example .env

# &nbsp;  ```

# 

# &nbsp;  Example environment variables you may need to set:

# 

# &nbsp;  ```env

# &nbsp;  DB\_CONNECTION\_STRING=Server=host;Port=3306;Database=devtasker;User=user;Password=pass;

# &nbsp;  SPLUNK\_HOST=http://your-splunk:8088

# &nbsp;  SPLUNK\_HEC\_TOKEN=<your-hec-token>

# &nbsp;  SPLUNK\_ADMIN\_PASSWORD=<your-splunk-admin-password>

# &nbsp;  ```

# 

# 2\. Start the stack:

# 

# &nbsp;  ```bash

# &nbsp;  docker compose up --build

# &nbsp;  ```

# 

# &nbsp;  - The API will be exposed on the ports mapped in `docker-compose.yml` (e.g. `http://localhost:8080`).

# &nbsp;  - Splunk Web \& HEC will be exposed on the ports specified in the compose file.

# 

# ---

# 

# \## API overview

# 

# Base path for all routes: `/api`

# 

# \### Projects

# 

# \*\*Routes\*\*

# 

# \- `GET /api/projects/all`  

# &nbsp; Returns all projects as `ProjectDto\[]`.

# 

# \- `GET /api/projects/all/details`  

# &nbsp; Returns all projects including their tasks (detailed view).

# 

# \- `GET /api/projects/{id}`  

# &nbsp; Returns a single project.

# 

# \- `GET /api/projects/{id}/details`  

# &nbsp; Returns a single project with its task details.

# 

# \- `POST /api/projects/create`  

# &nbsp; Creates a project.

# 

# \- `PUT /api/projects/update`  

# &nbsp; Updates a project.

# 

# \- `PATCH /api/projects/{id}/archive`  

# &nbsp; Toggles the `is\_archived` flag for a project.

# 

# \*\*Create project – example\*\*

# 

# ```http

# POST /api/projects/create

# Content-Type: application/json

# ```

# 

# ```json

# {

# &nbsp; "name": "New Project",

# &nbsp; "description": "Optional description"

# }

# ```

# 

# \*\*Update project – example\*\*

# 

# ```http

# PUT /api/projects/update

# Content-Type: application/json

# ```

# 

# ```json

# {

# &nbsp; "project\_id": 1001,

# &nbsp; "name": "Renamed Project",

# &nbsp; "description": "Updated description"

# }

# ```

# 

# ---

# 

# \### Tasks

# 

# Base path: `/api/tasks`

# 

# \*\*Routes\*\*

# 

# \- `GET /api/tasks/all`  

# &nbsp; Returns all tasks.

# 

# \- `GET /api/tasks/all/details`  

# &nbsp; Returns all tasks with additional details.

# 

# \- `GET /api/tasks/project/{projectId}`  

# &nbsp; Returns all tasks for a given project.

# 

# \- `GET /api/tasks/project/{projectId}/details`  

# &nbsp; Returns detailed tasks for a given project.

# 

# \- `GET /api/tasks/{taskId}`  

# &nbsp; Returns a single task.

# 

# \- `GET /api/tasks/{taskId}/details`  

# &nbsp; Returns a task with detailed info.

# 

# \- `POST /api/tasks/create`  

# &nbsp; Creates a new task.

# 

# \- `PUT /api/tasks/update`  

# &nbsp; Updates a task.

# 

# \- `PATCH /api/tasks/{taskId}/status`  

# &nbsp; Updates the \*\*status\*\* of a task.

# 

# \- `PATCH /api/tasks/{taskId}/priority`  

# &nbsp; Updates the \*\*priority\*\* of a task.

# 

# \*\*Enums (conceptually)\*\*

# 

# In JSON, `status` and `priority` are sent as integers:

# 

# ```csharp

# public enum TaskItemStatus

# {

# &nbsp;   Todo = 0,

# &nbsp;   InProgress = 1,

# &nbsp;   Done = 2,

# &nbsp;   Blocked = 3

# }

# 

# // Example priority enum in Domain:

# public enum TaskItemPriority

# {

# &nbsp;   VeryHigh = 0,

# &nbsp;   High     = 1,

# &nbsp;   Medium   = 2,

# &nbsp;   Low      = 3,

# &nbsp;   VeryLow  = 4

# }

# ```

# 

# \*\*Create task – example\*\*

# 

# ```http

# POST /api/tasks/create

# Content-Type: application/json

# ```

# 

# ```json

# {

# &nbsp; "project\_id": 1001,

# &nbsp; "title": "Implement projects endpoint",

# &nbsp; "description": "Implement CRUD and wire it to the frontend",

# &nbsp; "status": 0,

# &nbsp; "priority": 2,

# &nbsp; "due\_date": "2025-12-31T00:00:00Z",

# &nbsp; "completed\_at": null

# }

# ```

# 

# \*\*Update task status – example\*\*

# 

# ```http

# PATCH /api/tasks/1/status

# Content-Type: application/json

# ```

# 

# ```json

# {

# &nbsp; "status": 1

# }

# ```

# 

# \*\*Update task priority – example\*\*

# 

# ```http

# PATCH /api/tasks/1/priority

# Content-Type: application/json

# ```

# 

# ```json

# {

# &nbsp; "task\_id": 1,

# &nbsp; "priority": 0

# }

# ```

# 

# ---

# 

# \### Work logs

# 

# Base path: `/api/worklogs`

# 

# \*\*Routes\*\*

# 

# \- `POST /api/worklogs/create`  

# &nbsp; Create a new work log entry for a task.

# 

# \- `GET /api/worklogs/task/{taskItemId}`  

# &nbsp; Get all work logs for a specific task.

# 

# \- `GET /api/worklogs/{workLogId}`  

# &nbsp; Get a single work log by ID.

# 

# \*\*Create work log – example\*\*

# 

# ```http

# POST /api/worklogs/create

# Content-Type: application/json

# ```

# 

# ```json

# {

# &nbsp; "task\_item\_id": 1,

# &nbsp; "started\_at": "2025-12-08T09:00:00Z",

# &nbsp; "ended\_at": "2025-12-08T11:30:00Z",

# &nbsp; "comment": "Initial implementation and unit tests"

# }

# ```

# 

# ---

# 

# \## Real-time notifications (SignalR)

# 

# The API exposes a \*\*SignalR Hub\*\* at:

# 

# \- `/hubs/notifications`

# 

# Events that are broadcast:

# 

# \- `"ProjectCreated"` – sent when a project is created

# \- `"ProjectUpdated"` – sent when a project is updated or archived

# \- `"TaskCreated"` – sent when a task is created

# \- `"TaskUpdated"` – sent when a task is updated (status, priority, etc.)

# \- `"WorkLogCreated"` – sent when a work log is created

# 

# \*\*Example (TypeScript with @microsoft/signalr):\*\*

# 

# ```ts

# import \* as signalR from "@microsoft/signalr";

# 

# const connection = new signalR.HubConnectionBuilder()

# &nbsp; .withUrl("http://localhost:5261/hubs/notifications", {

# &nbsp;   withCredentials: true

# &nbsp; })

# &nbsp; .build();

# 

# connection.on("ProjectCreated", project => {

# &nbsp; console.log("Project created:", project);

# });

# 

# connection.on("TaskUpdated", task => {

# &nbsp; console.log("Task updated:", task);

# });

# 

# await connection.start();

# ```

# 

# ---

# 

# \## Error handling

# 

# All unhandled exceptions and known domain exceptions flow through `ExceptionHandlingMiddleware`.

# 

# Error responses are wrapped in a consistent shape:

# 

# ```json

# {

# &nbsp; "error": {

# &nbsp;   "code": "UNEXPECTED\_ERROR | VALIDATION\_ERROR | NOT\_FOUND | CONFLICT | FORBIDDEN",

# &nbsp;   "message": "Human readable description",

# &nbsp;   "details": { /\* optional extra info \*/ }

# &nbsp; }

# }

# ```

# 

# \- Validation errors (e.g. missing required fields) return \*\*HTTP 400\*\*.  

# \- Missing resources return \*\*HTTP 404\*\*.  

# \- Conflicts (e.g. duplicate/invalid state) return \*\*HTTP 409\*\*.  

# \- Forbidden operations return \*\*HTTP 403\*\*.  

# \- All others are treated as unexpected errors (\*\*HTTP 500\*\*).

# 

# ---

# 

# \## Running tests

# 

# From the solution root:

# 

# ```bash

# dotnet test

# ```

# 

# The `DevTasker.Test` project uses:

# 

# \- \*\*xUnit\*\*  

# \- \*\*Moq\*\*  

# \- \*\*EF Core InMemory\*\* provider  

# 

# to exercise services and repositories.

# 

# ---

# 

# \## Notes / tips

# 

# \- \*\*OpenAPI\*\*: In Development, the OpenAPI document is available at `/openapi/v1.json`. You can use that to generate client SDKs or plug into tools like Postman, Swagger UI, or Scalar.

# \- \*\*Ports\*\*: Exact ports depend on `launchSettings.json` or your hosting environment—check the console output when you run `dotnet run`.

# \- \*\*Frontend\*\*: This project is API-only; integrate it into whatever frontend you like (Angular, React, Vue, etc.). Just remember to update the CORS origins.



