using ClassLibraryATM.Builders;
using ClassLibraryATM.Classes;
using ClassLibraryATM.Events;
using ClassLibraryATM.Interfaces;
using ClassLibraryATM.Repositories;
using ClassLibraryATM.Services;
using ClassLibraryATM.Validators;
using Microsoft.Extensions.DependencyInjection;
using System.Text;

class Program
{
    static void Main()
    {
        Console.InputEncoding = Encoding.UTF8;
        Console.OutputEncoding = Encoding.UTF8;

        // Налаштування DI контейнера
        var services = new ServiceCollection();
        ConfigureServices(services);
        var serviceProvider = services.BuildServiceProvider();

        Console.WriteLine("--------------------------------------------------------------");
        Console.WriteLine("Лабораторну роботу №2\nВиконав:\nстудент: Черепанов І.І.\nгрупа: ЗІПЗ-24-1");
        Console.WriteLine("--------------------------------------------------------------");
        Console.WriteLine("Ласкаво просимо до банкомату!\n");

        // Отримання банку та репозиторіїв з DI
        var bank = serviceProvider.GetRequiredService<IBank>();

        // Створення тестових акаунтів через AccountBuilder та реєстрація в банку
        var account1 = new AccountBuilder()
            .WithCardNumber("3456234556784567")
            .WithOwnerFullName("Черепанов Ілля")
            .WithPinCode("3451")
            .WithBalance(15000m)
            .Build();

        var account2 = new AccountBuilder()
            .WithCardNumber("2345547434526786")
            .WithOwnerFullName("Левченко Крістіна")
            .WithPinCode("4655")
            .WithBalance(8000m)
            .Build();

        bank.RegisterAccount(account1);
        bank.RegisterAccount(account2);

        // Налаштування та створення банкомату через AtmBuilder та DI-фабрику
        var atmSettings = new AtmBuilder()
            .WithAtmId("ATM-001")
            .WithAddress("вул. Героїв Чорнобиля, 10")
            .WithCashAvailable(50000m)
            .WithMaxWithdrawPerOperation(20000m)
            .WithFeePercent(1.0m)
            .Build();

        var atmFactory = serviceProvider.GetRequiredService<Func<AtmSettings, IBank, IAtm>>();
        var atm = atmFactory(atmSettings, bank);

        // Підписка на події банкомату
        var atmEventPublisher = serviceProvider.GetRequiredService<IAtmEventPublisher>();
        SubscribeToEvents(atmEventPublisher);

        // Головний цикл взаємодії з користувачем
        RunAtmInterface(atm, bank);

        Console.WriteLine("\n--------------------------------------------------------------");
        Console.WriteLine("Завершення роботи програми. Формування звіту...");
        PrintReport(bank, atm);
        Console.WriteLine("--------------------------------------------------------------");
    }

    static void ConfigureServices(ServiceCollection services)
    {
        // 1. Валідатори (Validators)
        services.AddSingleton<ICardValidator, CardValidator>();
        services.AddSingleton<IPinValidator, PinValidator>();
        services.AddSingleton<IAmountValidator, AmountValidator>();

        // 2. Репозиторії (Repositories)
        services.AddSingleton<IAccountRepository, AccountRepository>();
        services.AddSingleton<IBankRepository, BankRepository>();

        // 3. Сервіси бізнес-логіки (Services)
        services.AddSingleton<IAuthenticationService, AuthenticationService>();
        services.AddSingleton<ITransactionService, TransactionService>();
        services.AddSingleton<IWithdrawService, WithdrawService>();
        services.AddSingleton<IDepositService, DepositService>();
        services.AddSingleton<ITransferService, TransferService>();

        // 4. Події (Events & Publisher)
        services.AddSingleton<IAtmEventPublisher, AtmEventPublisher>();

        // 5. Банк (Bank) на базі IAccountRepository
        services.AddSingleton<IBank>(sp =>
        {
            var accountRepo = sp.GetRequiredService<IAccountRepository>();
            var bank = new Bank("Житомир-Банк", accountRepo);
            var bankRepo = sp.GetRequiredService<IBankRepository>();
            bankRepo.SaveBank(bank);
            return bank;
        });

        // 6. Фабрика для створення ATM із впровадженням усіх залежностей
        services.AddSingleton<Func<AtmSettings, IBank, IAtm>>(sp => (settings, bank) =>
            new AutomatedTellerMachine(
                settings,
                bank,
                sp.GetRequiredService<IAuthenticationService>(),
                sp.GetRequiredService<IWithdrawService>(),
                sp.GetRequiredService<IDepositService>(),
                sp.GetRequiredService<ITransferService>(),
                sp.GetRequiredService<ITransactionService>(),
                sp.GetRequiredService<IAtmEventPublisher>()
            )
        );
    }

