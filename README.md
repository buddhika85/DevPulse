
 # DevPulse — Distributed Engineering Productivity Platform

DevPulse is a cloud‑hosted engineering productivity platform built on .NET 8, Azure, and event‑driven microservices. It demonstrates real‑world distributed systems design, secure authentication, containerized deployments, and production‑grade CI/CD automation.

---

## Code Quality (SonarCloud)

[![Quality Gate](https://sonarcloud.io/api/project_badges/quality_gate?project=buddhika85_DevPulse)](https://sonarcloud.io/summary/new_code?id=buddhika85_DevPulse)
[![SonarCloud](https://sonarcloud.io/images/project_badges/sonarcloud-light.svg)](https://sonarcloud.io/summary/new_code?id=buddhika85_DevPulse)

DevPulse is continuously scanned using SonarCloud to ensure high standards of reliability, maintainability, and security across all microservices.

| Metric | Status |
|--------|--------|
| **Bugs** | [![Bugs](https://sonarcloud.io/api/project_badges/measure?project=buddhika85_DevPulse&metric=bugs)](https://sonarcloud.io/summary/new_code?id=buddhika85_DevPulse) |
| **Code Smells** | [![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=buddhika85_DevPulse&metric=code_smells)](https://sonarcloud.io/summary/new_code?id=buddhika85_DevPulse) |
| **Vulnerabilities** | [![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=buddhika85_DevPulse&metric=vulnerabilities)](https://sonarcloud.io/summary/new_code?id=buddhika85_DevPulse) |
| **Maintainability** | [![Maintainability](https://sonarcloud.io/api/project_badges/measure?project=buddhika85_DevPulse&metric=sqale_rating)](https://sonarcloud.io/summary/new_code?id=buddhika85_DevPulse) |
| **Reliability** | [![Reliability](https://sonarcloud.io/api/project_badges/measure?project=buddhika85_DevPulse&metric=reliability_rating)](https://sonarcloud.io/summary/new_code?id=buddhika85_DevPulse) |
| **Security** | [![Security](https://sonarcloud.io/api/project_badges/measure?project=buddhika85_DevPulse&metric=security_rating)](https://sonarcloud.io/summary/new_code?id=buddhika85_DevPulse) |
| **Technical Debt** | [![Technical Debt](https://sonarcloud.io/api/project_badges/measure?project=buddhika85_DevPulse&metric=sqale_index)](https://sonarcloud.io/summary/new_code?id=buddhika85_DevPulse) |

🔗 **View full analysis:**  
https://sonarcloud.io/summary/new_code?id=buddhika85_DevPulse

---

## 1. Architecture Overview

DevPulse follows a distributed microservices architecture with strict service boundaries, database‑per‑service design, and asynchronous communication via Azure Service Bus. Each service is independently deployable, containerized, and versioned.

### Microservices
- User API — identity, roles, user lifecycle  
- Task API — task creation, updates, assignments  
- Mood API — mood entries, soft deletes  
- Journal API — journal entries, feedback visibility  
- Journal Link API — task ↔ journal linking (Cosmos DB)  
- Orchestrator API — workflow coordination and cross‑service operations  

### Architectural Patterns
- CQRS with MediatR  
- Repository + Unit of Work  
- Event‑Driven Messaging (Azure Service Bus)  
- API Gateway via Azure API Management  
- Zero‑trust authentication with Microsoft Entra External ID  
- Distributed tracing with Serilog + Application Insights  

---

## 2. Authentication & Authorization

DevPulse uses OAuth 2.0 Authorization Code Flow with PKCE via Microsoft Entra External ID.

### Frontend
- MSAL.js for login, token caching, and silent refresh  
- Angular interceptors for secure API calls  

### Backend
- Validates Entra tokens  
- Exchanges frontend token for backend‑issued JWT with role claims  
- Enforces authorization via policy‑based checks  

---

## 3. Data Storage

Each microservice owns its own database.

| Service            | Storage Type |
|-------------------|--------------|
| User API          | Azure SQL    |
| Task API          | Azure SQL    |
| Mood API          | Azure SQL    |
| Journal API       | Azure SQL    |
| Journal Link API  | Azure Cosmos DB |
| Orchestrator      | Stateless    |

Cosmos DB is used for high‑volume, flexible linking between journals and tasks.

---

## 4. Messaging & Workflows

Azure Service Bus enables asynchronous, reliable communication.

Examples:
- Task updates → notify Journal Link service  
- Journal creation → trigger Orchestrator workflows  
- Daily summary → Azure Functions (Timer Trigger)  

Messages follow a contract‑first schema with versioning.

---

## 5. CI/CD Pipeline

Azure DevOps pipelines implement full automation.

### Build Stage
- Restore, build, and test (.NET 8)  
- Run xUnit tests with Moq + FluentAssertions  
- Publish test results  

### Containerization
- Docker build per microservice  
- Push images to Azure Container Registry  

### Deployment
- Deploy to Azure App Service / Azure Container Apps  
- Multi‑stage YAML pipelines  

---

## 6. Frontend

Angular 20 SPA with:
- MSAL.js authentication  
- Angular Material UI  
- Role‑based navigation  
- Secure API calls with PKCE tokens  

Hosted on Azure Static Web Apps.

---

## 7. Local Development

### Prerequisites
- .NET 8 SDK  
- Node 20+  
- Docker Desktop  
- Azure CLI  

### Running Services
```bash
docker-compose up --build


### Running Frontend
cd frontend
npm install
ng serve

---

## 8. Live Demo & Repository

Live Demo (Overview Page):  
https://nice-river-045c3a100.3.azurestaticapps.net/overview

Demo Video:  
https://www.youtube.com/watch?v=KQ2l7VS5r64
