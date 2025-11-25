using ClassLibraryATM.Enums;

namespace ClassLibraryATM.Classes
{
    public class AutomatedTellerMachine
    {
        public string? AtmId { get; private set; }
        public string? Address { get; private set; }
        public decimal CashAvailable { get; set; }
        private bool IsOnline { get; set; }
        public Bank OwnerBank { get; private set; }
        private DateTime LastServiceDate { get; set; }
        private decimal MaxWithdrawPerOperation { get; set; }
        private decimal FeePercent { get; set; }
        public List<Transaction> AtmJournal { get; private set; }

        public Account? CurrentAccount { get; private set; }

        public AtmState State;

        public AutomatedTellerMachine() {
            AtmId = "0";
            Address = "Вулиця Героїв Чорнобилю";
            CashAvailable = 0;
            IsOnline = true;
            OwnerBank = new Bank();
            LastServiceDate = new DateTime(2025, 09, 18, 22, 32, 24);
            MaxWithdrawPerOperation = 30000m;
            FeePercent = 0;
            AtmJournal = new List<Transaction>();
            CurrentAccount = new Account();
            State = AtmState.Authenticated;
        }

        public AutomatedTellerMachine(string atmId, string address, decimal cashAvailable, bool isOnline, Bank ownerBank) 
        {
            AtmId = atmId;
            Address = address;
            CashAvailable = cashAvailable;
            IsOnline = isOnline;
            OwnerBank = ownerBank;
            LastServiceDate = new DateTime(2025, 09, 20, 18, 12, 34);
            MaxWithdrawPerOperation = 30000m;
            FeePercent = 0;
            AtmJournal = new List<Transaction>();
            CurrentAccount = new Account();
            State = AtmState.Authenticated;
        }

        public AutomatedTellerMachine(Bank ownerBank)
        {
            OwnerBank = ownerBank;
            IsOnline = true;
            MaxWithdrawPerOperation = 30000m;
            AtmJournal = new List<Transaction>();
            CurrentAccount = new Account();
            State = AtmState.Authenticated;
            LastServiceDate = DateTime.Now;
        }

        public AutomatedTellerMachine(string atmId, string address, decimal cashAvailable, bool isOnline, Bank ownerBank, DateTime lastServiceDate, decimal maxWithdrawPerOperation, decimal feePercent, List<Transaction> atmJournal, Account currentAccount, AtmState state)
        {
            AtmId = atmId;
            Address = address;
            CashAvailable = cashAvailable;
            IsOnline = isOnline;
            OwnerBank = ownerBank;
            LastServiceDate = lastServiceDate;
            MaxWithdrawPerOperation = maxWithdrawPerOperation;
            FeePercent = feePercent;
            AtmJournal = atmJournal;
            CurrentAccount = currentAccount;
            State = state;
        }

        public AutomatedTellerMachine(AutomatedTellerMachine other)
        {
            AtmId = other.AtmId;
            Address = other.Address;
            CashAvailable = other.CashAvailable;
            IsOnline = other.IsOnline;
            OwnerBank = other.OwnerBank != null ? new Bank(other.OwnerBank) : new Bank();
            LastServiceDate = other.LastServiceDate;
            MaxWithdrawPerOperation = other.MaxWithdrawPerOperation;
            FeePercent = other.FeePercent;
            AtmJournal = other.AtmJournal != null ? new List<Transaction>(other.AtmJournal) : new List<Transaction>();
            CurrentAccount = other.CurrentAccount != null ? new Account(other.CurrentAccount) : new Account();
            State = other.State;
        }

        public void Logout()
        {
            CurrentAccount = null;
            State = AtmState.CardInserted;
        }

        public delegate void AuthEventHandler(string cardNumber, bool success, string message);
        public event AuthEventHandler? Authenticated;

        public delegate void BalanceEventHandler(Account? acc, decimal balance, string msg);
        public event BalanceEventHandler? BalanceChecked;

        public delegate void WithdrawEventHandler(Account? acc, decimal amount, bool success, string msg);
        public event WithdrawEventHandler? WithdrawCompleted;

        public delegate void DepositEventHandler(Account? acc, decimal amount, bool success, string msg);
        public event DepositEventHandler? DepositCompleted;

        public delegate void TransferEventHandler(Account? from, Account? to, decimal amount, bool success, string msg);
        public event TransferEventHandler? TransferCompleted;

