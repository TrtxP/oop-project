using ClassLibraryATM.Enums;
using ClassLibraryATM.Interfaces;

namespace ClassLibraryATM.Classes
{
    public class Account : IAccount
    {
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
        public List<Transaction> History { get; private set; }

        internal Account(string cardNumber, string ownerFullName, decimal balance, string pinCode, decimal dailyWithdrawLimit = 100000m)
        {
            CardNumber = cardNumber;
            OwnerFullName = ownerFullName;
            Balance = balance;
            _pinCode = pinCode;
            DailyWithdrawLimit = dailyWithdrawLimit;
            _isBlocked = false;
            _failedPinAttempts = 0;
            _expireDate = new DateTime(2034, 03, 22);
            _currency = "USD";
            WithdrawnToday = 0;
            _lastWithdrawDate = new DateTime(2025, 11, 22);
            History = new List<Transaction>();
            Status = AccountStatus.Active;
        }

        public bool VerifyPin(string pin)
        {
            if (_isBlocked)
                return false;

            if (string.IsNullOrWhiteSpace(pin) || pin.Length != 4 || !pin.All(char.IsDigit))
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
                return;

            Balance += amount;
        }

        public void Withdraw(decimal amount)
        {
            if (amount <= 0)
                return;

            ResetDailyLimitIfNeeded();

            if (Balance < amount)
                return;

            if (WithdrawnToday + amount > DailyWithdrawLimit)
                return;

            WithdrawnToday += amount;
            Balance -= amount;
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
