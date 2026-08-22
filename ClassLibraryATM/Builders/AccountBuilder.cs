using ClassLibraryATM.Classes;

namespace ClassLibraryATM.Builders
{
    public class AccountBuilder
    {
        private string? _cardNumber;
        private string? _ownerFullName;
        private decimal _balance = 0m;
        private string? _pinCode;
        private decimal _dailyWithdrawLimit = 100000m;
        private DateTime _expireDate = DateTime.Now.AddYears(5);
        private string _currency = "UAH";

        public AccountBuilder WithCardNumber(string cardNumber)
        {
            _cardNumber = cardNumber;
            return this;
        }

        public AccountBuilder WithOwnerFullName(string ownerFullName)
        {
            _ownerFullName = ownerFullName;
            return this;
        }

        public AccountBuilder WithBalance(decimal balance)
        {
            _balance = balance;
            return this;
        }

        public AccountBuilder WithPinCode(string pinCode)
        {
            _pinCode = pinCode;
            return this;
        }

        public AccountBuilder WithDailyWithdrawLimit(decimal dailyWithdrawLimit)
        {
            _dailyWithdrawLimit = dailyWithdrawLimit;
            return this;
        }

        public AccountBuilder WithExpireDate(DateTime expireDate)
        {
            _expireDate = expireDate;
            return this;
        }

        public AccountBuilder WithCurrency(string currency)
        {
            _currency = currency;
            return this;
        }

        public Account Build()
        {
            if (string.IsNullOrWhiteSpace(_cardNumber))
                throw new InvalidOperationException("Номер картки обов'язковий.");

            if (string.IsNullOrWhiteSpace(_ownerFullName))
                throw new InvalidOperationException("Ім'я власника обов'язкове.");

            if (string.IsNullOrWhiteSpace(_pinCode))
                throw new InvalidOperationException("PIN код обов'язковий.");

            return new Account(_cardNumber, _ownerFullName, _balance, _pinCode, _dailyWithdrawLimit, _expireDate, _currency);
        }
    }
}
