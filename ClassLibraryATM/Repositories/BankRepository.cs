using ClassLibraryATM.Interfaces;

namespace ClassLibraryATM.Repositories
{
    public class BankRepository : IBankRepository
    {
        private readonly Dictionary<string, IBank> _banks = new();

        public void SaveBank(IBank bank)
        {
            if (bank == null)
                throw new ArgumentNullException(nameof(bank));

            if (string.IsNullOrWhiteSpace(bank.Name))
                throw new InvalidOperationException("Назва банку не може бути порожна.");

            _banks[bank.Name] = bank;
        }

        public IBank? GetBank(string bankName)
        {
            if (string.IsNullOrWhiteSpace(bankName))
                return null;

            _banks.TryGetValue(bankName, out var bank);
            return bank;
        }

        public List<IBank> GetAllBanks()
        {
            return _banks.Values.ToList();
        }
    }
}
