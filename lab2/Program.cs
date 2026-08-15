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

        // Настройка DI контейнера
        var services = new ServiceCollection();
        ConfigureServices(services);
        var serviceProvider = services.BuildServiceProvider();

        Console.WriteLine("--------------------------------------------------------------");
        Console.WriteLine("Лабораторну роботу №2\nВиконав:\nстудент: Череланов І.І.\nгрупа: ЗІПЗ-24-1");
        Console.WriteLine("--------------------------------------------------------------");
        Console.WriteLine("Ласкаво просимо у банкоматі!");

        // Инициализация банка и счетов
        var bank = new Bank("ATM №12");

        // Использование AccountBuilder для создания аккаунтов
        var registerAccount1 = new AccountBuilder().WithCardNumber("3456234556784567").WithOwnerFullName("Черепанов Ілля").WithPinCode("3451").Build();
        var registerAccount2 = new AccountBuilder().WithCardNumber("2345547434526786").WithOwnerFullName("Левченко Крістіна").WithPinCode("4655").Build();

        bank.RegisterAccount(registerAccount1);
        bank.RegisterAccount(registerAccount2);

        // Получение ATM из контейнера с инвертированными зависимостями
        var atmEventPublisher = serviceProvider.GetRequiredService<IAtmEventPublisher>();

        var atmSettings = new AtmBuilder().WithAtmId("1").WithAddress("Вулиця Героїв Чорнобиля").WithCashAvailable(0).Build();
        var atmFactory = serviceProvider.GetRequiredService<Func<AtmSettings, IBank, IAtm>>();
        var atm = atmFactory(atmSettings, bank);

        // Подписка на события
        SubscribeToEvents(atmEventPublisher);

        // Главный цикл
        RunAtmInterface(atm, bank);

        Console.WriteLine("\n--------------------------------------------------------------");
        Console.WriteLine("Завершення роботи програм. Формування звіту...");

        PrintReport(bank, atm);

        Console.WriteLine("--------------------------------------------------------------");
    }

    static void ConfigureServices(ServiceCollection services)
    {
        // Validators
        services.AddSingleton<ICardValidator, CardValidator>();
        services.AddSingleton<IPinValidator, PinValidator>();
        services.AddSingleton<IAmountValidator, AmountValidator>();

        // Services
        services.AddSingleton<IAuthenticationService, AuthenticationService>();
        services.AddSingleton<ITransactionService, TransactionService>();
        services.AddSingleton<IWithdrawService, WithdrawService>();
        services.AddSingleton<IDepositService, DepositService>();
        services.AddSingleton<ITransferService, TransferService>();

        // Repositories
        services.AddSingleton<IAccountRepository, AccountRepository>();
        services.AddSingleton<IBankRepository, BankRepository>();

        // Event Publisher
        services.AddSingleton<IAtmEventPublisher, AtmEventPublisher>();

        // Factory для создания ATM с инъекциями
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
            if (!e.Success)
            {
                Console.WriteLine("Авторизація неуспішна! Спробуйте ще раз.\n");
            }
            else
            {
                Console.WriteLine("Успішна авторизація");
            }
        };

        atmEventPublisher.BalanceChecked += (sender, e) =>
        {
            Console.WriteLine(e.Message);
            Console.WriteLine($"Баланс: {e.Balance}");
        };

        atmEventPublisher.WithdrawCompleted += (sender, e) =>
        {
            Console.WriteLine(e.Message);
            if (e.Success) Console.WriteLine($"Знято: {e.Amount}");
        };

        atmEventPublisher.DepositCompleted += (sender, e) =>
        {
            Console.WriteLine(e.Message);
            if (e.Success) Console.WriteLine($"Поповнено: {e.Amount}");
        };

        atmEventPublisher.TransferCompleted += (sender, e) =>
        {
            Console.WriteLine(e.Message);
            if (e.Success) Console.WriteLine($"Переказано {e.Amount} на {e.ToAccount?.CardNumber}");
        };
    }

    static void RunAtmInterface(IAtm atm, Bank bank)
    {
        while (true)
        {
            Console.WriteLine("(Для виходу, введіть 'exit')");
            Console.Write("Номер картки: ");
            string? card = Console.ReadLine();
            Console.Write("PIN: ");
            string? pin = Console.ReadLine();

            if (card?.ToLower() == "exit" || pin?.ToLower() == "exit")
            {
                Console.WriteLine("\n--------------------------------------------------------------");
                Console.WriteLine("Завершення роботи програм. Формування звіту...");

                Console.WriteLine("\n=== Звіт про транзакції ===");

                foreach (var acc in bank.Accounts)
                {
                    Console.WriteLine($"\nКартка: {acc.Key} - {acc.Value.OwnerFullName}");
                    if (acc.Value.History.Count == 0)
                    {
                        Console.WriteLine("  (Транзакцій немає)");
                        continue;
                    }

                    foreach (var tr in acc.Value.History)
                    {
                        Console.WriteLine($"  {tr.Date}: {tr.Type} - {tr.Amount} UAH");
                    }
                }

                Console.WriteLine($"\n=== Стан банкомата ===");
                Console.WriteLine($"Кількість грошей у наявності: {atm.CashAvailable}");
                Console.WriteLine($"Стан: {atm.State}");
                Console.WriteLine("--------------------------------------------------------------");

                break;
            }

            if (string.IsNullOrWhiteSpace(card) || string.IsNullOrWhiteSpace(pin))
            {
                Console.WriteLine("Номер картки та PIN не можуть бути порожніми. Спробуйте ще раз!\n");
                continue;
            }

            bool success = atm.Authenticate(card, pin);

            if (!success)
            {
                Console.WriteLine("Спробуйте ще раз!\n");
                continue;
            }

            while (true)
            {
                Console.WriteLine("\nМеню:");
                Console.WriteLine("1. Баланс");
                Console.WriteLine("2. Зняти кошти");
                Console.WriteLine("3. Поповнити картку");
                Console.WriteLine("4. Переказати кошти");
                Console.WriteLine("5. Вийти");

                if (!int.TryParse(Console.ReadLine(), out int choice))
                {
                    Console.WriteLine("Некоректний вибір");
                    continue;
                }

                if (choice == 5)
                {
                    atm.Logout();
                    break;
                }

                switch (choice)
                {
                    case 1:
                        atm.CheckBalance();
                        break;

                    case 2:
                        Console.Write("Сума для зняття: ");
                        if (decimal.TryParse(Console.ReadLine(), out decimal withdraw))
                        {
                            atm.Withdraw(withdraw);
                        }
                        else
                        {
                            Console.WriteLine("Некоректна сума");
                        }
                        break;

                    case 3:
                        Console.Write("Сума для поповнення: ");
                        if (decimal.TryParse(Console.ReadLine(), out decimal deposit))
                        {
                            atm.Deposit(deposit);
                        }
                        else
                        {
                            Console.WriteLine("Некоректна сума");
                        }
                        break;

                    case 4:
                        Console.Write("Картка отримувача: ");
                        string? destCard = Console.ReadLine();
                        Console.Write("Сума переказу: ");
                        if (decimal.TryParse(Console.ReadLine(), out decimal transfer) && !string.IsNullOrWhiteSpace(destCard))
                        {
                            atm.Transfer(destCard, transfer);
                        }
                        else
                        {
                            Console.WriteLine("Некоректні дані");
                        }
                        break;

                    default:
                        Console.WriteLine("Виберіть пункт від 1 до 5");
                        break;
                }
            }
        }
    }

    static void PrintReport(Bank bank, IAtm atm)
    {
        Console.WriteLine("\n=== Звіт про транзакції ===");

        foreach (var acc in bank.Accounts)
        {
            Console.WriteLine($"\nКартка: {acc.Key} - {acc.Value.OwnerFullName}");
            if (acc.Value.History.Count == 0)
            {
                Console.WriteLine("  (Транзакцій немає)");
                continue;
            }

            foreach (var tr in acc.Value.History)
            {
                Console.WriteLine($"  {tr.Date}: {tr.Type} - {tr.Amount} UAH");
            }
        }

        Console.WriteLine($"\n=== Стан банкомата ===");
        Console.WriteLine($"Кількість грошей у наявності: {atm.CashAvailable}");
        Console.WriteLine($"Стан: {atm.State}");
    }
}