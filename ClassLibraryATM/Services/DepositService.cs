using ClassLibraryATM.Interfaces;

namespace ClassLibraryATM.Services
{
    public class DepositService : IDepositService
    {
        private readonly IAmountValidator _amountValidator;

        public DepositService(IAmountValidator amountValidator)
        {
            _amountValidator = amountValidator ?? throw new ArgumentNullException(nameof(amountValidator));
        }

        public bool CanDeposit(decimal amount)
        {
            return _amountValidator.IsValid(amount);
        }

        public void ProcessDeposit(IAccount account, decimal amount)
        {
            if (account == null)
                throw new ArgumentNullException(nameof(account));

            if (!_amountValidator.IsValid(amount))
                throw new InvalidOperationException("Невалідна сума для поповнення.");

            account.Deposit(amount);
        }
    }
}
