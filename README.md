<img src="/frontend/public/logo-hestia.png" width="128"/>
Hestia is a Nuxt mobile web-app that helps you organise and manage your collocation.
The backend is coded in C#.
Hestia can help with organising chores, budget and alot more by giving you better ways to communicate with your room-mates!

## Build
To build and deploy the Hestia app localy, you can use the docker compose provided with the repository. Simply use `docker compose up` in the docker folder and everything should start up.
The database of the project is hosted so you will need credentials to connect to it.

# Backend

## Overview

The backend of Hestia is a RESTful API built with ASP.NET Core 8.0. It serves as the primary interface between the mobile application and the underlying PostgreSQL database, providing secure and performant endpoints for data access and manipulation.

## Technical stack

**Framework** : ASP.NET Core 8.0 (C#)
**Authentication** : JWT (JSON Web Tokens) with OAuth 2.0 (Google Sign-In)
**Database** : Postgresql (with Entity Framework core)
**Caching** : LazyCache (in-memory caching)
**API Documentation** : Swagger (Open API)
**Testing** : xUnit, Moq
**Containerization** : Docker, Docker Compose
**Hosting** : Ubuntu virtual machine (Epitech VM)
**CI/CD** : Github Action

## Key Features

- Dependency Injection using .NET built-in DI container
- Global Error Handling using Problem Details (RFC 7807)
- Secure Authentication using JWT and Google OAuth 2.0
- Optimized performance using caching (LazyCache in memory)
- Modular and Layered Architecture (API, Business, Data Access, Shared)
- Entity Framework Core for ORM and migrations
- Swagger (OpenAPI 3.0) for automatic API documentation
- Docker & Docker Compose support for local development
- Environment-based configuration loading (Development / Production)
- Unit testing with xUnit and mocking with Moq
- Clear separation of concerns with Clean Architecture principles
- Centralized Logging System

## Project Artchitecture

backend/
├── Api/
│   ├── Configuration/
│   ├── Controllers/
│   ├── ErrorHandler/
│   ├── appsettings.json
│   └── Program.cs
│
├── Business/
│   ├── Interfaces/
│   ├── Jwt/
│   ├── Mappers/
│   └── Services/
│
├── EntityFramework/
│   ├── Context/
│   ├── Migrations/
│   ├── Models/
│   └── Repositories/
│
├── Shared/
│   ├── Enums/
│   ├── Exceptions/
│   └── Models/
│       ├── DTO/
│       ├── Input/
│       ├── Output/
│       └── Update/
│
├── Dockerfile
│
└── Tests/

# Getting Started with the Backend

## Configuration File

Complete the file named `appsettings.json` with the info of the DB, google id etc...

The file is here :
```
Hestia/backend/Api/
```

## Running with Docker

If everything is properly configured, you can run the backend using Docker. Open a terminal and execute the following commands:

```bash
cd Hestia/docker
docker compose up
```

If the setup is successful, you should see the log message:

```
Connection successful!
```

The API will be available at:

- Base URL: `http://localhost:8080/api/`
- Swagger UI: `http://localhost:8080/swagger`

## Running Locally with Visual Studio

1. Open the solution file located at:

```
Hestia/backend/backend.sln
```

2. In Visual Studio, set the **Api** project as the startup project.
3. Start the application by pressing **F5** or clicking the green "Run" button.

## Running from the Terminal

To run the project manually via the .NET CLI:

```bash
cd Hestia/backend/Api
dotnet run
```

The API will be available at the default Kestrel endpoint

## Available environments

- `Development` (`appsettings.Development.json`)
- `Production` (`appsettings.Production.json`)

## Switching Environments

Go to Hestia/docker/compose/yml

You can change this line to either development or production :

```
environment:
    - ASPNETCORE_ENVIRONMENT=Development
```