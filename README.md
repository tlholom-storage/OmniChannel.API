# Omni Channel Client Hub API

A .NET Core Web API built on **.NET 10** that provides backend services for client management, analytics, and system monitoring. The API exposes RESTful endpoints for CRUD operations and uses real-time communication to broadcast system events.

---

## Technology Stack

- **.NET 10**
- **ASP.NET Core Web API**
- **Entity Framework Core**
- **SignalR**
- **Swagger / OpenAPI**

---

## Key Features

### Client Management (CRUD)
- Create, read, update, and delete client records
- Entity Framework Core is used for data access and persistence
- Clean separation of concerns using controllers and services

### Real-Time System Logs
- **SignalR** is used to broadcast system and application logs
- Connected clients receive live updates without polling
- Useful for monitoring background processes and system activity

### API Documentation
- **Swagger** is enabled for interactive API exploration and testing
- Automatically generated OpenAPI specification

Access Swagger UI here:

🔗 **Swagger UI**  
https://net-core-omni-channel-client-hub.azurewebsites.net/index.html


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
