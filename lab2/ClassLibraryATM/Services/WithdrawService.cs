using ClassLibraryATM.Enums;
using ClassLibraryATM.Interfaces;

namespace ClassLibraryATM.Services
{
    public class WithdrawService : IWithdrawService
    {
        private readonly IAmountValidator _amountValidator;

        public WithdrawService(IAmountValidator amountValidator)
        {
            _amountValidator = amountValidator ?? throw new ArgumentNullException(nameof(amountValidator));
        }

        public bool CanWithdraw(IAccount account, decimal amount, decimal atmCashAvailable)
        {
            if (account == null)
                throw new ArgumentNullException(nameof(account));

            if (!_amountValidator.IsValid(amount))
                return false;

            if (amount > atmCashAvailable)
                return false;

            if (amount > account.Balance)
                return false;

            if (account.WithdrawnToday + amount > account.DailyWithdrawLimit)
                return false;

            return true;
        }

        public void ProcessWithdraw(IAccount account, decimal amount, decimal fee = 0)
        {
            if (account == null)
                throw new ArgumentNullException(nameof(account));

            if (!_amountValidator.IsValid(amount))
                throw new InvalidOperationException("Невалідна сума для зняття.");

            decimal totalAmount = amount + fee;
            if (totalAmount > account.Balance)
                throw new InvalidOperationException("Недостатньо коштів для зняття.");

            account.Withdraw(totalAmount);
        }
    }
}
