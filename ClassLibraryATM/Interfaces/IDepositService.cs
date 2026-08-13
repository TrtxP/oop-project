namespace ClassLibraryATM.Interfaces
{
    public interface IDepositService
    {
        bool CanDeposit(decimal amount);
        void ProcessDeposit(IAccount account, decimal amount);
    }
}
