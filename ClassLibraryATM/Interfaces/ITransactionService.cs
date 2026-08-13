using ClassLibraryATM.Classes;

namespace ClassLibraryATM.Interfaces
{
    public interface ITransactionService
    {
        void RecordTransaction(IAccount account, Transaction transaction);
        List<Transaction> GetAccountHistory(IAccount account);
    }
}
