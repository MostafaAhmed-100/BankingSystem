
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
| **Logging** | Serilog (File & Console Sinks, Transaction Tracking) |
| **Concurrency** | EF Core Optimistic Concurrency (`RowVersion`) |
| **Security** | Role-Based Authorization (Banker / Customer) |

## 🏗️ Project Structure

```text
├── Controllers/               # API endpoints (Thin layer routing requests)
├── Services/                  # Business logic & Financial rules 
├── Repository/
│   ├── GenericRepository/     # Base CRUD operations (with AsNoTracking support)
│   ├── SpecificRepository/    # Domain-specific queries (e.g., GetAccountWithHistory)
│   └── UnitOfWork/            # Centralized transaction management (ACID properties)
├── Entities/                  # EF Core models (Clean POCOs, zero Data Annotations)
├── DTOs/                      # Data Transfer Objects for Request/Response mapping
├── Configurations/            # Fluent API configurations for absolute DB schema control
├── Middlewares/               # Custom pipeline (Global Exception Handling)
├── Constants/                 # AppRoles, TransactionTypes
└── Data/
    └── AppDbContext           # Context and Migrations

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

### 🗄️ Clean Database Architecture

The data layer relies completely on **Fluent API** (`IEntityTypeConfiguration`), stripping all Data Annotations from domain models. This prevents multiple cascade paths, explicitly handles many-to-many relationships (e.g., `CustomerBankers`), and precisely defines decimal scales (e.g., `decimal(18,2)`) to prevent rounding errors.

### 🗑️ Soft Deletion Strategy

Critical financial entities (like Accounts, Cards, and Customers) are never hard-deleted from the database. A strict `IsActive` flag governs state changes to maintain referential integrity and historical reporting.

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
| **AbdALlatif Hossni** | [linkedin.com/in/abdallatif-hossni](https://www.google.com/search?q=https://www.linkedin.com/in/abdallatif-hossni-9217091b9/) |

---

## 📜 License

This project is open-source and available under the MIT License.
