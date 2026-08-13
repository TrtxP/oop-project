using ClassLibraryATM.Enums;
using ClassLibraryATM.Interfaces;

namespace ClassLibraryATM.Services
{
    public class TransferService : ITransferService
    {
        private readonly IAmountValidator _amountValidator;

        public TransferService(IAmountValidator amountValidator)
        {
            _amountValidator = amountValidator ?? throw new ArgumentNullException(nameof(amountValidator));
        }

        public bool CanTransfer(IAccount fromAccount, IAccount toAccount, decimal amount, decimal fee = 0)
        {
            if (fromAccount == null || toAccount == null)
                return false;

            if (!_amountValidator.IsValid(amount))
                return false;

            decimal totalAmount = amount + fee;
            if (totalAmount > fromAccount.Balance)
                return false;

            if (fromAccount.Status != AccountStatus.Active || toAccount.Status != AccountStatus.Active)
                return false;

            return true;
        }

        public void ProcessTransfer(IAccount fromAccount, IAccount toAccount, decimal amount, decimal fee = 0)
        {
            if (fromAccount == null || toAccount == null)
                throw new ArgumentNullException("Рахунки не можуть бути нульовими.");

            if (!_amountValidator.IsValid(amount))
                throw new InvalidOperationException("Невалідна сума для переказу.");

            decimal totalAmount = amount + fee;
            if (totalAmount > fromAccount.Balance)
                throw new InvalidOperationException("Недостатньо коштів для переказу.");

            fromAccount.Withdraw(totalAmount);
            toAccount.Deposit(amount);
        }
    }
}
