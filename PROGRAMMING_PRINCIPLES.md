# Programming principles in lab2 project

## Scope 

Analyze of programming principles is only done for project, that his structure is described in folders **ClassLibraryATM** and **lab2**.

## Introduction

This project implements the general system of ATM.
His main purpose is decribe a clear coding and OOP.

---

## 1. Single Responsibility Principles (SRP)

Every class from **Classes** folder and enum from **Enums** folder and accordingly class **Program.cs** from folder **lab2** responce only for logical consistence:

- **ClassLibraryATM\Classes**:

- Account.cs - demonstating an ATM account and contains logical work with balance.
- Bank.cs - controlling the lists of the ATM accounts and ATMs.
- AutomatedTellerMachine.cs - responcible for working an ATM.
- Transaction.cs - class for implementing a displaying of transaction in ATM display.

- **ClassLibraryATM\Enums**:

- AccountStatus.cs - for checking an account status.
- AtmState.cs - for checking the state of ATM.
- TransactionType.cs - for checking the chosen transaction type.

- **lab2**:
- Program.cs - entry point of program.

Examples:

Account:
[Account.cs](./lab2/ClassLibraryATM/Classes/Account.cs)

Bank:
[Bank.cs](./lab2/ClassLibraryATM/Classes/Bank.cs)

AutomatedTellerMachine:
[AutomatedTellerMachine.cs](./lab2/ClassLibraryATM/Classes/AutomatedTellerMachine.cs)

Transaction:
[Transaction.cs](./lab2/ClassLibraryATM/Classes/Transaction.cs)

Account status:
[AccountStatus.cs](./lab2/ClassLibraryATM/Enums/AccountStatus.cs)

AMT state:
[AtmState.cs](./lab2/ClassLibraryATM/Enums/AtmState.cs)

Transaction type:
[TransactionType.cs](./lab2/ClassLibraryATM/Enums/TransactionType.cs)

This demonstrate a separation of responsibilities and clearest architecture of code.

---

## 2. Encapsylation 

The some field of classes are hided by **private** statement that it allows have access in methods

For example, in Account.cs, the balance is changed in **Credit** method, not directly:

[Account.cs](./lab2/ClassLibraryATM/Classes/Account.cs#L129-L144)

It avoid the uncontrollable changing object state.

---

## 3. Separation of Concerns

The program logic is separated on separateness:

- Interaction with user - Program.cs
- Bussiness logic - AutomatedTellerMachine.cs
- Saving accounts - Bank.cs

Program.cs:
[Program.cs](./lab2/lab2/Program.cs)

This approach makes the code more understandable and maintainable.

---

## 4. KISS (Keep It Simple, Stupid)

Methods are implemented without unnecessary complexity.
Operations of registering and finding account execute via simple conditions.

For example:
[Bank.cs](./lab2/ClassLibraryATM/Classes/Bank.cs#L49-L61)

Logic is a rectilinear and understandable.

---

## 5. Basic principles and elements of OOP

Project uses an OOP approach, including:

- Classes
- Methods
- Fields
- Encapsylation

For example in [Program.cs](./lab2/lab2/Program.cs#L17-L25) the instances of classes Bank, AutomatedTellerMachine and Account are declared and processing the interaction of ones.

It's completely according to OOP principles.

---

## 6. Input validation 

I check the entered card data before using it.

[Program.cs](./lab2/lab2/Program.cs)

For checking input validation of selecting choises using [**TryParse**](./lab2/lab2/Program.cs#L130-L134) method.

---
