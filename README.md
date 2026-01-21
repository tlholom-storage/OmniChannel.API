# .NET Core Web API

This project is a **.NET 10 Web API** that provides backend services for managing application data and broadcasting system activity. It exposes RESTful endpoints for CRUD operations and uses **SignalR** to stream system logs to connected clients in real time.

---

## Features

- **.NET 10 Web API**
- **Entity Framework Core**
  - Code-first approach
  - CRUD operations via REST endpoints
- **SignalR**
  - Real-time broadcasting of system logs
  - Supports multiple connected clients
- **Clean, layered architecture**
- **Extensible and easy to maintain**

---

## Technology Stack

- .NET 10
- ASP.NET Core Web API
- Entity Framework Core
- SignalR
- SQL Server (or configurable relational database)

---

## API Overview

The API exposes endpoints for managing domain entities using standard HTTP verbs:

- `GET` – Retrieve records
- `POST` – Create new records
- `PUT` – Update existing records
- `DELETE` – Remove records

All database operations are handled using **Entity Framework Core**.

---

## Real-Time Logging with SignalR

The API includes a SignalR hub responsible for broadcasting system logs such as:

- CRUD operations
- Background processing events
- Application-level notifications

Connected clients receive log messages instantly without polling.

---

## Running the Application Locally

### Prerequisites
- .NET 10 SDK
- SQL Server (or compatible database)

### Steps

Restore dependencies:

```bash
dotnet restore
```

Apply database migrations:
```bash
dotnet ef database update
```


Run the API:

```bash
dotnet run
```


The API will be available at:

https://localhost:5001

---

# Configuration

Database connection strings are configured in appsettings.json

SignalR endpoints are mapped in the application startup configuration

# Notes

The API is designed to integrate with frontend applications such as Angular or React.

SignalR allows real-time visibility into system activity and operational logs.

The project structure supports future enhancements and additional services.

## License

This project is intended for internal, learning, or demonstration purposes.