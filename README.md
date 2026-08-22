# ATM Simulation System (Lab 2)

A robust, modular console-based Automated Teller Machine (ATM) simulation built with **C#** and **.NET 8**. The project showcases clean architecture, Object-Oriented Programming (OOP) best practices, design patterns (Builder, Observer, Dependency Injection), and automated testing with xUnit and Moq.

---

## Table of Contents

- [Overview](#overview)
- [Key Features](#key-features)
- [Project Architecture & Structure](#project-architecture--structure)
- [Programming Principles & Design Patterns](#programming-principles--design-patterns)
- [Technologies & Dependencies](#technologies--dependencies)
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Building the Project](#building-the-project)
  - [Running the Console Application](#running-the-console-application)
  - [Running Unit Tests](#running-unit-tests)
- [Project Documentation](#project-documentation)

---

## Overview

This project simulates real-world ATM operations in a multi-account banking environment. The system is designed with a strong separation of concerns:
- **Domain & Business Logic** are isolated inside `ClassLibraryATM` and depend entirely on abstractions.
- **Dependency Injection (DI)** manages lifecycle and wiring via `Microsoft.Extensions.DependencyInjection`.
- **UI & Presentation** is driven by an interactive command-line interface in `lab2`.
- **Quality Assurance** is ensured by a comprehensive test suite in `ATMTests`.

---

## Key Features

- 🔐 **Authentication & Security**:
  - Card number and 4-digit PIN verification.
  - Account blocking mechanism after 3 consecutive failed attempts.
- 💵 **Financial Operations**:
  - **Balance Inquiry**: Check current funds available.
  - **Cash Withdrawal**: ATM cash capacity checks, account balance checks, withdrawal limits, and fee calculation.
  - **Cash Deposit**: Account crediting and ATM cash replenishment.
  - **Fund Transfer**: Card-to-card money transfer between bank accounts.
- 📜 **Transaction Journaling**:
  - Per-account transaction history tracking (with timestamps, transaction types, amounts, fees, and recipient info).
  - Global ATM journal recording all operations performed at the machine.
- 🔔 **Event-Driven Architecture**:
  - Real-time event notifications for operations (authenticated, balance checked, withdrawal, deposit, transfer) delivered through delegates and events.
- 📊 **Bank & ATM Summary Reports**:
  - Overview of all customer accounts, balances, and ATM vault state at session termination.

---

## Project Architecture & Structure

The solution contains three distinct projects:

```
lab2/
├── ClassLibraryATM/              # Core business logic & domain library
│   ├── Builders/                 # Fluent builders for object construction
│   │   ├── AccountBuilder.cs
│   │   └── AtmBuilder.cs
│   ├── Classes/                  # Core domain models and entities
│   │   ├── Account.cs
│   │   ├── AtmSettings.cs
│   │   ├── AutomatedTellerMachine.cs
│   │   ├── Bank.cs
│   │   └── Transaction.cs
│   ├── Delegates/                # Custom delegate definitions for events
│   │   └── AtmEventHandler.cs
│   ├── Enums/                    # State and status enumerations
│   │   ├── AccountStatus.cs
│   │   ├── AtmState.cs
│   │   └── TransactionType.cs
│   ├── Events/                   # Event publisher implementation
│   │   └── AtmEventPublisher.cs
│   ├── Interfaces/               # Abstractions for classes, services, repos, validators
│   │   ├── IAccount.cs
│   │   ├── IAccountRepository.cs
│   │   ├── IAmountValidator.cs
│   │   ├── IAtm.cs
│   │   ├── IAtmEventPublisher.cs
│   │   ├── IAuthenticationService.cs
│   │   ├── IBank.cs
│   │   ├── IBankRepository.cs
│   │   ├── ICardValidator.cs
│   │   ├── IDepositService.cs
│   │   ├── IPinValidator.cs
│   │   ├── ITransactionService.cs
│   │   ├── ITransferService.cs
│   │   └── IWithdrawService.cs
│   ├── Repositories/             # Data access and collection storage
│   │   ├── AccountRepository.cs
│   │   └── BankRepository.cs
│   ├── Services/                 # Business logic operations
│   │   ├── AuthenticationService.cs
│   │   ├── DepositService.cs
│   │   ├── TransactionService.cs
│   │   ├── TransferService.cs
│   │   └── WithdrawService.cs
│   └── Validators/               # Input and domain validation rules
│       ├── AmountValidator.cs
│       ├── CardValidator.cs
│       └── PinValidator.cs
├── lab2/                         # Console presentation layer & DI Composition Root
│   └── Program.cs                # Entry point, DI container setup, interactive CLI
├── ATMTests/                     # Automated unit test suite
│   ├── AtmWorkflowTests.cs       # End-to-end ATM scenario workflows
│   ├── RepositoriesTests/        # Account and Bank repository unit tests
│   ├── ServicesTests/            # Business service logic unit tests
│   └── ValidatorsTests/          # Validator unit tests
├── PROGRAMMING_PRINCIPLES.md     # Detailed documentation of programming principles
├── README.md                     # Main project documentation
└── lab2.sln                      # Visual Studio / .NET solution file
```

---

## Programming Principles & Design Patterns

The architecture strictly adheres to modern software engineering principles and design patterns. For an in-depth breakdown and code references, refer to the dedicated [PROGRAMMING_PRINCIPLES.md](./PROGRAMMING_PRINCIPLES.md) document.

| Principle / Pattern | Description & Application | Reference in Project |
| :--- | :--- | :--- |
| **[Single Responsibility Principle (SRP)](./PROGRAMMING_PRINCIPLES.md#1-single-responsibility-principles-srp)** | Every class, service, repository, and validator handles exactly one concern. `AutomatedTellerMachine` orchestrates actions without managing storage or business validation directly. | [See SRP Section](./PROGRAMMING_PRINCIPLES.md#1-single-responsibility-principles-srp) |
| **[Encapsulation & Data Hiding](./PROGRAMMING_PRINCIPLES.md#2-encapsulation)** | Internal state fields are private and mutated only through dedicated methods (`Deposit`, `Withdraw`) enforcing domain invariants. `AutomatedTellerMachine` protects `CurrentAccount` and `State` with private setters. | [See Encapsulation Section](./PROGRAMMING_PRINCIPLES.md#2-encapsulation) |
| **[Dependency Inversion (DIP) & DI](./PROGRAMMING_PRINCIPLES.md#3-dependency-inversion--dependency-injection)** | High-level modules do not depend on low-level modules; both depend on abstractions (`IAtm`, `IBank`, `IWithdrawService`, etc.). Dependencies are configured in a central composition root (`ConfigureServices`). | [See DIP & DI Section](./PROGRAMMING_PRINCIPLES.md#3-dependency-inversion--dependency-injection) |
| **[Builder Pattern](./PROGRAMMING_PRINCIPLES.md#4-builder-pattern-for-object-construction)** | Simplifies construction of complex objects (`AccountBuilder`, `AtmBuilder`) with fluent configuration and pre-creation validation, separating value construction from dependency injection. | [See Builder Section](./PROGRAMMING_PRINCIPLES.md#4-builder-pattern-for-object-construction) |
| **[Observer Pattern](./PROGRAMMING_PRINCIPLES.md#5-observer-pattern-via-delegates-and-events)** | Uses C# delegates (`AtmEventHandler`) and events (`AtmEventPublisher`) to decouple operation outcomes from UI presentation. | [See Observer Section](./PROGRAMMING_PRINCIPLES.md#5-observer-pattern-via-delegates-and-events) |
| **[KISS (Keep It Simple, Stupid)](./PROGRAMMING_PRINCIPLES.md#6-kiss-keep-it-simple-stupid)** | Logic and data structures are kept clear, readable, and straightforward without unnecessary layers or overengineering. | [See KISS Section](./PROGRAMMING_PRINCIPLES.md#6-kiss-keep-it-simple-stupid) |
| **[Core OOP Principles](./PROGRAMMING_PRINCIPLES.md#7-basic-principles-and-elements-of-oop)** | Systematic use of classes, interfaces, properties, encapsulation, and polymorphism throughout all layers. | [See OOP Section](./PROGRAMMING_PRINCIPLES.md#7-basic-principles-and-elements-of-oop) |
| **[Input Validation & Defensive Programming](./PROGRAMMING_PRINCIPLES.md#8-input-validation)** | Multi-tiered validation: UI layer checks (`TryParse`, format checks) and domain validator services (`CardValidator`, `PinValidator`, `AmountValidator`). | [See Validation Section](./PROGRAMMING_PRINCIPLES.md#8-input-validation) |

---

## Technologies & Dependencies

- **Platform**: [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Language**: C# 12
- **Inversion of Control**: `Microsoft.Extensions.DependencyInjection` (v8.0.0)
- **Testing Framework**: [xUnit](https://xunit.net/) (v2.6.3)
- **Mocking Library**: [Moq](https://github.com/moq/moq4) (v4.20.69)
- **Test Runner**: `Microsoft.NET.Test.Sdk` & `xunit.runner.visualstudio`

---

## Getting Started

### Prerequisites

Ensure you have the [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) installed on your system.

To check your installed version:
```bash
dotnet --version
```

### Building the Project

Clone the repository and build the entire solution:
```bash
dotnet build
```

### Running the Console Application

Run the interactive ATM console interface:
```bash
dotnet run --project lab2/lab2.csproj
```

#### Default Test Accounts

When starting the application, the following test accounts are pre-configured:

| Card Number | Owner | PIN | Initial Balance |
| :--- | :--- | :--- | :--- |
| `3456234556784567` | Черепанов Ілля | `3451` | 15,000 UAH |
| `2345547434526786` | Левченко Крістіна | `4655` | 8,000 UAH |

### Running Unit Tests

Execute all automated unit tests across the test project:
```bash
dotnet test
```

Sample output:
```text
Passed!  - Failed: 0, Passed: 58, Skipped: 0, Total: 58, Duration: ~110 ms - ATMTests.dll
```

---

## Project Documentation

For a comprehensive explanation of architectural decisions, OOP principles, and clean code practices implemented in this project, please consult:

- 📖 **[PROGRAMMING_PRINCIPLES.md](./PROGRAMMING_PRINCIPLES.md)** — In-depth analysis of programming principles (SRP, Encapsulation, DIP/DI, Builder, Observer, KISS, OOP, and Validation).
