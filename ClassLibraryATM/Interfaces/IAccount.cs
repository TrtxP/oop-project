using ClassLibraryATM.Enums;

namespace ClassLibraryATM.Interfaces
{
    public interface IAccount
    {
        string? CardNumber { get; }
        string? OwnerFullName { get; }
        decimal Balance { get; }
        AccountStatus Status { get; }
        decimal DailyWithdrawLimit { get; }
        decimal WithdrawnToday { get; }
        List<Transaction> History { get; }

        bool VerifyPin(string pin);
        void Deposit(decimal amount);
        void Withdraw(decimal amount);
        void AddTransaction(Transaction transaction);
        void Block();
        void ResetDailyWithdrawCounter();
    }
}
