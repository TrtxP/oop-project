# Programming principles in lab2 project

## Scope 

Analyze of programming principles is only done for project, that his structure is described in folders **ClassLibraryATM** and **lab2**.

## Introduction

This project implements the general system of ATM.
His main purpose is decribe a clear coding and OOP, built around a clean architecture with Dependency Injection: business rules (**Classes**) depend only on abstractions (**Interfaces**), concrete behaviour is provided by **Services**, **Repositories** and **Validators**, object creation is separated into **Builders**, and cross-cutting notifications go through **Events**/**Delegates**. The composition of all these parts happens in one place — `ConfigureServices` in `Program.cs`.

---

## 1. Single Responsibility Principles (SRP)

Every class/interface is responsible for exactly one concern:

- **ClassLibraryATM\Classes**:
  - Account.cs - an ATM account: identity data and its own balance logic (Deposit/Withdraw).
  - Bank.cs - keeps and looks up the collection of registered accounts.
  - AutomatedTellerMachine.cs - orchestrates a single ATM operation (authenticate, withdraw, deposit, transfer) using the injected services below; it does not implement validation or persistence itself.
  - AtmSettings.cs - plain configuration values for an ATM instance (id, address, cash available, limits) — no behaviour.
  - Transaction.cs - a record of a completed operation for history/journal display.

- **ClassLibraryATM\Interfaces**:
  - IAtm, IBank, IAccount - the abstractions that `Classes` implement, so higher-level code depends on behaviour, not on concrete types.
  - IAuthenticationService, IWithdrawService, IDepositService, ITransferService, ITransactionService - one interface per business operation.
  - IAccountRepository, IBankRepository - abstraction over how accounts/banks are stored and retrieved.
  - ICardValidator, IPinValidator, IAmountValidator - one validator per kind of input.
  - IAtmEventPublisher - abstraction for broadcasting ATM operation events.

- **ClassLibraryATM\Services**: each class (AuthenticationService, WithdrawService, DepositService, TransferService, TransactionService) implements exactly one of the interfaces above and contains only the business rule for that operation.

- **ClassLibraryATM\Repositories**: AccountRepository.cs, BankRepository.cs - responsible only for storing/retrieving data, no business rules.

- **ClassLibraryATM\Validators**: CardValidator.cs, PinValidator.cs, AmountValidator.cs - responsible only for checking that a single piece of input is well-formed.

- **ClassLibraryATM\Builders**: AccountBuilder.cs, AtmBuilder.cs - responsible only for assembling a valid set of construction values (data) step by step; they know nothing about services or DI.

- **ClassLibraryATM\Events** / **ClassLibraryATM\Delegates**: AtmEventPublisher.cs and AtmEventHandler.cs - responsible only for raising and delivering ATM operation events to subscribers.

- **ClassLibraryATM\Enums**:
  - AccountStatus.cs - for checking an account status.
  - AtmState.cs - for checking the state of ATM.
  - TransactionType.cs - for checking the chosen transaction type.

- **lab2**:
  - Program.cs - entry point: wires up the DI container (`ConfigureServices`) and drives the console UI; it does not contain business logic itself.

Examples:

Account:
[Account.cs](./ClassLibraryATM/Classes/Account.cs)

Bank:
[Bank.cs](./ClassLibraryATM/Classes/Bank.cs)

AutomatedTellerMachine:
[AutomatedTellerMachine.cs](./ClassLibraryATM/Classes/AutomatedTellerMachine.cs)

WithdrawService:
[WithdrawService.cs](./ClassLibraryATM/Services/WithdrawService.cs)

AccountRepository:
[AccountRepository.cs](./ClassLibraryATM/Repositories/AccountRepository.cs)

AtmBuilder:
[AtmBuilder.cs](./ClassLibraryATM/Builders/AtmBuilder.cs)

AtmEventPublisher:
[AtmEventPublisher.cs](./ClassLibraryATM/Events/AtmEventPublisher.cs)

Transaction:
[Transaction.cs](./ClassLibraryATM/Classes/Transaction.cs)

Account status:
[AccountStatus.cs](./ClassLibraryATM/Enums/AccountStatus.cs)

ATM state:
[AtmState.cs](./ClassLibraryATM/Enums/AtmState.cs)

Transaction type:
[TransactionType.cs](./ClassLibraryATM/Enums/TransactionType.cs)

This demonstrates a clear separation of responsibilities across the architecture, not only inside `Classes`.

---

## 2. Encapsulation

Fields that represent internal state are hidden behind **private** access and only change through methods that enforce invariants.

For example, in Account.cs the balance is changed only inside **Deposit**/**Withdraw**, never assigned directly from outside:

