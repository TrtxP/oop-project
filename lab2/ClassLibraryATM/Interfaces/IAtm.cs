using ClassLibraryATM.Enums;

namespace ClassLibraryATM.Interfaces
{
    public interface IAtm
    {
        string? AtmId { get; }
        string? Address { get; }
        decimal CashAvailable { get; set; }
        IBank OwnerBank { get; }
        AtmState State { get; }
        IAccount? CurrentAccount { get; }
        List<Transaction> AtmJournal { get; }

        event EventHandler<AuthenticatedEventArgs>? Authenticated;
        event EventHandler<BalanceCheckedEventArgs>? BalanceChecked;
        event EventHandler<WithdrawCompletedEventArgs>? WithdrawCompleted;
        event EventHandler<DepositCompletedEventArgs>? DepositCompleted;
        event EventHandler<TransferCompletedEventArgs>? TransferCompleted;

        bool Authenticate(string cardNumber, string pin);
        void CheckBalance();
        void Withdraw(decimal amount);
        void Deposit(decimal amount);
        void Transfer(string destinationCardNumber, decimal amount);
        void Logout();
    }

    // Event Arguments classes
    public class AuthenticatedEventArgs : EventArgs
    {
        public string CardNumber { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
    }

    public class BalanceCheckedEventArgs : EventArgs
    {
        public IAccount? Account { get; set; }
        public decimal Balance { get; set; }
        public string Message { get; set; }
    }

    public class WithdrawCompletedEventArgs : EventArgs
    {
        public IAccount? Account { get; set; }
        public decimal Amount { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
    }

    public class DepositCompletedEventArgs : EventArgs
    {
        public IAccount? Account { get; set; }
        public decimal Amount { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
    }

    public class TransferCompletedEventArgs : EventArgs
    {
        public IAccount? FromAccount { get; set; }
        public IAccount? ToAccount { get; set; }
        public decimal Amount { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
