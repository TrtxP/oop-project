using ClassLibraryATM.Interfaces;

namespace ClassLibraryATM.Validators
{
    public class PinValidator : IPinValidator
    {
        private const int PinLength = 4;

        public bool IsValid(string pin)
        {
            if (string.IsNullOrWhiteSpace(pin))
                return false;

            return pin.Length == PinLength && pin.All(char.IsDigit);
        }

        public string GetValidationError(string pin)
        {
            if (string.IsNullOrWhiteSpace(pin))
                return "PIN код не може бути порожній.";

            if (pin.Length != PinLength)
                return $"PIN код повинен мати {PinLength} цифри, отримано: {pin.Length}";

            if (!pin.All(char.IsDigit))
                return "PIN код повинен містити тільки цифри.";

            return string.Empty;
        }
    }
}
