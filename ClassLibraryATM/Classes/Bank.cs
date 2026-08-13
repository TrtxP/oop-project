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
        public List<Transaction> BackLedger { get; private set; }

        public Bank()
        {
            Name = "ATM №12";
            _accounts = new Dictionary<string, IAccount>();
            _transferFeePercent = 0;
            _minBalanceRequired = 0;
            BackLedger = new List<Transaction>();
        }

        public Bank(string name) : this()
        {
            Name = name;
        }

        public Bank(string name, Dictionary<string, IAccount> accounts) : this(name)
        {
            _accounts = accounts ?? new Dictionary<string, IAccount>();
        }

        public Bank(string name, Dictionary<string, IAccount> accounts, decimal transferFeePercent, decimal minBalanceRequired) : this(name, accounts)
        {
            _transferFeePercent = transferFeePercent;
            _minBalanceRequired = minBalanceRequired;
        }

        public Bank(Bank other)
        {
            Name = other.Name;
            _accounts = new Dictionary<string, IAccount>(other._accounts);
            _transferFeePercent = other._transferFeePercent;
            _minBalanceRequired = other._minBalanceRequired;
            BackLedger = new List<Transaction>(other.BackLedger);
        }

        public void RegisterAccount(IAccount acc)
        {
            if (acc?.CardNumber != null)
            {
            _accounts[acc.CardNumber] = acc;
        }
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
