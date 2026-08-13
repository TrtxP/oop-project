namespace ClassLibraryATM.Interfaces
{
    public interface ITransferService
    {
        bool CanTransfer(IAccount fromAccount, IAccount toAccount, decimal amount, decimal fee = 0);
        void ProcessTransfer(IAccount fromAccount, IAccount toAccount, decimal amount, decimal fee = 0);
    }
}
