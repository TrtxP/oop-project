using ClassLibraryATM.Interfaces;

namespace ClassLibraryATM.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly Dictionary<string, IAccount> _accounts = new();

        public void Add(IAccount account)
        {
            if (account == null)
                throw new ArgumentNullException(nameof(account));

            if (string.IsNullOrWhiteSpace(account.CardNumber))
                throw new InvalidOperationException("Номер картки не може бути порожній.");

            if (_accounts.ContainsKey(account.CardNumber))
                throw new InvalidOperationException("Акаунт з цим номером картки вже існує.");

            _accounts[account.CardNumber] = account;
        }

        public IAccount? FindByCardNumber(string cardNumber)
        {
            if (string.IsNullOrWhiteSpace(cardNumber))
                return null;

            _accounts.TryGetValue(cardNumber, out var account);
            return account;
        }

        public IEnumerable<IAccount> GetAll()
        {
            return _accounts.Values.ToList().AsReadOnly();
        }

        public bool Exists(string cardNumber)
        {
            if (string.IsNullOrWhiteSpace(cardNumber))
                return false;

            return _accounts.ContainsKey(cardNumber);
        }
    }
}
