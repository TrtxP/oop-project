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

            if (ReferenceEquals(fromAccount, toAccount) || (!string.IsNullOrEmpty(fromAccount.CardNumber) && fromAccount.CardNumber == toAccount.CardNumber))
                return false;

            if (!_amountValidator.IsValid(amount))
                return false;

            if (fee < 0)
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

            if (ReferenceEquals(fromAccount, toAccount) || (!string.IsNullOrEmpty(fromAccount.CardNumber) && fromAccount.CardNumber == toAccount.CardNumber))
                throw new InvalidOperationException("Неможливо здійснити переказ на той самий рахунок.");

            if (!_amountValidator.IsValid(amount))
                throw new InvalidOperationException("Невалідна сума для переказу.");

            if (fee < 0)
                throw new InvalidOperationException("Комісія не може бути від'ємною.");

            decimal totalAmount = amount + fee;
            if (totalAmount > fromAccount.Balance)
                throw new InvalidOperationException("Недостатньо коштів для переказу.");

            if (fromAccount.Status != AccountStatus.Active || toAccount.Status != AccountStatus.Active)
                throw new InvalidOperationException("Один з рахунків не є активним.");

            fromAccount.Withdraw(totalAmount);
            toAccount.Deposit(amount);
        }
    }
}