    static void SubscribeToEvents(IAtmEventPublisher atmEventPublisher)
    {
        atmEventPublisher.Authenticated += (sender, e) =>
        {
            if (e.Success)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"[Авторизація] {e.Message}");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[Авторизація] {e.Message}");
                Console.ResetColor();
            }
        };

        atmEventPublisher.BalanceChecked += (sender, e) =>
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"[Баланс] {e.Message}");
            Console.WriteLine($"[Баланс] Доступно: {e.Balance} UAH");
            Console.ResetColor();
        };

        atmEventPublisher.WithdrawCompleted += (sender, e) =>
        {
            if (e.Success)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"[Зняття] {e.Message}");
                Console.WriteLine($"[Зняття] Сума: {e.Amount} UAH");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[Зняття] Помилка: {e.Message}");
                Console.ResetColor();
            }
        };

        atmEventPublisher.DepositCompleted += (sender, e) =>
        {
            if (e.Success)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"[Поповнення] {e.Message}");
                Console.WriteLine($"[Поповнення] Сума: {e.Amount} UAH");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[Поповнення] Помилка: {e.Message}");
                Console.ResetColor();
            }
        };

        atmEventPublisher.TransferCompleted += (sender, e) =>
        {
            if (e.Success)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"[Переказ] {e.Message}");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[Переказ] Помилка: {e.Message}");
                Console.ResetColor();
            }
        };
    }

    static void RunAtmInterface(IAtm atm, IBank bank)
    {
        while (true)
        {
            Console.WriteLine("--------------------------------------------------------------");
            Console.WriteLine("Введіть номер картки та PIN (або 'exit' для завершення):");
            Console.Write("Номер картки: ");
            string? card = Console.ReadLine();

            if (card?.Trim().ToLower() == "exit")
                break;

            Console.Write("PIN: ");
            string? pin = Console.ReadLine();

            if (pin?.Trim().ToLower() == "exit")
                break;

            if (string.IsNullOrWhiteSpace(card) || string.IsNullOrWhiteSpace(pin))
            {
                Console.WriteLine("Номер картки та PIN не можуть бути порожніми.\n");
                continue;
            }

            bool success = atm.Authenticate(card.Trim(), pin.Trim());
            if (!success)
            {
                Console.WriteLine("Спробуйте ще раз!\n");
                continue;
            }

            // Меню операцій для авторизованого користувача
            while (atm.State == ClassLibraryATM.Enums.AtmState.Authenticated)
            {
                Console.WriteLine("\n--- Меню операцій ---");
                Console.WriteLine("1. Переглянути баланс");
                Console.WriteLine("2. Зняти готівку");
                Console.WriteLine("3. Поповнити картку");
                Console.WriteLine("4. Переказати кошти на іншу картку");
                Console.WriteLine("5. Історія транзакцій акаунту");
                Console.WriteLine("6. Завершити сеанс (Вийти)");
                Console.Write("Оберіть опцію (1-6): ");

                if (!int.TryParse(Console.ReadLine(), out int choice))
                {
                    Console.WriteLine("Некоректний вибір. Введіть число від 1 до 6.");
                    continue;
                }

                if (choice == 6)
                {
                    atm.Logout();
                    Console.WriteLine("Сеанс завершено. Картку вилучено.\n");
                    break;
                }

                switch (choice)
                {
                    case 1:
                        atm.CheckBalance();
                        break;

                    case 2:
                        Console.Write("Введіть суму для зняття: ");
                        if (decimal.TryParse(Console.ReadLine(), out decimal withdrawAmount))
                        {
                            atm.Withdraw(withdrawAmount);
                        }
                        else
                        {
                            Console.WriteLine("Некоректний формат суми.");
                        }
                        break;

                    case 3:
                        Console.Write("Введіть суму для поповнення: ");
                        if (decimal.TryParse(Console.ReadLine(), out decimal depositAmount))
                        {
                            atm.Deposit(depositAmount);
                        }
                        else
                        {
                            Console.WriteLine("Некоректний формат суми.");
                        }
                        break;

                    case 4:
                        Console.Write("Введіть номер картки отримувача: ");
                        string? destCard = Console.ReadLine();
                        Console.Write("Введіть суму переказу: ");
                        if (decimal.TryParse(Console.ReadLine(), out decimal transferAmount) && !string.IsNullOrWhiteSpace(destCard))
                        {
                            atm.Transfer(destCard.Trim(), transferAmount);
                        }
                        else
                        {
                            Console.WriteLine("Некоректні дані переказу.");
                        }
                        break;

                    case 5:
                        Console.WriteLine("\n=== Історія операцій акаунту ===");
                        if (atm.CurrentAccount?.History.Count == 0)
                        {
                            Console.WriteLine("  Операцій ще не було.");
                        }
                        else
                        {
                            foreach (var tr in atm.CurrentAccount!.History)
                            {
                                Console.WriteLine($"  [{tr.Date:yyyy-MM-dd HH:mm:ss}] {tr.Type}: {tr.Amount} UAH (Комісія: {tr.Fee} UAH) | {tr.FromCard} -> {tr.ToCard}");
                            }
                        }
                        break;

                    default:
                        Console.WriteLine("Оберіть пункт від 1 до 6.");
                        break;
                }
            }
        }
    }

    static void PrintReport(IBank bank, IAtm atm)
    {
        Console.WriteLine("\n================== ЗВІТ ПО БАНКУ ТА БАНКОМАТУ ==================");

        Console.WriteLine($"\n[Банк]: {bank.Name}");
        Console.WriteLine("--- Рахунки клієнтів ---");
        foreach (var acc in bank.Accounts)
        {
            Console.WriteLine($"\nКартка: {acc.Key} | Власник: {acc.Value.OwnerFullName} | Баланс: {acc.Value.Balance} UAH | Статус: {acc.Value.Status}");
            if (acc.Value.History.Count == 0)
            {
                Console.WriteLine("  (Транзакцій немає)");
            }
            else
            {
                foreach (var tr in acc.Value.History)
                {
                    Console.WriteLine($"  [{tr.Date:yyyy-MM-dd HH:mm:ss}] {tr.Type} - {tr.Amount} UAH (Комісія: {tr.Fee} UAH)");
                }
            }
        }

        Console.WriteLine("\n--- Стан банкомату ---");
        Console.WriteLine($"Ідентифікатор: {atm.AtmId}");
        Console.WriteLine($"Адреса: {atm.Address}");
        Console.WriteLine($"Готівка в банкоматі: {atm.CashAvailable} UAH");
        Console.WriteLine($"Стан банкомату: {atm.State}");
        Console.WriteLine($"Загальна кількість транзакцій у журналі ATM: {atm.AtmJournal.Count}");
        Console.WriteLine("================================================================\n");
    }
}