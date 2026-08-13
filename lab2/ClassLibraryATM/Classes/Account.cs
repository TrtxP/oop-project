using ClassLibraryATM.Enums;
using ClassLibraryATM.Interfaces;

namespace ClassLibraryATM.Classes
{
    public class Account : IAccount
    {
        public string? CardNumber { get; private set; }
        public string? OwnerFullName { get; private set; }
        public decimal Balance { get; private set; }
        private string? _pinCode;
        private bool _isBlocked;
        private int _failedPinAttempts;
        private DateTime _expireDate;
        private string? _currency;
        private DateTime _lastWithdrawDate;

        public string? CardNumber { get; private set; }
        public string? OwnerFullName { get; private set; }
        public decimal Balance { get; private set; }
        public AccountStatus Status { get; private set; }
        public decimal DailyWithdrawLimit { get; private set; }
        public decimal WithdrawnToday { get; private set; }
        private DateTime _lastWithdrawDate;
        public List<Transaction> History { get; private set; }

        public AccountStatus Status { get; private set; }

        public Account()
        {
            CardNumber = "0000 0000 0000 0000";
            OwnerFullName = "Прізвище Ім'я";
            Balance = 0;
            _pinCode = "0000";
            _isBlocked = false;
            _failedPinAttempts = 0;
            _expireDate = new DateTime(2034, 03, 22);
            _currency = "USD";
            DailyWithdrawLimit = 100000m;
            WithdrawnToday = 0;
            _lastWithdrawDate = new DateTime(2025, 11, 22);
            History = new List<Transaction>();
            Status = AccountStatus.Active;
        }

        public Account(string cardNumber, string ownerFullName, decimal balance, string pinCode) : this()
        {
            if (cardNumber != null && cardNumber.Replace(" ", "").Length == 16 && cardNumber.All(char.IsDigit))
            {
                CardNumber = cardNumber;
            }

            OwnerFullName = ownerFullName;
            Balance = balance;

            if (pinCode != null && pinCode.Length == 4 && pinCode.All(char.IsDigit))
            {
                _pinCode = pinCode;
            }
        }

        public Account(string cardNumber, string ownerFullName, decimal balance, string pinCode, decimal dailyWithdrawLimit, DateTime expireDate, string currency) : this(cardNumber, ownerFullName, balance, pinCode)
        {
            DailyWithdrawLimit = dailyWithdrawLimit;
            _expireDate = expireDate;
            _currency = currency;
        }

        public Account(Account other)
        {
            CardNumber = other.CardNumber;
            OwnerFullName = other.OwnerFullName;
            Balance = other.Balance;
            _pinCode = other._pinCode;
            _isBlocked = other._isBlocked;
            _failedPinAttempts = other._failedPinAttempts;
            _expireDate = other._expireDate;
            _currency = other._currency;
            DailyWithdrawLimit = other.DailyWithdrawLimit;
            WithdrawnToday = other.WithdrawnToday;
            _lastWithdrawDate = other._lastWithdrawDate;
            History = other.History != null ? new List<Transaction>(other.History) : new List<Transaction>();
            Status = other.Status;
        }

        public bool VerifyPin(string pin)
        {
            if (_isBlocked)
                return false;

            if (pin.Length != 4 || !pin.All(char.IsDigit))
            {
                _failedPinAttempts++;
                if (_failedPinAttempts >= 3)
                {
                    _isBlocked = true;
                    Status = AccountStatus.Blocked;
                }
                return false;
            }

            if (pin != _pinCode)
            {
                _failedPinAttempts++;
                if (_failedPinAttempts >= 3)
                {
                    _isBlocked = true;
                    Status = AccountStatus.Blocked;
                }
                return false;
            }

            _failedPinAttempts = 0;
            return true;
        }

        public void Deposit(decimal amount)
        {
            if (amount <= 0)
                throw new InvalidOperationException("Сума повинна бути більше нуля.");

            Balance += amount;
            AddTransaction(new Transaction(
                TransactionType.Deposit,
                amount,
                CardNumber ?? "0000000000000000",
                CardNumber ?? "0000000000000000"
            ));
        }

        public void Withdraw(decimal amount)
        {
            if (amount <= 0)
                throw new InvalidOperationException("Сума повинна бути більше нуля.");

            if (amount > Balance)
                throw new InvalidOperationException("Недостатньо коштів.");

            Balance -= amount;
            WithdrawnToday += amount;
            AddTransaction(new Transaction(
                TransactionType.Withdraw,
                amount,
                CardNumber ?? "0000000000000000",
                CardNumber ?? "0000000000000000"
            ));
        }

        public void AddTransaction(Transaction transaction)
        {
            if (transaction != null)
                History.Add(transaction);
        }

        public void Block()
        {
            _isBlocked = true;
            Status = AccountStatus.Blocked;
        }

        public void ResetDailyWithdrawCounter()
        {
            if (_lastWithdrawDate.Date != DateTime.Today)
            {
                WithdrawnToday = 0;
                _lastWithdrawDate = DateTime.Today;
            }
        }

        private void ResetDailyLimitIfNeeded()
        {
            ResetDailyWithdrawCounter();
        }
    }
}
