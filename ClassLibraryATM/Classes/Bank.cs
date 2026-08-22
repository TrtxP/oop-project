using ClassLibraryATM.Interfaces;
using ClassLibraryATM.Repositories;

namespace ClassLibraryATM.Classes
{
    public class Bank : IBank
    {
        private readonly IAccountRepository _accountRepository;
        private decimal _transferFeePercent;
        private decimal _minBalanceRequired;

        public string? Name { get; private set; }
        public IReadOnlyDictionary<string, IAccount> Accounts =>
            _accountRepository.GetAll().ToDictionary(a => a.CardNumber ?? string.Empty);
        public List<Transaction> BackLedger { get; private set; }

        public Bank() : this("ATM №12", new AccountRepository())
        {
        }

        public Bank(string name) : this(name, new AccountRepository())
        {
        }

        public Bank(string name, IAccountRepository accountRepository)
        {
            Name = name;
            _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
            _transferFeePercent = 0;
            _minBalanceRequired = 0;
            BackLedger = new List<Transaction>();
        }

        public Bank(string name, Dictionary<string, IAccount> accounts) : this(name, new AccountRepository())
        {
            if (accounts != null)
            {
                foreach (var acc in accounts.Values)
                {
                    _accountRepository.Add(acc);
                }
            }
        }

        public Bank(string name, Dictionary<string, IAccount> accounts, decimal transferFeePercent, decimal minBalanceRequired) : this(name, accounts)
        {
            _transferFeePercent = transferFeePercent;
            _minBalanceRequired = minBalanceRequired;
        }

        public Bank(Bank other) : this(other.Name ?? "Банк", new AccountRepository())
        {
            _transferFeePercent = other._transferFeePercent;
            _minBalanceRequired = other._minBalanceRequired;
            BackLedger = new List<Transaction>(other.BackLedger);
            foreach (var acc in other._accountRepository.GetAll())
            {
                _accountRepository.Add(acc);
            }
        }

        public void RegisterAccount(IAccount acc)
        {
            if (acc == null)
                throw new ArgumentNullException(nameof(acc));

            _accountRepository.Add(acc);
        }

        public IAccount? FindAccount(string cardNumber)
        {
            if (string.IsNullOrWhiteSpace(cardNumber))
                return null;

            return _accountRepository.FindByCardNumber(cardNumber);
        }
    }
}
