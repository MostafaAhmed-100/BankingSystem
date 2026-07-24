# 🏦 Secure Banking System REST API

A highly optimized, secure, and robust Banking System backend built with **ASP.NET Core 8 Web API**. This system is engineered to handle critical financial operations with absolute data integrity, supporting features like optimistic concurrency, transactional audit trails, clean domain isolation, and role-based access control (Banker & Customer).

## 🚀 Tech Stack

| Layer | Technology |
| --- | --- |
| **Framework** | ASP.NET Core 8 Web API |
| **ORM** | Entity Framework Core (Pure Fluent API) |
| **Database** | SQL Server |
| **Auth** | ASP.NET Core Identity + JWT Bearer |
| **Architecture** | Clean Architecture + Repository Pattern + Unit of Work |
| **Object Mapping** | AutoMapper |
| **Input Validation** | Fluent Validation + C# 11 `required` modifiers |
| **Logging** | Serilog (File & Console Sinks, Transaction Tracking) |
| **Concurrency** | EF Core Optimistic Concurrency (`RowVersion`) |
| **Error Handling** | Custom Exception Middleware + Domain Exceptions |

## 🏗️ Project Structure

```text
├── Constants/                 # AppRoles, TransactionTypes
├── Controllers/               # API endpoints (Thin layer routing requests)
├── Data/                      # Database context, Entities, and DB Configurations
│   ├── Configurations/        # Fluent API configurations for absolute DB schema control
│   ├── models/                # EF Core models (Clean POCOs, zero Data Annotations)
│   └── AppDbContext.cs
├── DTOS/                      # Data Transfer Objects organized by domain
│   ├── AccountDTOs/
│   ├── Auth&IdentityDTOs/
│   ├── CreditCards&Loans/
│   ├── PaymentGateway/
│   ├── Transactions&Transfers/
│   ├── Validators/            # Fluent Validation rules isolated from DTOs
│   └── Shared/                # Shared wrappers (e.g., ApiResponseDto)
├── Exceptions/                # Domain-specific custom exceptions
├── Mappings/                  # AutoMapper Profiles (Domain-to-DTO transformation)
├── Middlewares/               # Custom pipeline (Global Exception Handling)
├── Migrations/                # EF Core migration history
└── Repository/                # Data access layer
    ├── GenericRepository/     # Base CRUD operations
    ├── SpecificRepository/    # Domain-specific queries (e.g., AccountRepository)
    └── UnitOfWork/            # Centralized transaction management (ACID properties)

```

---

## 👥 Roles & Permissions

| Role | Capabilities |
| --- | --- |
| `Banker` | Manages branches, registers new customers, reviews loan applications, and monitors overall banking activities. |
| `Customer` | Browses owned accounts, manages credit cards, requests loans, and performs secure deposits/withdrawals. |

---

## 🛡️ Core Banking Features & Security

### ⚡ Absolute Data Integrity (Concurrency)

To prevent "Double Spending" or race conditions when multiple transactions occur simultaneously, the `Account` entity is protected using **Optimistic Concurrency** via SQL Server `RowVersion`. If two requests attempt to modify the same balance at the exact same millisecond, the system safely aborts the conflicting request, ensuring no funds are lost or duplicated.

### 📜 Immutable Transaction History (Audit Trail)

Balances are never modified without a trace. Every financial movement (Deposit / Withdraw) is structurally logged as an immutable `Transaction` record tied to the specific account, establishing a strict, bank-grade ledger.

### 🛡️ Clean Validation & Global Error Handling

The project implements a robust observability and validation architecture:

* **Strict Input Validation:** DTOs are protected using C# 11 `required` properties alongside `FluentValidation`.
* **Centralized Exception Middleware:** Captures unhandled exceptions, logs them via **Serilog**, prevents app crashes, and maps them to standardized HTTP responses.
* **Domain-Specific Exceptions:** Repetitive error returns in the Service Layer have been replaced with clean, domain-specific exceptions (e.g., `NotFoundException`, `ConflictException`, `BadRequestException`).

### 🗄️ Clean Database Architecture

The data layer relies completely on **Fluent API** (`IEntityTypeConfiguration`), stripping all Data Annotations from domain models. This prevents multiple cascade paths, explicitly handles many-to-many relationships (e.g., `CustomerBankers`), and precisely defines decimal scales (e.g., `decimal(18,2)`) to prevent rounding errors. All list endpoints execute memory-optimized paging natively at the database level.

---

## 📁 Unified Response Format

Every endpoint returns the exact same wrapper shape (`ApiResponseDto<T>`), making frontend or third-party integration (like E-Commerce Payment Gateways) seamless.

**Example: Success Response (200 OK)**

```json
{
  "isSuccess": true,
  "statusCode": 200,
  "message": "Operation successful.",
  "data": {
    "accountNumber": 123456,
    "balance": 5000.00
  }
}

```

---

## ⚙️ Configuration (`appsettings.json`)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=.\\SQLEXPRESS;Initial Catalog=BankingSystem;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;"
  },
  "Serilog": {
    "Using": [ "Serilog.Sinks.File" ],
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning"
      }
    },
    "WriteTo": [
      { "Name": "Console" },
      {
        "Name": "File",
        "Args": {
          "path": "Logs\\BankingSystemLogs.txt",
          "rollingInterval": "Day"
        }
      }
    ]
  },
  "Jwt": {
    "Key": "your_highly_secure_jwt_secret_key_here",
    "Issuer": "https://localhost:7132/",
    "Audience": "APISecureUser"
  }
}

```

---

## 🗄️ Database Setup

```bash
# Clone the repository
git clone [https://github.com/MostafaAhmed-100/Banking-System-API.git](https://github.com/MostafaAhmed-100/Banking-System-API.git)
cd Banking-System-API

# Restore dependencies and update the database
dotnet restore
dotnet ef database update

# Run the API
dotnet run

```

---

## 🙏 Mentorship

Special thanks to the following mentors for their continuous technical guidance, architectural advice, and support:

| Name | LinkedIn |
| --- | --- |
| **AbdALlatif Hossni** | [linkedin.com/in/abdallatif-hossni](https://www.linkedin.com/in/abdallatif-hossni-9217091b9/) |

---

## 📜 License

This project is open-source and available under the MIT License.
