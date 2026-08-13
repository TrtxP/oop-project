namespace ClassLibraryATM.Interfaces
{
    public interface IWithdrawService
    {
        bool CanWithdraw(IAccount account, decimal amount, decimal atmCashAvailable);
        void ProcessWithdraw(IAccount account, decimal amount, decimal fee = 0);
    }
}
