namespace ClassLibraryATM.Interfaces
{
    public interface IBank
    {
        string? Name { get; }
        IReadOnlyDictionary<string, IAccount> Accounts { get; }
        List<Transaction> BackLedger { get; }

        void RegisterAccount(IAccount account);
        IAccount? FindAccount(string cardNumber);
    }
}
