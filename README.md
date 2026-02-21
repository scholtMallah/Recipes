
# Recipes
- **Backend**: ASP.NET Core 8 Web API (`http://localhost:8080`)
---

## Getting Started

You can run the project in several ways depending on your environment and preference.

---

### 1. Using Bash (Linux/macOS)

```bash
chmod +x start.sh
./start.sh
```

Starts:
- .NET backend at `http://localhost:8080`
---

### 2. Using PowerShell (Windows)

```powershell
.\start.ps1
```

Starts:
- .NET backend at `http://localhost:8080`

> If blocked by execution policy:
> ```powershell
> Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass
> ```

---

###  3. Using Docker Compose (Linux & Windows)

```bash
docker compose up --build
```

Builds and serves:
- ASP.NET backend

Make sure Docker & Docker Compose are installed.

Access:
- API: [http://localhost:8080](http://localhost:8080)
- Swagger: [http://localhost:8080/swagger](http://localhost:8080/swagger)

---

### 4. Manual Startup (VS Code / Visual Studio)

Start backend:

```bash
cd Recipes
dotnet --project run --urls=http://localhost:8080
```
---

## Port Mapping

| Component | Port   | URL                            |
|-----------|--------|--------------------------------|
| Backend   | 8080   | http://localhost:8080          |
| Swagger   | 8080   | http://localhost:8080/swagger  |

---

## Run Tests

To run backend unit tests:

```bash
dotnet test
```

---

## Authentication

The API uses JWT Bearer authentication.

### 1. Obtain a token

Call:

POST /api/auth/token

Example using curl:

```bash
curl -X POST http://localhost:8080/api/auth/token \
  -H "Content-Type: application/json" \
  -d '{
    "clientId": "your-client-id",
    "clientSecret": "your-client-secret"
  }'
```
### 1. Obtain a token from with-in SWAGGER

---

## 🏗 Architecture Overview

- Controllers → API layer
- Services → Business logic (DTO-free)
- Repository → JSON file-backed store
- FluentValidation → Input validation
- AutoMapper → DTO ↔ Domain mapping
- JWT → Authentication

---

## Assumptions

- Data is stored in a JSON file (no external database).
- API is secure by default using JWT authentication, only read API endpoints are open.
- Search endpoint filters by name and ingredients only.
- Since this is a code assesment sample clientId and secret are stored in app settings. This could have also been in docker secrets for example but since the running method is unknown to me appsettings was chosen.
