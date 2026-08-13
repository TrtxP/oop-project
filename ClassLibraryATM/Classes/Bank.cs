using ClassLibraryATM.Interfaces;

namespace ClassLibraryATM.Classes
{
    public class Bank : IBank
    {
        private readonly Dictionary<string, IAccount> _accounts;
        private decimal _transferFeePercent;
        private decimal _minBalanceRequired;

        public string? Name { get; private set; }
        public IReadOnlyDictionary<string, IAccount> Accounts => _accounts.AsReadOnly();
        public List<AutomatedTellerMachine>? Atms { get; private set; }
        public List<Transaction> BackLedger { get; private set; }

        public Bank(string? name = null)
        {
            Name = name ?? "ATM №12";
            _accounts = new Dictionary<string, IAccount>();
            Atms = new List<AutomatedTellerMachine>();
            _transferFeePercent = 0;
            _minBalanceRequired = 0;
            BackLedger = new List<Transaction>();
        }

        public void RegisterAccount(IAccount acc)
        {
            if (acc == null)
                throw new ArgumentNullException(nameof(acc));

            if (string.IsNullOrWhiteSpace(acc.CardNumber))
                throw new InvalidOperationException("Номер картки не може бути порожній.");

            if (_accounts.ContainsKey(acc.CardNumber))
                throw new InvalidOperationException("Акаунт з цим номером уже зареєстрований.");

            _accounts[acc.CardNumber] = acc;
        }

        public IAccount? FindAccount(string cardNumber)
        {
            if (string.IsNullOrWhiteSpace(cardNumber))
                return null;

            _accounts.TryGetValue(cardNumber, out var acc);
            return acc;
        }
    }
}
