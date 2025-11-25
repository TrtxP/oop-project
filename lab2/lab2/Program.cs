using System.Text;
using ClassLibraryATM.Classes;
using ClassLibraryATM.Enums;

class Program
{
    static void Main()
    {
        Console.InputEncoding = Encoding.UTF8;
        Console.OutputEncoding = Encoding.UTF8;

        Console.WriteLine("--------------------------------------------------------------");
        Console.WriteLine("Лабораторну робота №2\nВиконав:\nстудент: Черепанов І.І.\nгрупа:ЗІПЗ-24-1");
        Console.WriteLine("--------------------------------------------------------------");
        Console.WriteLine("Ласкаво просимо у банкомат!");

        var bank = new Bank();

        var account1 = new Account("3456234556784567", "Черепанов Ілля", 0, "3451", false, 0, new DateTime(2028, 09, 15), "UAH", 100000m, 0, new DateTime(2025, 11, 23), new List<Transaction>(), AccountStatus.Active);
        var account2 = new Account("2345547434526786", "Левченко Крістіна", 0, "4655", false, 0, new DateTime(2028, 10, 12), "UAH", 100000m, 0, new DateTime(2025, 10, 26), new List<Transaction>(), AccountStatus.Active);

        bank.RegisterAccount(account1);
        bank.RegisterAccount(account2);

        var ATM = new AutomatedTellerMachine(bank);

        ATM.Authenticated += (card, success, msg) =>
        {
            Console.WriteLine($"Подія авторизації => {msg} (success = {success})");

            if (!success)
            {
                Console.WriteLine("Авторизація неуспішна! Спробуйте ще раз.\n");
            }
            else
            {
                Console.WriteLine("Успішна авторизація");
            }
        };

        ATM.BalanceChecked += (acc, bal, msg) =>
        {
            Console.WriteLine(msg);
            Console.WriteLine($"Баланс: {bal}");
        };

        ATM.WithdrawCompleted += (acc, amt, ok, msg) =>
        {
            Console.WriteLine(msg);
            if (ok) Console.WriteLine($"Знято: {amt}");
        };

        ATM.DepositCompleted += (acc, amt, ok, msg) =>
        {
            Console.WriteLine(msg);
            if (ok) Console.WriteLine($"Поповнено: {amt}");
        };

        ATM.TransferCompleted += (from, to, amt, ok, msg) =>
        {
            Console.WriteLine(msg);
            if (ok) Console.WriteLine($"Переказано {amt} на {to?.CardNumber}");
        };

        bool execute = true;

        while (execute)
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
                Console.WriteLine($"Кількість грошей у наявності: {ATM.CashAvailable}");
                Console.WriteLine($"Стан: {ATM.State}");
                Console.WriteLine("--------------------------------------------------------------");

                execute = false;
                break;
            }

            if (string.IsNullOrWhiteSpace(card) || string.IsNullOrWhiteSpace(pin))
            {
                Console.WriteLine("Номер картки та PIN не можуть бути порожніми. Спробуйте ще раз!\n");
                continue;
            }

            bool success = ATM.Authenticate(card, pin);

            if (!success)
            {
                Console.WriteLine("Спробуйте ще раз!\n");
                continue;
            }

            bool exitMenu = false;
            while (!exitMenu)
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

                switch (choice)
                {
                    case 1:
                        ATM.CheckBalance();
                        break;

                    case 2:
                        Console.Write("Сума для зняття: ");
                        decimal w = decimal.Parse(Console.ReadLine()!);
                        ATM.Withdraw(w);
                        break;

                    case 3:
                        Console.Write("Сума для поповнення: ");
                        decimal d = decimal.Parse(Console.ReadLine()!);
                        ATM.Deposit(d);
                        break;

                    case 4:
                        Console.Write("Картка отримувача: ");
                        string dest = Console.ReadLine()!;
                        Console.Write("Сума переказу: ");
                        decimal t = decimal.Parse(Console.ReadLine()!);
                        ATM.Transfer(dest, t);
                        break;

                    case 5:
                        exitMenu = true;
                        ATM.Logout();
                        break;

                    default:
                        Console.WriteLine("Виберіть пункт від 1 до 5");
                        break;
                }
            }
        }
    }
}