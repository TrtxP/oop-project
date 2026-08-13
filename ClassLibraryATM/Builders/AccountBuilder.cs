using ClassLibraryATM.Enums;

namespace ClassLibraryATM.Builders
{
    public class AccountBuilder
    {
        private string? _cardNumber;
        private string? _ownerFullName;
        private decimal _balance = 0;
        private string? _pinCode;
        private decimal _dailyWithdrawLimit = 100000m;

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

        public AccountBuilder WithDailyWithdrawLimit(decimal limit)
        {
            _dailyWithdrawLimit = limit;
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

            return new Account(_cardNumber, _ownerFullName, _balance, _pinCode, _dailyWithdrawLimit);
        }
    }
}
