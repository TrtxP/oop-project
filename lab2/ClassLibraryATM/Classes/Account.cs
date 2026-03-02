using ClassLibraryATM.Enums;

namespace ClassLibraryATM.Classes
{
    public class Account
    {
        public string? CardNumber { get; private set; }
        public string? OwnerFullName { get; private set; }
        public decimal Balance { get; private set; }
        private string? PinCode { get; set; }
        private bool IsBlocked { get; set; }
        private int FailedPinAttempts { get; set; }
        private DateTime ExpireDate { get; set; }
        private string? Currency { get; set; }
        public decimal DailyWithdrawLimit { get; private set; }
        public decimal WithdrawnToday { get; private set; }
        private DateTime LastWithdrawDate { get; set; }
        public List<Transaction> History { get; private set; }

        public AccountStatus Status;

        public Account() {
            CardNumber = "0000 0000 0000 0000";
            OwnerFullName = "Прізвище Ім'я";
            Balance = 0;
            PinCode = "0000";
            IsBlocked = false;
            FailedPinAttempts = 0;
            ExpireDate = new DateTime(2034, 03, 22);
            Currency = "USD";
            DailyWithdrawLimit = 100000m;
            WithdrawnToday = 0;
            LastWithdrawDate = new DateTime(2025, 11, 22);
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
                PinCode = pinCode;
            }
        }

        public Account(Account other)
        {
            CardNumber = other.CardNumber;
            OwnerFullName = other.OwnerFullName;
            Balance = other.Balance;
            PinCode = other.PinCode;
            IsBlocked = other.IsBlocked;
            FailedPinAttempts = other.FailedPinAttempts;
            ExpireDate = other.ExpireDate;
            Currency = other.Currency;
            DailyWithdrawLimit = other.DailyWithdrawLimit;
            WithdrawnToday = other.WithdrawnToday;
            LastWithdrawDate = other.LastWithdrawDate;
            History = other.History != null ? new List<Transaction>(other.History) : new List<Transaction>();
            Status = other.Status;
        }

        public bool VerifyPin(string pin)
        {
            if (IsBlocked) return false;

            if (pin.Length != 4 || !pin.All(char.IsDigit))
            {
                FailedPinAttempts++;
                if (FailedPinAttempts >= 3)
                {
                    IsBlocked = true;
                    Status = AccountStatus.Blocked;
                }
                return false;
            }

            if (pin != PinCode)
            {
                FailedPinAttempts++;
                if (FailedPinAttempts >= 3)
                {
                    IsBlocked = true;
                    Status = AccountStatus.Blocked;
                }
                return false;
            }

            FailedPinAttempts = 0;
            return true;
        }

        public void Credit(decimal amount)
        {
            if (amount <= 0)
            {
                return;
            }

            if (amount > 29999m)
            {
                return;
            }

            Balance += amount;

            History.Add(new Transaction());
        }

        public void Debit(decimal amount)
        {
            if (amount <= 0)
            {
                return;
            }

            ResetDailyLimitIfNeeded();

            if (Balance < amount)
            {
                return;
            }

            if (WithdrawnToday + amount > DailyWithdrawLimit)
            {
                return;
            }

            WithdrawnToday += amount;
            Balance -= amount;

            History.Add(new Transaction());
        }

        public void ResetDailyLimitIfNeeded()
        {
            if (LastWithdrawDate.Date != DateTime.Today)
            {
                WithdrawnToday = 0;
                LastWithdrawDate = DateTime.Today;
            }
        }
    }
}
