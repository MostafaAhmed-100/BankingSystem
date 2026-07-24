# 🏦 Secure Banking System REST API

A highly optimized, secure, and robust Banking System backend built with **ASP.NET Core 8 Web API**. This system is engineered to handle critical financial operations with absolute data integrity, supporting features like optimistic concurrency, transactional audit trails, clean domain isolation, payment gateway processing, and role-based access control (Banker & Customer).

## 🚀 Tech Stack

| Layer | Technology |
| --- | --- |
| **Framework** | ASP.NET Core 8 Web API |
| **ORM** | Entity Framework Core (Pure Fluent API) |
| **Database** | SQL Server |
| **Auth & Identity** | ASP.NET Core Identity + JWT Bearer + Refresh Token Rotation |
| **Architecture** | Clean Architecture + Repository Pattern + Unit of Work |
| **Object Mapping** | AutoMapper |
| **Input Validation** | Fluent Validation + C# 11 `required` modifiers |
| **Email Service** | SMTP / MailKit (Account Confirmation & Password Reset) |
| **Logging** | Serilog (File & Console Sinks, Transaction Tracking) |
| **Concurrency** | EF Core Optimistic Concurrency (`RowVersion`) |
| **Error Handling** | Custom Exception Middleware + Domain Exceptions |

## 🏗️ Project Structure

```text
├── Constants/                 # AppRoles, TransactionTypes
├── Controllers/               # API endpoints (Thin layer routing requests)
├── Data/                      # Database context, Entities, and DB Configurations
│   ├── Configurations/        # Fluent API configurations for absolute DB schema control
│   ├── models/                # EF Core models (Account, CreditCard, RefreshToken, Transaction, etc.)
│   └── AppDbContext.cs
├── DTOS/                      # Data Transfer Objects organized by domain
│   ├── AccountDTOs/
│   ├── Auth&IdentityDTOs/
│   ├── CreditCards&Loans/
│   ├── PaymentGatewayDTOs/    # ChargeCard, Refund, and CardValidation DTOs
│   ├── Transactions&Transfers/# Transaction, Transfer, and Statement DTOs
│   ├── Validators/            # Fluent Validation rules isolated from DTOs
│   └── Shared/                # Shared wrappers (e.g., ApiResponseDto, PaginationRequestDto)
├── Exceptions/                # Domain-specific custom exceptions
├── Mappings/                  # AutoMapper Profiles (Domain-to-DTO transformation)
├── Middlewares/               # Custom pipeline (Global Exception Handling)
├── Migrations/                # EF Core migration history & schema snapshots
├── Repository/                # Data access layer
│   ├── GenericRepository/     # Base CRUD operations
│   ├── SpecificRepository/    # Domain-specific queries (Account, CreditCard, Transaction)
│   └── UnitOfWork/            # Centralized transaction management (ACID properties)
└── Services/                  # Business Logic Layer
    ├── Auth/                  # Identity, Token Generation & Refresh Logic
    ├── EmailService/          # Account confirmation & Password reset emails
    └── PaymentGateway/        # External payment processing & Idempotent refunds

```

---

## 👥 Roles & Permissions

| Role | Capabilities |
| --- | --- |
| `Banker` | Manages branches, registers new customers/bankers, reviews loan applications, and monitors overall banking activities. |
| `Customer` | Browses owned accounts, manages credit cards, requests loans, performs transfers, and views paginated transaction history. |

---

## 🛡️ Core Banking Features & Security

### 🔐 Advanced Authentication & Refresh Token Flow

* **JWT & Refresh Tokens:** Secure token issuance with sliding-expiration Refresh Tokens stored and validated in the database.
* **Email Verification & Password Reset:** Integrated `EmailService` handles automated confirmation links and password recovery workflows upon user registration.

### 💳 Enterprise Payment Gateway & Idempotency Protection

* **External Charge & Refund Processing:** Acts as a secure payment gateway for third-party platforms (e.g., E-Commerce stores).
* **Idempotency Protection (`ReferenceId`):** Prevents duplicate charges caused by network latency or repeated API calls using strict reference tracking.
* **Double-Refund Prevention:** Tracks original transaction states to ensure a single payment cannot be refunded multiple times.
* **Card Authentication:** Validates CVV, Expiration Date, and Active Status before authorizing charges or limit deductions.

### ⚡ Absolute Data Integrity (Concurrency)

To prevent "Double Spending" or race conditions when multiple transactions occur simultaneously, the `Account` entity is protected using **Optimistic Concurrency** via SQL Server `RowVersion`. If two requests attempt to modify the same balance at the exact same millisecond, the system safely aborts the conflicting request, ensuring no funds are lost or duplicated.

### 📜 Immutable Transaction History (Audit Trail)

Balances are never modified without a trace. Every financial movement (Deposit, Withdrawal, Transfer, Gateway Payment, Refund) is structurally logged as an immutable `Transaction` record tied to the specific account, establishing a strict, bank-grade ledger.

### 📊 Database-Level Pagination

All listing endpoints (Accounts, Credit Cards, Transaction Statements) perform memory-optimized pagination natively on the database server using `Skip()` and `Take()`.

---

## 📁 Unified Response Format

Every endpoint returns the exact same wrapper shape (`ApiResponseDto<T>`), making frontend or third-party integration seamless.

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
    "Audience": "APISecureUser",
    "DurationInMinutes": 60
  },
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "Port": 587,
    "SenderName": "Secure Banking System",
    "SenderEmail": "no-reply@securebank.com",
    "Username": "your_email@gmail.com",
    "Password": "your_app_password"
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

```

```
