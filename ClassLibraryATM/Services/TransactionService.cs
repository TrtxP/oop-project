using ClassLibraryATM.Classes;
using ClassLibraryATM.Interfaces;

namespace ClassLibraryATM.Services
{
    public class TransactionService : ITransactionService
    {
        public void RecordTransaction(IAccount account, Transaction transaction)
        {
            if (account == null)
                throw new ArgumentNullException(nameof(account));

            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            account.AddTransaction(transaction);
        }

        public List<Transaction> GetAccountHistory(IAccount account)
        {
            if (account == null)
                throw new ArgumentNullException(nameof(account));

            return new List<Transaction>(account.History);
        }
    }
}
