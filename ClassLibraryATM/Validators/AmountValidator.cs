using ClassLibraryATM.Interfaces;

namespace ClassLibraryATM.Validators
{
    public class AmountValidator : IAmountValidator
    {
        public bool IsValid(decimal amount)
        {
            return amount > 0;
        }

        public string GetValidationError(decimal amount)
        {
            if (amount <= 0)
                return "Сума повинна бути більше нуля.";

            return string.Empty;
        }
    }
}
