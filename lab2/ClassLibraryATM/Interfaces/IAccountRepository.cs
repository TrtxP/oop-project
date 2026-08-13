namespace ClassLibraryATM.Interfaces
{
    public interface IAccountRepository
    {
        void Add(IAccount account);
        IAccount? FindByCardNumber(string cardNumber);
        IEnumerable<IAccount> GetAll();
        bool Exists(string cardNumber);
    }
}