        public bool Authenticate(string cardNumber, string pin)
        {
            if (!IsOnline)
            {
                State = AtmState.OutOfService;
                Authenticated?.Invoke(cardNumber, false, "Банкомат не активний.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(cardNumber) || string.IsNullOrWhiteSpace(pin))
            {
                State = AtmState.CardInserted;
                Authenticated?.Invoke(cardNumber ?? "NULL", false, "Некоректні дані для авторизації.");
                return false;
            }

            var account = OwnerBank.FindAccount(cardNumber);

            if (account == null)
            {
                Authenticated?.Invoke(cardNumber, false, "Картку не знайдено.");
                return false;
            }

            bool ok = account.VerifyPin(pin);

            if (ok)
            {
                CurrentAccount = account;
                State = AtmState.Authenticated;
                Authenticated?.Invoke(cardNumber, true, "PIN код успішно підтверджено.");
                return true;
            }
            else if (!ok)
            {
                State = AtmState.CardInserted;
                Authenticated?.Invoke(cardNumber, false, "Некоректний PIN код.");
                return false;
            }
            else
            {
                State = AtmState.Locked;
                Authenticated?.Invoke(cardNumber, false, "Ви більше не можете вводити операції вводу для авторизації.");
                return false;
            }
        }

        public void CheckBalance()
        {
            if (CurrentAccount == null)
            {
                BalanceChecked?.Invoke(null, 0, "Немає активного рахунку.");
                return;
            }

            if (State != AtmState.Authenticated)
            {
                BalanceChecked?.Invoke(CurrentAccount, 0, "Користувач не авторизований.");
                return;
            }

            if (IsMaintenanceExpired())
            {
                State = AtmState.OutOfService;
                BalanceChecked?.Invoke(CurrentAccount, CurrentAccount.Balance, "Банкомат потребує обслуговування.");
                return;
            }

            BalanceChecked?.Invoke(CurrentAccount, CurrentAccount.Balance, "Баланс успішно отримано.");
        }

        public void Withdraw(decimal amount)
        {
            if (CurrentAccount == null)
            {
                WithdrawCompleted?.Invoke(null!, amount, false, "Немає активного рахунку.");
                return;
            }

            if (!IsOnline)
            {
                State = AtmState.OutOfService;
                WithdrawCompleted?.Invoke(CurrentAccount, amount, false, "Банкомат не активний.");
                return;
            }

            if (State != AtmState.Authenticated)
            {
                WithdrawCompleted?.Invoke(CurrentAccount, amount, false, "Користувач не авторизований.");
                return;
            }

            if (CurrentAccount.Status != AccountStatus.Active)
            {
                State = AtmState.CardInserted;
                WithdrawCompleted?.Invoke(CurrentAccount, amount, false, "Картка не активна.");
                return;
            }

            if (amount <= 0)
            {
                State = AtmState.Authenticated;
                WithdrawCompleted?.Invoke(CurrentAccount, amount, false, "Некоректна сума.");
                return;
            }

            if (CashAvailable < amount)
            {
                State = AtmState.Authenticated;
                WithdrawCompleted?.Invoke(CurrentAccount, amount, false, "У банкоматі недостатньо коштів.");
                return;
            }

            if (CurrentAccount.Balance < amount)
            {
                State = AtmState.Authenticated;
                WithdrawCompleted?.Invoke(CurrentAccount, amount, false, "На рахунку недостатньо коштів.");
                return;
            }

            if (amount > CurrentAccount.DailyWithdrawLimit)
            {
                State = AtmState.Authenticated;
                WithdrawCompleted?.Invoke(CurrentAccount, amount, false, "Сума перевищує денний ліміт.");
                return;
            }

            if (amount > MaxWithdrawPerOperation)
            {
                State = AtmState.Authenticated;
                WithdrawCompleted?.Invoke(CurrentAccount, amount, false, "Сума перевищує ліміт на одну операцію.");
                return;
            }

            decimal fee = amount * FeePercent / 100;
            decimal total = amount + fee;

            if (CurrentAccount.Balance < total)
            {
                State = AtmState.Authenticated;
                WithdrawCompleted?.Invoke(CurrentAccount, amount, false, "Недостатньо коштів з урахуванням комісії.");
                return;
            }

            if (IsMaintenanceExpired())
            {
                State = AtmState.OutOfService;
                WithdrawCompleted?.Invoke(CurrentAccount, amount, false, "Банкомат потребує обслуговування.");
                return;
            }

            var transaction = new Transaction(
                TransactionType.Widthdraw,
                amount,
                CurrentAccount.CardNumber ?? string.Empty,
                "",
                DateTime.Now,
                fee
                );

            CurrentAccount.Debit(total);
            CashAvailable -= amount;

            AtmJournal.Add(transaction);
            CurrentAccount.History.Add(transaction);

            WithdrawCompleted?.Invoke(CurrentAccount, amount, true, "Зняття готівки виконано.");
        }

        public void Deposit(decimal amount)
        {
            if (CurrentAccount == null)
            {
                DepositCompleted?.Invoke(null!, amount, false, "Немає активного рахунку.");
                return;
            }

            if (!IsOnline)
            {
                State = AtmState.OutOfService;
                DepositCompleted?.Invoke(CurrentAccount, amount, false, "Банкомат не активний.");
                return;
            }

            if (State != AtmState.Authenticated)
            {
                DepositCompleted?.Invoke(CurrentAccount, amount, false, "Користувач не авторизований.");
                return;
            }

            if (CurrentAccount.Status != AccountStatus.Active)
            {
                State = AtmState.Authenticated;
                DepositCompleted?.Invoke(CurrentAccount, amount, false, "Картка не активна.");
                return;
            }

            if (amount <= 0)
            {
                State = AtmState.Authenticated;
                DepositCompleted?.Invoke(CurrentAccount, amount, false, "Некоректна сума.");
                return;
            }

            if (IsMaintenanceExpired())
            {
                State = AtmState.OutOfService;
                DepositCompleted?.Invoke(CurrentAccount, amount, false, "Банкомат потребує обслуговування.");
                return;
            }

            CurrentAccount.Credit(amount);
            CashAvailable += amount;

            DepositCompleted?.Invoke(CurrentAccount, amount, true, "Зарахування готівки відбулося успішно.");
        }

        public void Transfer(string toCardNumber, decimal amount)
        {
            if (CurrentAccount == null)
            {
                TransferCompleted?.Invoke(null, null, amount, false, "Немає активного рахунку.");
                return;
            }

            if (!IsOnline)
            {
                State = AtmState.OutOfService;
                TransferCompleted?.Invoke(CurrentAccount, CurrentAccount, amount, false, "Банкомат не активний.");
                return;
            }

            if (CurrentAccount.Status != AccountStatus.Active)
            {
                State = AtmState.Authenticated;
                TransferCompleted?.Invoke(CurrentAccount, CurrentAccount, amount, false, "Картка не активна.");
                return;
            }

            if (amount <= 0)
            {
                State = AtmState.Authenticated;
                TransferCompleted?.Invoke(CurrentAccount, CurrentAccount, amount, false, "Некоректна сума.");
                return;
            }

            var toAccount = OwnerBank.FindAccount(toCardNumber);
            if (toAccount == null)
            {
                State = AtmState.Authenticated;
                TransferCompleted?.Invoke(CurrentAccount, null, amount, false, "Рахунок одержувача не знайдено.");
                return;
            }

            if (CurrentAccount.Balance < amount)
            {
                State = AtmState.Authenticated;
                TransferCompleted?.Invoke(CurrentAccount, toAccount, amount, false, "На рахунку недостатньо коштів.");
                return;
            }

            if (CurrentAccount.WithdrawnToday + amount > CurrentAccount.DailyWithdrawLimit)
            {
                State = AtmState.Authenticated;
                TransferCompleted?.Invoke(CurrentAccount, toAccount, amount, false, "Сума перевищує денний ліміт.");
                return;
            }

            decimal fee = amount * FeePercent / 100;
            decimal total = amount + fee;

            if (CurrentAccount.Balance < total)
            {
                State = AtmState.Authenticated;
                TransferCompleted?.Invoke(CurrentAccount, toAccount, amount, false, "Недостатньо коштів для переказу коштів з урахуванням комісії.");
                return;
            }

            if (IsMaintenanceExpired())
            {
                State = AtmState.OutOfService;
                TransferCompleted?.Invoke(CurrentAccount, toAccount, amount, false, "Банкомат потребує обслуговування.");
                return;
            }

            var trasnsaction = new Transaction(
                TransactionType.Transfer,
                amount,
                CurrentAccount.CardNumber ?? string.Empty,
                toAccount.CardNumber ?? string.Empty,
                DateTime.Now,
                fee
                );

            CurrentAccount.Debit(total);
            toAccount.Credit(amount);

            AtmJournal.Add(trasnsaction);
            CurrentAccount.History.Add(trasnsaction);
            toAccount.History.Add(trasnsaction);

            TransferCompleted?.Invoke(CurrentAccount, toAccount, amount, true, "Перехаування виконано.");
        }

        private bool IsMaintenanceExpired()
        {
            return (DateTime.Now - LastServiceDate).TotalDays > 180;
        }
    }
}