[Account.cs](./ClassLibraryATM/Classes/Account.cs#L110-L140)

The same idea applies at the ATM level: `AutomatedTellerMachine` exposes `CurrentAccount` and `State` as **private set** properties, so external code (`Program.cs`) can read the ATM state but cannot mutate it directly — only the ATM's own operation methods (`Authenticate`, `Withdraw`, `Deposit`, `Transfer`, `Logout`) are allowed to change it:

[AutomatedTellerMachine.cs](./ClassLibraryATM/Classes/AutomatedTellerMachine.cs)

It avoids the uncontrollable changing of object state.

---

## 3. Dependency Inversion & Dependency Injection

`AutomatedTellerMachine` does not create its collaborators with `new`; it depends only on abstractions (`IAuthenticationService`, `IWithdrawService`, `IDepositService`, `ITransferService`, `ITransactionService`, `IAtmEventPublisher`), which are supplied through its constructor:

[AutomatedTellerMachine.cs](./ClassLibraryATM/Classes/AutomatedTellerMachine.cs)

The concrete implementations are registered once, in a single composition root, and resolved from there:

[Program.cs](./lab2/Program.cs#L59-L91)

This means both the high-level module (`AutomatedTellerMachine`) and the low-level modules (`AuthenticationService`, `WithdrawService`, ...) depend on the same interfaces instead of on each other, which keeps implementations swappable (e.g. for unit tests) without touching the classes that use them.

---

## 4. Builder pattern for object construction

Objects with several optional or ordered construction values are assembled through a dedicated builder instead of long constructor calls, and each builder validates its inputs before creating the object:

Account creation:
[AccountBuilder.cs](./ClassLibraryATM/Builders/AccountBuilder.cs)

ATM configuration:
[AtmBuilder.cs](./ClassLibraryATM/Builders/AtmBuilder.cs)

`AtmBuilder` only produces the plain data object `AtmSettings` — it does not know about services or the DI container. The full `AutomatedTellerMachine` is assembled from `AtmSettings` (data) and the injected services (behaviour) together, in the factory registered in `ConfigureServices`:

[Program.cs](./lab2/Program.cs#L38-L43)

This keeps construction of *values* (Builder) and resolution of *dependencies* (DI container) as two separate concerns.

---

## 5. Observer pattern via delegates and events

ATM operations do not report their outcome through return values alone — they publish events that any number of independent subscribers can react to, without the ATM knowing who is listening:

[AtmEventHandler.cs](./ClassLibraryATM/Delegates/AtmEventHandler.cs)
[AtmEventPublisher.cs](./ClassLibraryATM/Events/AtmEventPublisher.cs)

`Program.cs` subscribes to these events once, after resolving the same `IAtmEventPublisher` instance that the ATM itself uses:

[Program.cs](./lab2/Program.cs#L92-L96)

---

## 6. KISS (Keep It Simple, Stupid)

Methods are implemented without unnecessary complexity.
Operations of registering and finding an account execute via simple conditions.

For example:
[Bank.cs](./ClassLibraryATM/Classes/Bank.cs#L49-L64)

Logic is rectilinear and understandable, even though it now sits behind an interface (`IBank`).

---

## 7. Basic principles and elements of OOP

Project uses an OOP approach, including:

- Classes and Interfaces
- Methods
- Fields and Properties
- Encapsulation
- Dependency Inversion (Interfaces + DI container)

For example, in [Program.cs](./lab2/Program.cs#L28-L43) instances of `Bank`, `Account` (via `AccountBuilder`) and `AutomatedTellerMachine` (via `AtmBuilder` + the DI-resolved factory) are created and made to interact through their interfaces (`IBank`, `IAccount`, `IAtm`).

It's completely according to OOP principles.

---

## 8. Input validation

Card data, PIN and chosen amounts are checked before being used in any operation, both at the UI boundary and inside the corresponding validator/service.

[Program.cs](./lab2/Program.cs)

For checking input validation of selected menu choices and amounts, see the [**TryParse**](./lab2/Program.cs#L197) calls in `RunAtmInterface`.

Domain-level validation (card format, PIN format, amount limits) is delegated to the dedicated validators:
[CardValidator.cs](./ClassLibraryATM/Validators/CardValidator.cs)
[PinValidator.cs](./ClassLibraryATM/Validators/PinValidator.cs)
[AmountValidator.cs](./ClassLibraryATM/Validators/AmountValidator.cs)

---