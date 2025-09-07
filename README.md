# WebhookService

A .NET 8-based Webhook Service API with SQL Server and Redis, containerized using Docker Compose.

## Setup

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/products/docker-desktop)
- [Git](https://git-scm.com/)

### Clone the Repository

### Configuration
- The main configuration is in `WebhookService.API/appsettings.json`.
- Connection strings and secrets are overridden by environment variables in `docker-compose.yml`.

### Build and Run (Docker Compose)
- This will start the API, SQL Server, and Redis containers.
- The API will be available at [http://localhost:8080](http://localhost:8080) (default port).

## Run Locally (Without Docker)
1. Ensure SQL Server and Redis are running locally.
2. Update `appsettings.json` with your local connection strings.
3. Run the API:

## Assumptions
- SQL Server and Redis are required for the service to function.
- The API expects a 256-bit base64-encoded master key for cryptographic operations.
- Default credentials and secrets in `docker-compose.yml` should be changed for production.
- The API listens on port 80 inside the container, mapped to 8080 on the host.

## Improvements
- **Security:** Use secure secrets management (e.g., Docker secrets, Azure Key Vault).
- **Testing:** Add unit and integration tests.
- **Documentation:** Expand API documentation (e.g., Swagger).
- **Scalability:** Add support for horizontal scaling and health checks.
- **CI/CD:** Integrate automated build and deployment pipelines.
- **Monitoring:** Add logging and monitoring (e.g., Prometheus, Grafana).

## Troubleshooting
- If SQL Server fails to start, ensure Docker has enough memory allocated.
- For connection issues, verify network settings and environment variables.

## UML Diagram
```plantuml
@startuml
!define RECTANGLE class

RECTANGLE WebhookServiceAPI [
  WebhookService.API
  (.NET 8)
]

RECTANGLE SQLServer [
  SQL Server
  (Docker Container)
]

RECTANGLE Redis [
  Redis
  (Docker Container)
]

WebhookServiceAPI -down-> SQLServer : "ConnectionStrings:Database"
WebhookServiceAPI -down-> Redis : "ConnectionStrings:Redis"

cloud "Docker Compose" {
  WebhookServiceAPI
  SQLServer
  Redis
}

@enduml
```

---
Feel free to contribute or open issues for bugs and feature requests!